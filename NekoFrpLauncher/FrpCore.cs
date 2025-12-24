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

        public bool IsProxySuccess { get; private set; }
        public string LastError { get; private set; }
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

            _frpProcess.OutputDataReceived += (s, e) =>
            {
                if (string.IsNullOrEmpty(e.Data)) return;
                OnLogReceived?.Invoke(e.Data);

                if (e.Data.Contains("start proxy success") || e.Data.Contains("proxy added"))
                    IsProxySuccess = true;

                if (e.Data.Contains("start error") || e.Data.Contains("port not allowed") || e.Data.Contains("port already used"))
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

        public string LoadConfig()
        {
            if (!File.Exists(CONFIG_FILE)) return "";
            try
            {
                string raw = File.ReadAllText(CONFIG_FILE, Encoding.UTF8);
                return TryConvertIniToToml(raw);
            }
            catch { return ""; }
        }

        public void SaveConfig(string content)
        {
            string converted = TryConvertIniToToml(content);
            File.WriteAllText(CONFIG_FILE, converted, new UTF8Encoding(false));
        }

        public string TryConvertIniToToml(string content)
        {
            if (content.Contains("[[proxies]]") || content.Contains("serverAddr")) return content;
            if (content.Contains("[common]"))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("# Auto-converted from legacy INI");
                sb.AppendLine($"serverAddr = \"{ExtractValue(content, "serverAddr")}\"");
                sb.AppendLine($"serverPort = {ExtractValue(content, "serverPort")}");
                string user = ExtractValue(content, "user");
                if (!string.IsNullOrEmpty(user)) sb.AppendLine($"user = \"{user}\"");
                sb.AppendLine("auth.method = \"token\"");
                sb.AppendLine($"auth.token = \"{ExtractValue(content, "token")}\"");
                sb.AppendLine("\n[[proxies]]");
                var m = Regex.Match(content, @"^\[(?!common)(.+)\]", RegexOptions.Multiline);
                sb.AppendLine($"name = \"{(m.Success ? m.Groups[1].Value : "proxy")}\"");
                sb.AppendLine($"type = \"{ExtractValue(content, "type")}\"");
                sb.AppendLine($"localIP = \"{ExtractValue(content, "localIP")}\"");
                sb.AppendLine($"localPort = {ExtractValue(content, "localPort")}");
                sb.AppendLine($"remotePort = {ExtractValue(content, "remotePort")}");
                return sb.ToString();
            }
            return content;
        }

        public string ExtractValue(string content, string key)
        {
            string underscoreKey = Regex.Replace(key, "([a-z])([A-Z])", "$1_$2").ToLower();
            string pattern = $@"(?:{key}|{underscoreKey})\s*=\s*(?:""([^""]*)""|'([^']*)'|([^#\r\n\s]+))";
            var match = Regex.Match(content, pattern, RegexOptions.IgnoreCase);
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
