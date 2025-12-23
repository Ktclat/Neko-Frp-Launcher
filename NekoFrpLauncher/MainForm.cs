using System;
using System.Drawing;
using System.Reflection; // 必须引用，用于读取嵌入资源
using System.Windows.Forms;

namespace NekoFrpLauncher
{
    public partial class MainForm : Form
    {
        private readonly FrpCore _core = new FrpCore();

        // 页面引用
        private PageFastConfig pageFast;
        private PageDetailedConfig pageDetailed;
        private PageSettings pageSettings;
        private PageAbout pageAbout;

        // 底部控件
        private Button btnStart, btnStop;
        private Label lblStatus;
        private NotifyIcon trayIcon;

        public MainForm()
        {
            InitializeWindow();
            BuildUI();
        }

        private void InitializeWindow()
        {
            this.Text = "Neko Frp Launcher";
            this.Size = new Size(450, 540);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;

            // 【关键修改 1】必须开启图标显示，否则任务栏无法显示自定义图标
            this.ShowIcon = true;

            // 【关键修改 2】加载 icons 文件夹下的 logo.ico
            LoadAppIcon();

            trayIcon = new NotifyIcon
            {
                Icon = this.Icon, // 托盘图标同步使用主图标
                Text = "Neko Frp Launcher",
                Visible = false
            };
            trayIcon.MouseDoubleClick += (s, e) => { this.Show(); this.WindowState = FormWindowState.Normal; trayIcon.Visible = false; };
        }

        private void LoadAppIcon()
        {
            try
            {
                // 资源名称规则：命名空间.文件夹.文件名
                // 请确保 icons 文件夹里有 logo.ico，且属性为“嵌入的资源”
                string resourceName = "NekoFrpLauncher.icons.logo.ico";
                var assembly = Assembly.GetExecutingAssembly();

                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        // 直接加载 .ico 文件，它会自动包含多种尺寸(16x16, 32x32等)
                        // 这样任务栏(32px)和标题栏(16px)都会很清晰
                        this.Icon = new Icon(stream);
                    }
                    else
                    {
                        // 没找到资源时的保底
                        this.Icon = SystemIcons.Application;
                    }
                }
            }
            catch
            {
                this.Icon = SystemIcons.Application;
            }
        }

        private void BuildUI()
        {
            // 底部功能区
            FlowLayoutPanel pnlBottom = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 65,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(15, 5, 5, 5),
                FlowDirection = FlowDirection.LeftToRight
            };

            btnStart = UIBuilder.CreateMainButton("🚀 启动 FRP", 110, 40);
            btnStop = UIBuilder.CreateMainButton("🛑 停止", 90, 40);
            btnStop.Enabled = false;

            Panel pnlStatusWrap = new Panel { Size = new Size(150, 40), Margin = new Padding(10, 10, 0, 0) };
            lblStatus = UIBuilder.CreateLabel("状态: 未运行", 0, 12);
            lblStatus.ForeColor = Color.Gray;
            pnlStatusWrap.Controls.Add(lblStatus);

            btnStart.Click += Global_BtnStart_Click;
            btnStop.Click += Global_BtnStop_Click;

            pnlBottom.Controls.Add(btnStart);
            pnlBottom.Controls.Add(btnStop);
            pnlBottom.Controls.Add(pnlStatusWrap);

            this.Controls.Add(pnlBottom);

            // 内容区
            TabControl tabContent = new TabControl { Dock = DockStyle.Fill };

            pageFast = new PageFastConfig(_core);
            pageDetailed = new PageDetailedConfig(_core);
            pageSettings = new PageSettings(_core);
            pageAbout = new PageAbout();

            tabContent.TabPages.Add(pageFast);
            tabContent.TabPages.Add(pageDetailed);
            tabContent.TabPages.Add(pageSettings);
            tabContent.TabPages.Add(pageAbout);

            this.Controls.Add(tabContent);
            tabContent.BringToFront();
            pnlBottom.SendToBack();
        }

        private void Global_BtnStart_Click(object sender, EventArgs e)
        {
            try
            {
                _core.StartProcess();
                UpdateStatus(true);
                if (pageSettings.ChkMinimizeTray.Checked)
                {
                    this.Hide();
                    trayIcon.Visible = true;
                    trayIcon.ShowBalloonTip(3000, "Neko Frp", "FRP 已在后台启动", ToolTipIcon.Info);
                }
            }
            catch (Exception ex) { MessageBox.Show("启动失败: " + ex.Message); }
        }

        private void Global_BtnStop_Click(object sender, EventArgs e)
        {
            _core.StopProcess();
            UpdateStatus(false);
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
