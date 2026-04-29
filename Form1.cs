using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CyntecMESEquipment;

namespace VNA_Data_Grabber
{
    public partial class Form1 : Form, IMessageFilter
    {
        private const int VnaPort = 5025;
        private const int TimeoutMs = 2000; 
        private string _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        private List<TraceRecord> _currentSessionData = new List<TraceRecord>();
        
        // MES 服務
        private MESService? _mesService;
        private string _currentUserId = "";
        private string _currentUserName = "";
        private JToken? _currentWoInfo;
        private AppConfig _lastLoadedConfig = new AppConfig();

        // 閒置計時器 (15分鐘)
        private Timer _inactivityTimer = new Timer();
        private const int InactivityLimitMs = 15 * 60 * 1000; 

        public Form1()
        {
            InitializeComponent();
            try { this.Icon = new System.Drawing.Icon("vna.ico"); } catch { }
            
            this.Load += (s, e) => LoadConfig();
            this.FormClosing += (s, e) => SaveConfig();

            txtMesUserId.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter) btnMesLogin_Click(this, EventArgs.Empty);
            };

            _inactivityTimer.Interval = InactivityLimitMs;
            _inactivityTimer.Tick += (s, e) => PerformLogout("系統閒置超過 15 分鐘，已自動登出。");
            Application.AddMessageFilter(this);
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x0201 || m.Msg == 0x0204 || m.Msg == 0x0207 || m.Msg == 0x0100)
            {
                ResetInactivityTimer();
            }
            return false;
        }

        private void ResetInactivityTimer()
        {
            if (!string.IsNullOrEmpty(_currentUserId))
            {
                _inactivityTimer.Stop();
                _inactivityTimer.Start();
            }
        }

        public class AppConfig
        {
            public string IP { get; set; } = "169.254.76.8";
            public string TraceList { get; set; } = "1,3,5,6";
            public string MarkerCount { get; set; } = "5";
            public string MachNo { get; set; } = "VNA001";
            public string MesUrl { get; set; } = "http://twcynmesqas01/CyntecDataCenter/service/Eqp/Eqp_Portal.asmx";
            public bool TestMode { get; set; } = false;
        }

        private void LoadConfig()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    string json = File.ReadAllText(_configPath);
                    var config = JsonConvert.DeserializeObject<AppConfig>(json);
                    if (config != null)
                    {
                        _lastLoadedConfig = config;
                        txtIPAddress.Text = config.IP;
                        txtTraceList.Text = config.TraceList;
                        txtMarkerCount.Text = config.MarkerCount;
                        _mesService = new MESService(config.MachNo);
                        _mesService.SetUrl(config.MesUrl);
                        
                        string title = $"VNA Data Grabber - {config.MachNo}";
                        if (config.TestMode) title += " [測試模式]";
                        this.Text = title;
                    }
                }
                else
                {
                    _lastLoadedConfig = new AppConfig();
                    _mesService = new MESService(_lastLoadedConfig.MachNo);
                    _mesService.SetUrl(_lastLoadedConfig.MesUrl);
                    this.Text = $"VNA Data Grabber - {_lastLoadedConfig.MachNo}";
                    SaveConfig(); 
                }
            }
            catch (Exception ex) { UpdateStatus("載入設定失敗: " + ex.Message, false); }
        }

        private void SaveConfig()
        {
            try
            {
                var config = new AppConfig
                {
                    IP = txtIPAddress.Text.Trim(),
                    TraceList = txtTraceList.Text.Trim(),
                    MarkerCount = txtMarkerCount.Text.Trim(),
                    MachNo = _lastLoadedConfig.MachNo,
                    MesUrl = _lastLoadedConfig.MesUrl,
                    TestMode = _lastLoadedConfig.TestMode
                };
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(_configPath, json);
            }
            catch { }
        }

        #region VNA 讀取邏輯
        private async void btnReadData_Click(object sender, EventArgs e)
        {
            string ip = txtIPAddress.Text.Trim();
            string traceInput = txtTraceList.Text.Trim();
            if (!int.TryParse(txtMarkerCount.Text.Trim(), out int markerCount)) markerCount = 5;

            try
            {
                btnReadData.Enabled = false;
                UpdateStatus(_lastLoadedConfig.TestMode ? "[測試模式] 正在產生模擬數據..." : "讀取儀器中...", false);
                
                if (_lastLoadedConfig.TestMode)
                {
                    await Task.Delay(500);
                    _currentSessionData = GenerateMockData(traceInput.Split(','), markerCount);
                }
                else
                {
                    _currentSessionData = await Task.Run(() => FetchVnaData(ip, traceInput.Split(','), markerCount));
                }
                
                foreach (DataGridViewRow row in dgvManagement.Rows)
                {
                    if (row.Cells[2].Value?.ToString() == "NO")
                    {
                        row.Cells[2].Value = "YES";
                        row.Cells[2].Style.ForeColor = System.Drawing.Color.DarkGreen;
                    }
                }

                DisplayData();
                UpdateStatus(_lastLoadedConfig.TestMode ? "測試數據載入成功" : "讀取成功", true);
            }
            catch (Exception ex) { UpdateStatus("讀取失敗: " + ex.Message, false); }
            finally { btnReadData.Enabled = true; }
        }

        private List<TraceRecord> GenerateMockData(string[] traceIds, int markerCount)
        {
            var records = new List<TraceRecord>();
            var rnd = new Random();
            foreach (string tid in traceIds.Select(s => s.Trim()))
            {
                var rec = new TraceRecord { TraceNum = tid, TraceResult = "Pass" };
                for (int i = 1; i <= markerCount; i++)
                {
                    rec.Markers.Add(new MarkerInfo { 
                        Name = $"{i}.000GHz", 
                        Value = (rnd.NextDouble() * -50.0).ToString("F6") 
                    });
                }
                records.Add(rec);
            }
            return records;
        }

        private List<TraceRecord> FetchVnaData(string ip, string[] traceIds, int markerCount)
        {
            List<TraceRecord> records = new List<TraceRecord>();
            using (TcpClient client = new TcpClient())
            {
                var connectTask = client.ConnectAsync(ip, VnaPort);
                if (!connectTask.Wait(TimeoutMs)) throw new Exception("連線儀器逾時。");
                using (NetworkStream stream = client.GetStream())
                {
                    stream.ReadTimeout = TimeoutMs;
                    foreach (string tid in traceIds.Select(s => s.Trim()))
                    {
                        TraceRecord rec = new TraceRecord { TraceNum = tid };
                        SendCommand(stream, $"CALCulate1:PARameter:MNUMber {tid}");
                        for (int i = 1; i <= markerCount; i++)
                        {
                            SendCommand(stream, $"CALCulate1:MARKer{i}:X?");
                            string x = ReadResponse(stream);
                            string freq = (double.Parse(x) / 1e9).ToString("F3") + "GHz";
                            SendCommand(stream, $"CALCulate1:MARKer{i}:Y?");
                            string y = ReadResponse(stream).Split(',')[0].Trim();
                            rec.Markers.Add(new MarkerInfo { Name = freq, Value = double.Parse(y).ToString("F6") });
                        }
                        SendCommand(stream, "CALCulate1:LIMit:FAIL?");
                        rec.TraceResult = (ReadResponse(stream) == "1") ? "Fail" : "Pass";
                        records.Add(rec);
                    }
                }
            }
            return records;
        }

        private void DisplayData()
        {
            txtDisplay.Clear();
            string orderNo = txtOrderNo.Text.Trim();
            bool globalPass = _currentSessionData.All(r => r.TraceResult == "Pass");
            txtDisplay.AppendText($"[工單: {orderNo} 結果: {(globalPass ? "PASS" : "FAIL")}]\n");
            foreach (var r in _currentSessionData)
                txtDisplay.AppendText($"Tr{r.TraceNum}: {r.TraceResult} | {string.Join(", ", r.Markers.Select(m => $"{m.Name}:{m.Value}"))}\n");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            MessageBox.Show("資料已記錄至記憶體，請點擊 MES 上傳功能或手動確認。");
        }
        #endregion

        #region MES 整合邏輯

        private void btnMesLogin_Click(object sender, EventArgs e)
        {
            string barcode = txtMesUserId.Text.Trim();
            if (string.IsNullOrEmpty(barcode)) { MessageBox.Show("請刷入人員二維碼"); return; }

            var res = _mesService?.UserVerify("", "", barcode);
            if (res != null && res.IsSuccess)
            {
                string actualUid = res.GetValue<string>("UserID") ?? barcode;
                string userName = res.GetValue<string>("UserName") ?? "未知用戶";
                _currentUserId = actualUid;
                _currentUserName = userName;
                
                lblMesUser.Text = $"登入者: {userName} ({actualUid})";
                txtMesUserId.Clear();
                SetMesUIState(true);
                UpdateStatus("MES 登入成功", true);
                _inactivityTimer.Start();
            }
            else
            {
                MessageBox.Show("登入失敗: " + res?.Message);
                txtMesUserId.SelectAll();
            }
        }

        private void btnMesLogout_Click(object sender, EventArgs e)
        {
            PerformLogout();
        }

        private void PerformLogout(string? alertMsg = null)
        {
            _currentUserId = "";
            _currentUserName = "";
            _currentWoInfo = null;
            lblMesUser.Text = "登入狀態: 離線";
            
            txtWorkCenterNo.Clear();
            txtWorkCenterName.Clear();
            txtRefInputQty.Clear();
            txtMatNo.Clear();
            txtWipQty.Clear();
            txtCheckInUser.Clear();
            txtOrderNo.Clear();
            dgvManagement.Rows.Clear();

            SetMesUIState(false);
            _inactivityTimer.Stop();
            if (alertMsg != null) MessageBox.Show(alertMsg, "系統提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtMesUserId.Focus();
        }

        private void btnMesQuery_Click(object sender, EventArgs e)
        {
            string wo = txtOrderNo.Text.Trim();
            if (string.IsNullOrEmpty(wo)) { MessageBox.Show("請輸入工單號碼"); return; }

            var res = _mesService?.WOQRY(wo, _currentUserId);
            if (res != null && res.IsSuccess)
            {
                _currentWoInfo = res.RawFields != null ? JToken.FromObject(res.RawFields) : null;
                
                txtWorkCenterNo.Text = res.GetValue<string>("WorkCenterNo");
                txtWorkCenterName.Text = res.GetValue<string>("WorkCenterName");
                txtRefInputQty.Text = res.GetValue<string>("RefInputQty");
                txtMatNo.Text = res.GetValue<string>("MatNo");
                txtWipQty.Text = res.GetValue<string>("WIPQty");
                
                string alreadyIn = res.GetValue<string>("AlreadyInFlag") ?? "N";
                if (alreadyIn == "Y")
                {
                    txtCheckInUser.Text = $"{_currentUserId}-{_currentUserName} (已進站)";
                    ParseInputCodeList(res.GetValue<string>("InputCodeListDT"));
                    btnMesTrackIn.Enabled = false;
                    UpdateStatus("工單查詢成功 (此工單已進站)", true);
                }
                else
                {
                    txtCheckInUser.Clear();
                    dgvManagement.Rows.Clear();
                    btnMesTrackIn.Enabled = true;
                    UpdateStatus("工單查詢成功 (尚未進站)", true);
                }
            }
            else MessageBox.Show("查詢失敗: " + res?.Message);
        }

        private void btnMesTrackIn_Click(object sender, EventArgs e)
        {
            var res = _mesService?.WOCHECKIN(txtOrderNo.Text.Trim(), _currentUserId);
            if (res != null && res.IsSuccess)
            {
                UpdateStatus("工單進站成功，正在刷新資訊...", true);
                btnMesQuery_Click(this, EventArgs.Empty);
            }
            else MessageBox.Show("進站失敗: " + res?.Message);
        }

        private void ParseInputCodeList(string? json)
        {
            dgvManagement.Rows.Clear();
            if (string.IsNullOrEmpty(json)) return;

            try
            {
                JArray items = JArray.Parse(json);
                foreach (var item in items)
                {
                    string code = item["InputCode"]?.ToString() ?? "";
                    string name = item["InputCodeName"]?.ToString() ?? "";
                    string dataListItem = item["DataListItem"]?.ToString() ?? "";
                    string result = dataListItem.Contains("YES;NO") ? "NO" : "";
                    
                    int rowIndex = dgvManagement.Rows.Add(code, name, result);
                    if (result == "NO") dgvManagement.Rows[rowIndex].Cells[2].Style.ForeColor = System.Drawing.Color.DarkRed;
                }
            }
            catch { }
        }

        private void btnMesTrackOut_Click(object sender, EventArgs e)
        {
            string wo = txtOrderNo.Text.Trim();
            if (_currentSessionData.Count == 0) { MessageBox.Show("請先讀取儀器資料"); return; }

            // 準備管理項目資料 (InputListDt)
            JArray inputList = new JArray();
            foreach (DataGridViewRow row in dgvManagement.Rows)
            {
                string code = row.Cells[0].Value?.ToString() ?? "";
                string result = row.Cells[2].Value?.ToString() ?? "";
                if (result == "NO")
                {
                    MessageBox.Show($"項目「{row.Cells[1].Value}」尚未完成，無法出站。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                inputList.Add(new JObject { ["InputCode"] = code, ["InputValue"] = result });
            }

            // 1. 上傳資料：dtEDCRawData 為空陣列，但帶入管理項目 InputListDt
            var resData = _mesService?.EDCDATAADD(wo, _currentUserId, new JArray(), inputList);
            if (resData == null || !resData.IsSuccess) { MessageBox.Show("資料上傳失敗: " + resData?.Message); return; }

            // 2. 執行工單出站 (不再重複傳送管理項目)
            var resOut = _mesService?.WOCHECKOUT(wo, _currentUserId, 1);
            if (resOut != null && resOut.IsSuccess)
            {
                UpdateStatus("工單出站成功", true);
                MessageBox.Show("MES 作業完成！程式將自動登出。");
                PerformLogout(); 
            }
            else MessageBox.Show("出站失敗: " + resOut?.Message);
        }

        private void SetMesUIState(bool loggedIn)
        {
            btnMesLogin.Enabled = !loggedIn;
            txtMesUserId.Enabled = !loggedIn;
            btnMesLogout.Enabled = loggedIn;
            btnMesQuery.Enabled = loggedIn;
            btnMesTrackIn.Enabled = false; 
            btnMesTrackOut.Enabled = loggedIn;
            txtOrderNo.Enabled = loggedIn;
        }
        #endregion

        private void SendCommand(NetworkStream stream, string cmd)
        {
            byte[] data = Encoding.ASCII.GetBytes(cmd + "\n");
            stream.Write(data, 0, data.Length);
        }

        private string ReadResponse(NetworkStream stream)
        {
            StringBuilder sb = new StringBuilder();
            byte[] buffer = new byte[1024];
            DateTime start = DateTime.Now;
            while (true)
            {
                if (stream.DataAvailable)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    sb.Append(Encoding.ASCII.GetString(buffer, 0, read));
                }
                if (sb.ToString().Contains("\n")) return sb.ToString().Trim();
                if ((DateTime.Now - start).TotalMilliseconds > TimeoutMs) throw new Exception("儀器回應逾時。");
                System.Threading.Thread.Sleep(5);
            }
        }

        private void UpdateStatus(string msg, bool success)
        {
            lblStatus.Text = $"最後操作: {msg}";
            lblStatus.ForeColor = success ? System.Drawing.Color.DarkGreen : System.Drawing.Color.DarkRed;
        }

        public class MarkerInfo { public string Name { get; set; } = ""; public string Value { get; set; } = ""; }
        public class TraceRecord { public string TraceNum { get; set; } = ""; public string TraceResult { get; set; } = ""; public List<MarkerInfo> Markers { get; set; } = new List<MarkerInfo>(); }
    }
}
