using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;

namespace VNA_Data_Grabber
{
    public partial class Form1 : Form
    {
        private const int VnaPort = 5025;
        private const int TimeoutMs = 2000; 
        private string _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

        private List<TraceRecord> _currentSessionData = new List<TraceRecord>();

        public Form1()
        {
            InitializeComponent();
            try { this.Icon = new System.Drawing.Icon("vna.ico"); } catch { }
            this.Load += (s, e) => LoadConfig();
            this.FormClosing += (s, e) => SaveConfig();
        }

        public class AppConfig
        {
            public string IP { get; set; } = "192.168.1.100";
            public string TraceList { get; set; } = "1";
            public string MarkerCount { get; set; } = "5";
        }

        private void LoadConfig()
        {
            try
            {
                if (File.Exists(_configPath))
                {
                    string json = File.ReadAllText(_configPath);
                    var config = JsonSerializer.Deserialize<AppConfig>(json);
                    if (config != null)
                    {
                        txtIPAddress.Text = config.IP;
                        txtTraceList.Text = config.TraceList;
                        txtMarkerCount.Text = config.MarkerCount;
                    }
                }
            }
            catch { }
        }

        private void SaveConfig()
        {
            try
            {
                var config = new AppConfig
                {
                    IP = txtIPAddress.Text,
                    TraceList = txtTraceList.Text,
                    MarkerCount = txtMarkerCount.Text
                };
                string json = JsonSerializer.Serialize(config);
                File.WriteAllText(_configPath, json);
            }
            catch { }
        }

        public class MarkerInfo
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class TraceRecord
        {
            public string TraceNum { get; set; }
            public string TraceResult { get; set; }
            public List<MarkerInfo> Markers { get; set; } = new List<MarkerInfo>();
        }

