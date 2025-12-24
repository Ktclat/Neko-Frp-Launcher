using System.Drawing;
using System.Windows.Forms;

namespace NekoFrpLauncher
{
    public class PageSettings : TabPage
    {
        private FrpCore _core;
        public CheckBox ChkMinimizeTray { get; private set; }

        public PageSettings(FrpCore core)
        {
            _core = core;
            this.Text = "⚙️ 设置";
            this.BackColor = Color.White;

            CheckBox chkAutoStart = UIBuilder.CreateCheckBox("开机自动启动软件", 40, 40);
            chkAutoStart.CheckedChanged += (s, e) => _core.SetAutoStart(chkAutoStart.Checked);

            ChkMinimizeTray = UIBuilder.CreateCheckBox("启动后最小化到托盘", 40, 80);

            this.Controls.Add(chkAutoStart);
            this.Controls.Add(ChkMinimizeTray);
        }
    }
}
