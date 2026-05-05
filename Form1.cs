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
        private List<ParameterMap> _parameterMapping = new List<ParameterMap>();
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
            public string ProjectName { get; set; } = "DefaultProject";
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
            string baseName = txtOrderNo.Text.Trim();
            string projectName = _lastLoadedConfig.ProjectName;

            if (string.IsNullOrEmpty(projectName)) { MessageBox.Show("請輸入 Project Name"); return; }
            if (string.IsNullOrEmpty(baseName)) { MessageBox.Show("請先輸入工單號碼作為存檔名稱"); return; }
            if (!int.TryParse(txtMarkerCount.Text.Trim(), out int markerCount)) markerCount = 5;

            try
            {
                btnReadData.Enabled = false;
                UpdateStatus(_lastLoadedConfig.TestMode ? "[測試模式] 正在產生模擬數據..." : "讀取儀器與擷取檔案中...", false);
                
                if (_lastLoadedConfig.TestMode)
                {
                    await Task.Delay(500);
                    _currentSessionData = GenerateMockData(traceInput.Split(','), markerCount);
                    UpdateStatus("測試數據載入成功 (模擬模式不下載檔案)", true);
                }
                else
                {
                    _currentSessionData = await Task.Run(() => FetchVnaData(ip, traceInput.Split(','), markerCount, projectName, baseName));
                    UpdateStatus("數據與檔案擷取完成", true);
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
            }
            catch (Exception ex) { UpdateStatus("讀取失敗: " + ex.Message, false); MessageBox.Show("詳細錯誤: " + ex.ToString()); }
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

        private List<TraceRecord> FetchVnaData(string ip, string[] traceIds, int markerCount, string projectName, string baseName)
        {
            List<TraceRecord> records = new List<TraceRecord>();
            string localDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Exports", projectName, baseName);
            if (!Directory.Exists(localDir)) Directory.CreateDirectory(localDir);

            // E5080B 路徑格式：D:\{ProjectName}\{BaseName}
            string vnaProjDir = $@"D:\{projectName}";
            string vnaBaseDir = $@"{vnaProjDir}\{baseName}";

            using (TcpClient client = new TcpClient())
            {
                var connectTask = client.ConnectAsync(ip, VnaPort);
                if (!connectTask.Wait(TimeoutMs * 2)) throw new Exception("連線儀器逾時。");
                using (NetworkStream stream = client.GetStream())
                {
                    stream.ReadTimeout = TimeoutMs * 15; 

                    // 1. 建立儀器端目錄 (兩層確保)
                    SendCheckedCommand(stream, $":MMEMory:MDIR \"{vnaProjDir}\"");
                    SendCheckedCommand(stream, $":MMEMory:MDIR \"{vnaBaseDir}\"");
                    System.Threading.Thread.Sleep(200);

                    // 2. 讀取 Marker 數據
                    foreach (string tid in traceIds.Select(s => s.Trim()))
                    {
                        TraceRecord rec = new TraceRecord { TraceNum = tid };
                        SendCheckedCommand(stream, $"CALCulate1:PARameter:MNUMber {tid}");
                        
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

                        // 3. 儲存 Trace 的 CSV 檔案

                        string vnaCsvPath = $@"{vnaBaseDir}\Tr{tid}_CH1.csv";
                        SendCheckedCommand(stream, $":MMEMory:STORe:DATA \"{vnaCsvPath}\", \"CSV Formatted Data\", \"Trace\", \"Auto\", {tid}");
                        System.Threading.Thread.Sleep(500);
                        DownloadBinaryFile(stream, vnaCsvPath, Path.Combine(localDir, $"Tr{tid}_CH1.csv"));

                    }
                    
                    // 4. 儲存 Touchstone .s4p
                    for(int x = 1; x <= 1; x++)
                    {
                        string vnaS4pPath = $@"{vnaBaseDir}\{baseName}_CH{x}.s4p";
                        SendCheckedCommand(stream, $":CALCulate1:DATA:SNP:PORTs:SAVE \"1,2,3,4\", \"{vnaS4pPath}\"");
                        System.Threading.Thread.Sleep(1000);
                        DownloadBinaryFile(stream, vnaS4pPath, Path.Combine(localDir, $"{baseName}__CH{x}.s4p"));

                        // 5. 儲存螢幕截圖 .bmp
                        string vnaBmpPath = $@"{vnaBaseDir}\{baseName}_CH{x}.bmp";
                        SendCheckedCommand(stream, $":HCOPy:FILE \"{vnaBmpPath}\"");
                        System.Threading.Thread.Sleep(1000);
                        DownloadBinaryFile(stream, vnaBmpPath, Path.Combine(localDir, $"{baseName}_CH{x}.bmp"));
                    }
                    

                    
                }
            }
            return records;
        }

        private void SendCheckedCommand(NetworkStream stream, string cmd)
        {
            SendCommand(stream, cmd);
            SendCommand(stream, "SYSTem:ERRor?");
            string err = ReadResponse(stream);
            if (!err.Contains("No error"))
            {
                this.Invoke(new Action(() => {
                    txtDisplay.AppendText($"[儀器錯誤] 指令: {cmd} | 訊息: {err}\n");
                }));
            }
        }

        private void DownloadBinaryFile(NetworkStream stream, string vnaFilePath, string localSavePath)
        {
            // E5080B 下載檔案建議使用 MMEMory:TRANsfer?
            SendCommand(stream, $"MMEMory:TRANsfer? \"{vnaFilePath}\"");

            // 讀取 IEEE 488.2 Block Header (#<digits><length>)
            int headerStart = stream.ReadByte();
            if (headerStart != '#') return;

            int numDigits = stream.ReadByte() - '0';
            byte[] lengthBytes = new byte[numDigits];
            stream.Read(lengthBytes, 0, numDigits);
            int fileLength = int.Parse(Encoding.ASCII.GetString(lengthBytes));

            // 讀取檔案內容
            byte[] fileData = new byte[fileLength];
            int totalRead = 0;
            while (totalRead < fileLength)
            {
                int read = stream.Read(fileData, totalRead, fileLength - totalRead);
                if (read == 0) break;
                totalRead += read;
            }

            // 讀取結束符號 \n
            if (stream.DataAvailable) stream.ReadByte();

            File.WriteAllBytes(localSavePath, fileData);
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
                    ParseParameterMapping(res.GetValue<string>("InputParamListDT"));
                    btnMesTrackIn.Enabled = false;
                    UpdateStatus("工單查詢成功 (此工單已進站)", true);
                }
                else
                {
                    txtCheckInUser.Clear();
                    dgvManagement.Rows.Clear();
                    _parameterMapping.Clear();
                    btnMesTrackIn.Enabled = true;
                    UpdateStatus("工單查詢成功 (尚未進站)", true);
                }
            }
            else MessageBox.Show("查詢失敗: " + res?.Message);
        }

        private void ParseParameterMapping(string? json)
        {
            _parameterMapping.Clear();
            if (string.IsNullOrEmpty(json)) return;
            try
            {
                JArray items = JArray.Parse(json);
                foreach (var item in items)
                {
                    _parameterMapping.Add(new ParameterMap
                    {
                        ParamName = item["ParamName"]?.ToString() ?? "",
                        RefFieldCode = item["RefFieldCode"]?.ToString() ?? ""
                    });
                }
            }
            catch { }
        }

        private JArray PrepareEdcData()
        {
            JArray edcData = new JArray();
            string matNo = txtMatNo.Text.Trim();
            int sortIndex = 1;

            foreach (var rec in _currentSessionData)
            {
                foreach (var marker in rec.Markers)
                {
                    // 模糊匹配頻率: 13.000GHz -> 13GHz
                    string fuzzyMarker = marker.Name.Replace(".000", "");
                    
                    var matches = _parameterMapping.Where(p => p.ParamName.Contains(fuzzyMarker)).ToList();
                    foreach (var match in matches)
                    {
                        edcData.Add(new JObject
                        {
                            ["COMPONENTNO"] = matNo,
                            ["FIELDCODE"] = match.RefFieldCode,
                            ["VALUE"] = marker.Value,
                            ["Memo"] = $"Trace {rec.TraceNum}",
                            ["SORTINDEX"] = sortIndex.ToString()
                        });
                    }
                }
                sortIndex++;
            }
            return edcData;
        }

        private void btnMesMeasurePlan_Click(object sender, EventArgs e)
        {
            string wo = txtOrderNo.Text.Trim();
            if (string.IsNullOrEmpty(wo)) { MessageBox.Show("請輸入工單號碼"); return; }

            // 從 WOQRY 儲存的資訊中取得 FlowID 與 MatNo
            string flowId = _currentWoInfo?["FlowID"]?.ToString() ?? "";
            string matNo = txtMatNo.Text.Trim();

            UpdateStatus("正在查詢量測計畫 (EdcPlanGet)...", false);
            var res = _mesService?.WOMeasurePlanQry(wo, _currentUserId, flowId, matNo);
            if (res != null)
            {
                txtDisplay.Text = res.RawJson;
                UpdateStatus("量測計畫查詢完成", true);
            }
            else MessageBox.Show("查詢失敗，未收到回覆。");
        }

        private void btnMesEdcUpload_Click(object sender, EventArgs e)
        {
            string wo = txtOrderNo.Text.Trim();
            if (string.IsNullOrEmpty(wo)) { MessageBox.Show("請輸入工單號碼"); return; }
            if (_currentSessionData.Count == 0) { MessageBox.Show("請先讀取儀器資料"); return; }

            string flowId = _currentWoInfo?["FlowID"]?.ToString() ?? "";
            
            UpdateStatus("正在上傳量測數據 (EDCDATAADD)...", false);
            JArray edcData = PrepareEdcData();
            if (edcData.Count == 0) { MessageBox.Show("未匹配到任何量測計畫參數，請確認頻率名稱。"); return; }

            var res = _mesService?.EDCDATAADD(wo, _currentUserId, flowId, edcData, "N");
            if (res != null)
            {
                txtDisplay.Text = res.RawJson;
                if (res.IsSuccess) UpdateStatus("量測數據上傳成功", true);
                else MessageBox.Show("上傳失敗: " + res.Message);
            }
            else MessageBox.Show("上傳失敗，未收到回覆。");
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
            if (string.IsNullOrEmpty(wo)) { MessageBox.Show("請輸入工單號碼"); return; }
            if (_currentSessionData.Count == 0) { MessageBox.Show("請先讀取儀器資料"); return; }

            string flowId = _currentWoInfo?["FlowID"]?.ToString() ?? "";

            // 1. 自動 EDC 上報
            UpdateStatus("正在執行 EDC 數據上傳...", false);
            JArray edcData = PrepareEdcData();
            if (edcData.Count > 0)
            {
                var resEdc = _mesService?.EDCDATAADD(wo, _currentUserId, flowId, edcData, "N");
                txtDisplay.Text = resEdc?.RawJson;
                if (resEdc == null || !resEdc.IsSuccess)
                {
                    MessageBox.Show("EDC 上報失敗，出站流程中斷。\n錯誤訊息: " + resEdc?.Message);
                    return;
                }
                UpdateStatus("EDC 上報成功，執行出站中...", true);
            }
            else
            {
                if (MessageBox.Show("未匹配到任何量測計畫參數，是否直接嘗試出站？", "提示", MessageBoxButtons.YesNo) == DialogResult.No) return;
            }

            // 2. 執行工單出站 (準備管理項目)
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

            var resOut = _mesService?.WOCHECKOUT(wo, _currentUserId, Convert.ToInt16(txtWipQty.Text), null, inputList, null, null);
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
            btnMesMeasurePlan.Enabled = loggedIn;
            btnMesEdcUpload.Enabled = loggedIn;
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
        public class ParameterMap { public string ParamName { get; set; } = ""; public string RefFieldCode { get; set; } = ""; }
    }
}