        private async void btnReadData_Click(object sender, EventArgs e)
        {
            string ip = txtIPAddress.Text.Trim();
            string traceInput = txtTraceList.Text.Trim();
            string markerInput = txtMarkerCount.Text.Trim();

            if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(traceInput))
            {
                MessageBox.Show("請輸入 IP 與 Trace 列表。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(markerInput, out int markerCount)) markerCount = 5;

            try
            {
                UpdateStatus("讀取儀器資料中...", false);
                btnReadData.Enabled = false;
                
                txtDisplay.Clear(); 
                txtDisplay.ForeColor = System.Drawing.Color.Black; // 恢復預設顏色
                
                _currentSessionData.Clear();

                string[] traceIds = traceInput.Split(',').Select(s => s.Trim()).ToArray();
                _currentSessionData = await Task.Run(() => FetchVnaData(ip, traceIds, markerCount));

                DisplayData();
                UpdateStatus("讀取成功", true);
                SaveConfig();
            }
            catch (Exception ex)
            {
                // 使用 RichTextBox 顯示紅色錯誤
                txtDisplay.SelectionStart = txtDisplay.TextLength;
                txtDisplay.SelectionColor = System.Drawing.Color.Red;
                txtDisplay.AppendText(Environment.NewLine + "--------------------------------------");
                txtDisplay.AppendText(Environment.NewLine + "[讀取中斷] 錯誤原因: " + ex.Message);
                txtDisplay.AppendText(Environment.NewLine + "--------------------------------------");
                txtDisplay.SelectionColor = txtDisplay.ForeColor; // 恢復顏色

                UpdateStatus("讀取失敗: " + ex.Message, false);
            }
            finally
            {
                btnReadData.Enabled = true;
            }
        }

        private List<TraceRecord> FetchVnaData(string ip, string[] traceIds, int markerCount)
        {
            List<TraceRecord> records = new List<TraceRecord>();
            using (TcpClient client = new TcpClient())
            {
                var connectTask = client.ConnectAsync(ip, VnaPort);
                if (!connectTask.Wait(TimeoutMs)) throw new Exception("連線儀器失敗或逾時。");

                using (NetworkStream stream = client.GetStream())
                {
                    stream.ReadTimeout = TimeoutMs;
                    foreach (string tid in traceIds)
                    {
                        TraceRecord rec = new TraceRecord { TraceNum = tid };
                        SendCommand(stream, $"CALCulate1:PARameter:MNUMber {tid}");

                        for (int i = 1; i <= markerCount; i++)
                        {
                            SendCommand(stream, $"CALCulate1:MARKer{i}:X?");
                            string x = ReadResponse(stream);
                            if (!double.TryParse(x, out double hz))
                                throw new Exception($"Trace {tid} 無法讀取 Marker {i} 的頻率。");
                            
                            string freq = (hz / 1e9).ToString("F3") + "GHz";

                            SendCommand(stream, $"CALCulate1:MARKer{i}:Y?");
                            string y = ReadResponse(stream);
                            string valStr = y.Split(',')[0].Trim();
                            if (!double.TryParse(valStr, out double dVal))
                                throw new Exception($"Trace {tid} 無法讀取 Marker {i} 的數值。");

                            rec.Markers.Add(new MarkerInfo { Name = freq, Value = dVal.ToString("F6") });
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
            string orderNo = txtOrderNo.Text.Trim();
            if (string.IsNullOrEmpty(orderNo)) orderNo = DateTime.Now.ToString("yyyyMMddHHmmss");

            StringBuilder sb = new StringBuilder();
            bool globalPass = _currentSessionData.All(r => r.TraceResult == "Pass");
            sb.AppendLine($"[工單: {orderNo} 全局結果: {(globalPass ? "Pass" : "Fail")}]");
            foreach (var r in _currentSessionData)
            {
                string markers = string.Join(", ", r.Markers.Select(m => $"{m.Name}:{m.Value}"));
                sb.AppendLine($"Tr{r.TraceNum}: {r.TraceResult} | {markers}");
            }
            txtDisplay.AppendText(sb.ToString());
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_currentSessionData.Count == 0)
            {
                MessageBox.Show("目前無數據可存，請先成功讀取資料。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string orderNo = txtOrderNo.Text.Trim();
                if (string.IsNullOrEmpty(orderNo)) orderNo = DateTime.Now.ToString("yyyyMMddHHmmss");

                string dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MeasureData");
                if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);

                string fileName = Path.Combine(dirPath, $"{DateTime.Now:yyyyMMdd}.xlsx");
                bool globalPass = _currentSessionData.All(r => r.TraceResult == "Pass");
                string globalResult = globalPass ? "Pass" : "Fail";
                string timeStr = DateTime.Now.ToString("HH:mm:ss");

                using (var workbook = File.Exists(fileName) ? new XLWorkbook(fileName) : new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.FirstOrDefault() ?? workbook.Worksheets.Add("Log");

                    if (worksheet.LastRowUsed() == null)
                    {
                        worksheet.Cell(1, 1).Value = "時間";
                        worksheet.Cell(1, 2).Value = "工單號碼";
                        worksheet.Cell(1, 3).Value = "工單結果";
                        worksheet.Cell(1, 4).Value = "Trace結果";
                        worksheet.Cell(1, 5).Value = "Trace編號";
                        
                        int markerCount = _currentSessionData.Max(r => r.Markers.Count);
                        for (int i = 1; i <= markerCount; i++)
                        {
                            worksheet.Cell(1, 5 + (i * 2) - 1).Value = $"Mark{i}_Item";
                            worksheet.Cell(1, 5 + (i * 2)).Value = $"Mark{i}_Value";
                        }
                    }

                    int currentRow = worksheet.LastRowUsed().RowNumber() + 1;

                    foreach (var rec in _currentSessionData)
                    {
                        worksheet.Cell(currentRow, 1).Value = timeStr;
                        worksheet.Cell(currentRow, 2).Value = orderNo;
                        worksheet.Cell(currentRow, 3).Value = globalResult;
                        worksheet.Cell(currentRow, 4).Value = rec.TraceResult;
                        worksheet.Cell(currentRow, 5).Value = $"Tr{rec.TraceNum}";

                        for (int i = 0; i < rec.Markers.Count; i++)
                        {
                            worksheet.Cell(currentRow, 6 + (i * 2)).Value = rec.Markers[i].Name;
                            if (double.TryParse(rec.Markers[i].Value, out double numVal))
                            {
                                var valueCell = worksheet.Cell(currentRow, 7 + (i * 2));
                                valueCell.Value = numVal;
                                valueCell.Style.NumberFormat.Format = "0.000000";
                            }
                            else
                            {
                                worksheet.Cell(currentRow, 7 + (i * 2)).Value = rec.Markers[i].Value;
                            }
                        }
                        currentRow++;
                    }

                    workbook.SaveAs(fileName);
                }

                string msg = $"工單:{orderNo} 全局結果:{globalResult} -> 儲存完成";
                UpdateStatus(msg, true);
                
                txtDisplay.SelectionStart = txtDisplay.TextLength;
                txtDisplay.SelectionColor = System.Drawing.Color.Blue; // 儲存成功用藍色
                txtDisplay.AppendText(Environment.NewLine + ">> " + msg);
                txtDisplay.SelectionColor = txtDisplay.ForeColor;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Excel 存檔失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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
                if ((DateTime.Now - start).TotalMilliseconds > TimeoutMs)
                    throw new Exception("儀器回應逾時。");
                Task.Delay(5).Wait();
            }
        }

        private void UpdateStatus(string msg, bool success)
        {
            lblStatus.Text = $"最後操作: {msg}";
            lblStatus.ForeColor = success ? System.Drawing.Color.DarkGreen : System.Drawing.Color.DarkRed;
        }
    }
}
