using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace NekoFrpLauncher
{
    public partial class MainForm : Form
    {
        private readonly FrpCore _core = new FrpCore();
        private PageFastConfig pageFast;
        private PageDetailedConfig pageDetailed;
        private PageSettings pageSettings;
        private PageAbout pageAbout;
        private TabPage pageLog; // 新增日志页
        private RichTextBox txtLogBox; // 日志显示控件

        private Button btnStart, btnStop;
        private Label lblStatus;
        private NotifyIcon trayIcon;

        public MainForm()
        {
            InitializeWindow();
            BuildUI();

            // 绑定日志事件
            _core.OnLogReceived += (logLine) => {
                if (txtLogBox.InvokeRequired)
                {
                    txtLogBox.Invoke(new Action(() => AppendLog(logLine)));
                }
                else
                {
                    AppendLog(logLine);
                }
            };
        }

        private void AppendLog(string msg)
        {
            txtLogBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}\r\n");
            txtLogBox.SelectionStart = txtLogBox.Text.Length;
            txtLogBox.ScrollToCaret();
            // 自动清理过长日志
            if (txtLogBox.Lines.Length > 500) txtLogBox.Clear();
        }

        private void InitializeWindow()
        {
            this.Text = "Neko Frp Launcher";
            this.Size = new Size(480, 580); // 略微调高布局
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ShowIcon = true;
            LoadAppIcon();

            trayIcon = new NotifyIcon
            {
                Icon = this.Icon,
                Text = "Neko Frp Launcher",
                Visible = false
            };
            trayIcon.MouseDoubleClick += (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; trayIcon.Visible = false; };
        }

        private void LoadAppIcon()
        {
            try
            {
                string resourceName = "NekoFrpLauncher.icons.logo.ico";
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null) this.Icon = new Icon(stream);
                    else this.Icon = SystemIcons.Application;
                }
            }
            catch { this.Icon = SystemIcons.Application; }
        }

        private void BuildUI()
        {
            // --- 底部面板 (找回状态指示) ---
            Panel pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(10)
            };

            btnStart = UIBuilder.CreateMainButton("🚀 启动", 100, 35);
            btnStop = UIBuilder.CreateMainButton("🛑 停止", 100, 35);
            btnStart.Location = new Point(15, 15);
            btnStop.Location = new Point(125, 15);
            btnStop.Enabled = false;

            lblStatus = UIBuilder.CreateLabel("状态: 未运行", 240, 22, false);
            lblStatus.Font = new Font("Microsoft YaHei", 10, FontStyle.Bold);
            lblStatus.ForeColor = Color.Gray;
            lblStatus.AutoSize = true;

            pnlBottom.Controls.Add(btnStart);
            pnlBottom.Controls.Add(btnStop);
            pnlBottom.Controls.Add(lblStatus);
            this.Controls.Add(pnlBottom);

            // --- Tab内容区 ---
            TabControl tabContent = new TabControl { Dock = DockStyle.Fill };

            pageFast = new PageFastConfig(_core);
            pageDetailed = new PageDetailedConfig(_core);

            // --- 创建实时日志页 ---
            pageLog = new TabPage("📃 运行日志");
            pageLog.BackColor = Color.White;
            txtLogBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.LightGreen,
                Font = new Font("Consolas", 9),
                BorderStyle = BorderStyle.None
            };
            pageLog.Controls.Add(txtLogBox);

            pageSettings = new PageSettings(_core);
            pageAbout = new PageAbout();

            tabContent.TabPages.Add(pageFast);
            tabContent.TabPages.Add(pageDetailed);
            tabContent.TabPages.Add(pageLog); // 加入日志页
            tabContent.TabPages.Add(pageSettings);
            tabContent.TabPages.Add(pageAbout);

            this.Controls.Add(tabContent);
            tabContent.BringToFront();

            btnStart.Click += Global_BtnStart_Click;
            btnStop.Click += Global_BtnStop_Click;
        }

        private async void Global_BtnStart_Click(object sender, EventArgs e)
        {
            try
            {
                btnStart.Enabled = false;
                lblStatus.Text = "状态: 🟡 正在验证...";
                lblStatus.ForeColor = Color.Orange;
                txtLogBox.Clear();
                AppendLog("正在尝试启动 frpc 进程...");

                _core.StartProcess();

                bool success = false;
                for (int i = 0; i < 40; i++) // 等待 4 秒确认日志
                {
                    await Task.Delay(100);
                    if (_core.IsProxySuccess) { success = true; break; }
                    if (!_core.IsRunning() || !string.IsNullOrEmpty(_core.LastError)) break;
                }

                if (success)
                {
                    UpdateStatus(true);
                    AppendLog(">>> FRP 服务启动成功，映射已建立。");
                }
                else
                {
                    string err = string.IsNullOrEmpty(_core.LastError) ? "连接服务器超时" : _core.LastError;
                    _core.StopProcess();
                    throw new Exception(err);
                }
            }
            catch (Exception ex)
            {
                UpdateStatus(false);
                MessageBox.Show(ex.Message, "启动失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { if (!_core.IsRunning()) btnStart.Enabled = true; }
        }

        private void Global_BtnStop_Click(object sender, EventArgs e)
        {
            _core.StopProcess();
            UpdateStatus(false);
            AppendLog("FRP 服务已手动停止。");
        }

        private void UpdateStatus(bool isRunning)
        {
            btnStart.Enabled = !isRunning;
            btnStop.Enabled = isRunning;
            if (isRunning) { lblStatus.Text = "状态: 🟢 运行中"; lblStatus.ForeColor = Color.Green; }
            else { lblStatus.Text = "状态: 🔴 已停止"; lblStatus.ForeColor = Color.Red; }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _core.StopProcess();
            base.OnFormClosing(e);
        }
    }
}