using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Windows.Forms;

namespace NekoFrpLauncher
{
    public class FrpCore
    {
        private const string CONFIG_FILE = "frpc.toml";
        private Process _frpProcess;

        // 状态标志
        public bool IsProxySuccess { get; private set; }
        public string LastError { get; private set; }

        // 日志订阅事件
        public event Action<string> OnLogReceived;

        public void StartProcess()
        {
            if (!File.Exists("frpc.exe")) throw new FileNotFoundException("未找到 frpc.exe！");

            IsProxySuccess = false;
            LastError = "";

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "frpc.exe",
                Arguments = $"-c {CONFIG_FILE}",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8
            };

            _frpProcess = new Process { StartInfo = psi };
            _frpProcess.EnableRaisingEvents = true;

            // 异步捕获输出流
            _frpProcess.OutputDataReceived += (s, e) =>
            {
                if (string.IsNullOrEmpty(e.Data)) return;
                OnLogReceived?.Invoke(e.Data); // 触发日志事件

                // 核心状态判定：只有看到成功字样才算真正运行
                if (e.Data.Contains("start proxy success") || e.Data.Contains("proxy added"))
                {
                    IsProxySuccess = true;
                }
                if (e.Data.Contains("start error") || e.Data.Contains("port not allowed"))
                {
                    IsProxySuccess = false;
                    LastError = e.Data;
                }
            };

            _frpProcess.Start();
            _frpProcess.BeginOutputReadLine();
            _frpProcess.BeginErrorReadLine();
        }

        public bool IsRunning() => _frpProcess != null && !_frpProcess.HasExited;

        public void StopProcess()
        {
            if (_frpProcess != null && !_frpProcess.HasExited)
            {
                _frpProcess.Kill();
                _frpProcess = null;
            }
            IsProxySuccess = false;
        }

        public string LoadConfig() => File.Exists(CONFIG_FILE) ? File.ReadAllText(CONFIG_FILE, Encoding.UTF8) : "";
        public void SaveConfig(string content) => File.WriteAllText(CONFIG_FILE, content, new UTF8Encoding(false));

        public string ExtractValue(string content, string key)
        {
            var regex = new Regex($@"{key}\s*=\s*(?:""([^""\r\n]*)""|'([^'\r\n]*)'|([^#\r\n]+))");
            var match = regex.Match(content);
            return match.Success ? (match.Groups[1].Value + match.Groups[2].Value + match.Groups[3].Value).Trim() : "";
        }

        public string GenerateToml(string ip, string port, string token, string localPort, string remotePort, string protocol)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"serverAddr = \"{ip.Trim()}\"");
            sb.AppendLine($"serverPort = {port.Trim()}");
            sb.AppendLine("auth.method = \"token\"");
            sb.AppendLine($"auth.token = \"{token.Trim()}\"");
            sb.AppendLine("\n[[proxies]]");
            sb.AppendLine($"name = \"game_{DateTime.Now.Ticks}\"");
            sb.AppendLine($"type = \"{protocol.Trim().ToLower()}\"");
            sb.AppendLine("localIP = \"127.0.0.1\"");
            sb.AppendLine($"localPort = {localPort.Trim()}");
            sb.AppendLine($"remotePort = {remotePort.Trim()}");
            return sb.ToString();
        }

        public void SetAutoStart(bool enable)
        {
            try
            {
                RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (enable) rk.SetValue("NekoFrpLauncher", Application.ExecutablePath);
                else rk.DeleteValue("NekoFrpLauncher", false);
            }
            catch { }
        }
    }
}