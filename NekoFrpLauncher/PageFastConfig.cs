using System;
using System.Drawing;
using System.Windows.Forms;

namespace NekoFrpLauncher
{
    public class PageFastConfig : TabPage
    {
        private FrpCore _core;

        // 控件定义
        private TextBox txtServerIp, txtServerPort, txtToken, txtLocalPort, txtRemotePort;
        private ComboBox cmbProtocol;
        private CheckBox chkShowSecret;

        // 用于存储真实值的后台变量
        private string _realIp = "";
        private string _realPort = "";
        private string _realToken = "";

        public PageFastConfig(FrpCore core)
        {
            _core = core;
            this.Text = "⚡ 快速配置";
            this.BackColor = Color.White;

            BuildUI();
            ReloadData();

            // 关键：每次进入页面时强制刷新数据，确保同步详细配置中的更改
            this.Enter += (s, e) => ReloadData();
        }

        private void BuildUI()
        {
            // --- 底部按钮面板 ---
            FlowLayoutPanel pnlAction = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.WhiteSmoke,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 8, 15, 5)
            };

            Button btnApply = UIBuilder.CreateActionButton("✔ 应用");
            Button btnCancel = UIBuilder.CreateActionButton("✖ 取消");

            btnApply.Click += BtnApply_Click;
            btnCancel.Click += BtnCancel_Click;

            pnlAction.Controls.Add(btnApply);
            pnlAction.Controls.Add(btnCancel);
            this.Controls.Add(pnlAction);

            // --- 内容布局 ---
            int lblX = 25;
            int txtX = 135;
            int txtW = 230;
            int y = 20;

            this.Controls.Add(UIBuilder.CreateLabel("--- 服务器配置 ---", lblX, y, true));
            y += 35;

            this.Controls.Add(UIBuilder.CreateLabel("服务器 IP:", lblX, y));
            txtServerIp = UIBuilder.CreateTextBox("", txtX, y, txtW);
            this.Controls.Add(txtServerIp);

            y += 40;
            this.Controls.Add(UIBuilder.CreateLabel("服务器端口:", lblX, y));
            txtServerPort = UIBuilder.CreateTextBox("", txtX, y, txtW);
            this.Controls.Add(txtServerPort);

            y += 40;
            this.Controls.Add(UIBuilder.CreateLabel("连接密码:", lblX, y));
            txtToken = UIBuilder.CreateTextBox("", txtX, y, txtW);
            this.Controls.Add(txtToken);

            chkShowSecret = UIBuilder.CreateCheckBox("👁️ 显示敏感信息", txtX, y + 28);
            chkShowSecret.CheckedChanged += (s, e) => ToggleSecret();
            this.Controls.Add(chkShowSecret);

            y += 60;
            this.Controls.Add(UIBuilder.CreateLabel("--- 游戏配置 ---", lblX, y, true));
            y += 35;

            this.Controls.Add(UIBuilder.CreateLabel("协议类型:", lblX, y));
            cmbProtocol = UIBuilder.CreateComboBox(new string[] { "udp", "tcp" }, txtX, y, txtW);
            this.Controls.Add(cmbProtocol);

            y += 40;
            this.Controls.Add(UIBuilder.CreateLabel("本地游戏端口:", lblX, y));
            txtLocalPort = UIBuilder.CreateTextBox("", txtX, y, txtW);
            this.Controls.Add(txtLocalPort);

            y += 40;
            this.Controls.Add(UIBuilder.CreateLabel("远程映射端口:", lblX, y));
            txtRemotePort = UIBuilder.CreateTextBox("", txtX, y, txtW);
            this.Controls.Add(txtRemotePort);
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            // 智能提取：如果是星号则取原值，否则取输入值
            string finalIp = (txtServerIp.Text == "******") ? _realIp : txtServerIp.Text;
            string finalPort = (txtServerPort.Text == "******") ? _realPort : txtServerPort.Text;
            string finalToken = (txtToken.Text == "******") ? _realToken : txtToken.Text;

            string protocol = cmbProtocol.SelectedItem?.ToString() ?? "udp";

            // 生成并保存配置
            string newConfig = _core.GenerateToml(
                finalIp,
                finalPort,
                finalToken,
                txtLocalPort.Text,
                txtRemotePort.Text,
                protocol
            );

            try
            {
                _core.SaveConfig(newConfig);
                MessageBox.Show("快速配置已保存！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ReloadData();
            }
            catch (Exception ex) { MessageBox.Show("保存失败: " + ex.Message); }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            ReloadData();
            MessageBox.Show("已取消修改。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ReloadData()
        {
            string content = _core.LoadConfig(); // 此处会自动调用 FrpCore 的 TryConvert

            // 使用增强后的 ExtractValue 提取数据，兼容新旧格式
            _realIp = _core.ExtractValue(content, "serverAddr");
            _realPort = _core.ExtractValue(content, "serverPort");
            _realToken = _core.ExtractValue(content, "auth.token");

            txtLocalPort.Text = _core.ExtractValue(content, "localPort");
            txtRemotePort.Text = _core.ExtractValue(content, "remotePort");

            string type = _core.ExtractValue(content, "type").ToLower();
            cmbProtocol.SelectedItem = (type == "tcp") ? "tcp" : "udp";

            ApplySecretState();
        }

        private void ToggleSecret()
        {
            // 切换前如果不是星号，先同步当前输入到后台变量
            if (!chkShowSecret.Checked)
            {
                if (txtServerIp.Text != "******") _realIp = txtServerIp.Text;
                if (txtServerPort.Text != "******") _realPort = txtServerPort.Text;
                if (txtToken.Text != "******") _realToken = txtToken.Text;
            }
            ApplySecretState();
        }

        private void ApplySecretState()
        {
            if (chkShowSecret.Checked)
            {
                txtServerIp.Text = _realIp;
                txtServerPort.Text = _realPort;
                txtToken.Text = _realToken;
            }
            else
            {
                txtServerIp.Text = "******";
                txtServerPort.Text = "******";
                txtToken.Text = "******";
            }
        }
    }
}
