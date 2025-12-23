using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NekoFrpLauncher
{
    public class FrpCore
    {
        private const string CONFIG_FILE = "frpc.toml";
        private Process _frpProcess;

        // --- 配置文件处理 ---

        public string LoadConfig()
        {
            if (File.Exists(CONFIG_FILE))
            {
                try { return File.ReadAllText(CONFIG_FILE, Encoding.UTF8); }
                catch (Exception ex) { return "# 读取错误: " + ex.Message; }
            }
            return "# 未检测到 frpc.toml 文件\n# 请切换到“快速配置”填写，或在此处粘贴。";
        }

        public void SaveConfig(string content)
        {
            File.WriteAllText(CONFIG_FILE, content, Encoding.UTF8);
        }

        public string ExtractValue(string content, string key)
        {
            var regex = new Regex($@"{key}\s*=\s*(?:""([^""\r\n]*)""|'([^'\r\n]*)'|([^#\r\n]+))");
            var match = regex.Match(content);

            if (match.Success)
            {
                string rawValue = "";
                if (match.Groups[1].Success) rawValue = match.Groups[1].Value;
                else if (match.Groups[2].Success) rawValue = match.Groups[2].Value;
                else if (match.Groups[3].Success) rawValue = match.Groups[3].Value;
                return rawValue.Trim();
            }
            return "";
        }

        // 【修改】增加了 protocol 参数
        public string GenerateToml(string ip, string port, string token, string localPort, string remotePort, string protocol)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"serverAddr = \"{ip.Trim()}\"");
            sb.AppendLine($"serverPort = {port.Trim()}");
            sb.AppendLine("auth.method = \"token\"");
            sb.AppendLine($"auth.token = \"{token.Trim()}\"");
            sb.AppendLine();
            sb.AppendLine("[[proxies]]");
            sb.AppendLine($"name = \"game_auto_{DateTime.Now.Ticks}\"");

            // 使用传入的协议，如果是空则默认 udp
            string proto = string.IsNullOrWhiteSpace(protocol) ? "udp" : protocol.Trim().ToLower();
            sb.AppendLine($"type = \"{proto}\"");

            sb.AppendLine("localIP = \"127.0.0.1\"");
            sb.AppendLine($"localPort = {localPort.Trim()}");
            sb.AppendLine($"remotePort = {remotePort.Trim()}");
            return sb.ToString();
        }

        // --- 进程管理 ---

        public void StartProcess()
        {
            if (!File.Exists("frpc.exe")) throw new FileNotFoundException("未找到 frpc.exe！");

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "frpc.exe",
                Arguments = $"-c {CONFIG_FILE}",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = false
            };
            _frpProcess = Process.Start(psi);
        }

        public void StopProcess()
        {
            if (_frpProcess != null && !_frpProcess.HasExited)
            {
                _frpProcess.Kill();
                _frpProcess = null;
            }
        }

        public void SetAutoStart(bool enable)
        {
            try
            {
                string appName = "NekoFrpLauncher";
                string appPath = Application.ExecutablePath;
                RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (enable) rk.SetValue(appName, appPath);
                else rk.DeleteValue(appName, false);
            }
            catch (Exception ex) { Debug.WriteLine("自启设置失败: " + ex.Message); }
        }
    }
}
