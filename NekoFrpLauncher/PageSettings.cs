using System.Drawing;
using System.Windows.Forms;

namespace NekoFrpLauncher
{
    public class PageSettings : TabPage
    {
        private FrpCore _core;

        public CheckBox ChkMinimizeTray { get; private set; } // 公开给主程序读取

        public PageSettings(FrpCore core)
        {
            _core = core;
            this.Text = "⚙️ 设置";
            this.BackColor = Color.White;

            CheckBox chkAutoStart = UIBuilder.CreateCheckBox("开机自动启动软件", 40, 40);
            chkAutoStart.CheckedChanged += (s, e) => _core.SetAutoStart(chkAutoStart.Checked);

            ChkMinimizeTray = UIBuilder.CreateCheckBox("启动后最小化到托盘", 40, 80);

            Label lblTheme = UIBuilder.CreateLabel("主题颜色: 跟随系统 (默认浅色)", 40, 120);
            lblTheme.ForeColor = Color.Gray;

            this.Controls.Add(chkAutoStart);
            this.Controls.Add(ChkMinimizeTray);
            this.Controls.Add(lblTheme);
        }
    }
}