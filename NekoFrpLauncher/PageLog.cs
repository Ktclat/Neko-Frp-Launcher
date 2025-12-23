using System;
using System.Drawing;
using System.Windows.Forms;

namespace NekoFrpLauncher
{
    public class PageLog : TabPage
    {
        private RichTextBox txtLog;
        private FrpCore _core;

        public PageLog(FrpCore core)
        {
            _core = core;
            this.Text = "📋 运行日志";
            this.BackColor = Color.White;

            BuildUI();

            // 订阅核心的日志事件
            _core.OnLogReceived += (logLine) => {
                if (txtLog.IsHandleCreated)
                {
                    txtLog.BeginInvoke(new Action(() => {
                        AppendLog(logLine);
                    }));
                }
            };
        }

        private void BuildUI()
        {
            txtLog = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.Gainsboro,
                Font = new Font("Consolas", 10F),
                BorderStyle = BorderStyle.None
            };

            // 顶部控制条
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 35, BackColor = Color.WhiteSmoke };
            Button btnClear = new Button { Text = "清空日志", Left = 10, Top = 5, Height = 25 };
            btnClear.Click += (s, e) => txtLog.Clear();
            pnlTop.Controls.Add(btnClear);

            this.Controls.Add(txtLog);
            this.Controls.Add(pnlTop);
            txtLog.BringToFront();
        }

        private void AppendLog(string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            txtLog.AppendText($"[{time}] {message}\n");

            // 自动滚动到底部
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }
    }
}