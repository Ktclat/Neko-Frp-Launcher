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

        // 【新增】用于存储真实值的“替身变量”
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

            // 1. 服务器配置
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
            // 绑定事件：切换显示/隐藏状态
            chkShowSecret.CheckedChanged += (s, e) => ToggleSecret();
            this.Controls.Add(chkShowSecret);

            // 2. 游戏配置
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
            // 【核心逻辑】保存时的智能判断
            // 如果输入框里是 "******"，说明用户没改，我们取后台的真实值 (_real...)
            // 如果输入框里是别的内容，说明用户改了，我们取输入框的值
            string finalIp = (txtServerIp.Text == "******") ? _realIp : txtServerIp.Text;
            string finalPort = (txtServerPort.Text == "******") ? _realPort : txtServerPort.Text;
            string finalToken = (txtToken.Text == "******") ? _realToken : txtToken.Text;

            // 只有当用户修改了值，才更新后台的 _real 变量，确保同步
            if (txtServerIp.Text != "******") _realIp = txtServerIp.Text;
            if (txtServerPort.Text != "******") _realPort = txtServerPort.Text;
            if (txtToken.Text != "******") _realToken = txtToken.Text;

            // 获取协议
            string protocol = cmbProtocol.SelectedItem?.ToString() ?? "udp";

            // 生成配置
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
                MessageBox.Show("快速配置已保存并应用！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // 保存成功后，刷新一下数据显示（为了把刚才输入的新值变成******状态）
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
            string content = _core.LoadConfig();

            // 1. 先把读到的真实数据存进“后台变量”
            _realIp = _core.ExtractValue(content, "serverAddr");
            _realPort = _core.ExtractValue(content, "serverPort");
            _realToken = _core.ExtractValue(content, "auth.token");

            // 2. 加载不需要保密的数据
            txtLocalPort.Text = _core.ExtractValue(content, "localPort");
            txtRemotePort.Text = _core.ExtractValue(content, "remotePort");
            string type = _core.ExtractValue(content, "type").ToLower();
            if (type == "tcp") cmbProtocol.SelectedItem = "tcp";
            else cmbProtocol.SelectedItem = "udp";

            // 3. 根据当前的复选框状态，决定输入框显示什么
            ApplySecretState();
        }

        private void ToggleSecret()
        {
            // 切换时先保存当前输入框的内容（防止用户在明文模式下改了还没保存，一切换就丢了）
            if (chkShowSecret.Checked)
            {
                // 此时是从“隐藏”变“显示”：不需要保存，因为隐藏时是******
            }
            else
            {
                // 此时是从“显示”变“隐藏”：如果用户刚才改了IP，得存进 _realIp
                if (txtServerIp.Text != "******") _realIp = txtServerIp.Text;
                if (txtServerPort.Text != "******") _realPort = txtServerPort.Text;
                if (txtToken.Text != "******") _realToken = txtToken.Text;
            }

            ApplySecretState();
        }

        // 统一处理“显示/隐藏”的脏活累活
        private void ApplySecretState()
        {
            if (chkShowSecret.Checked)
            {
                // 明文模式：显示真实值
                txtServerIp.Text = _realIp;
                txtServerPort.Text = _realPort;
                txtToken.Text = _realToken;
            }
            else
            {
                // 保密模式：强制显示 6 个星号
                txtServerIp.Text = "******";
                txtServerPort.Text = "******";
                txtToken.Text = "******";
            }
        }
    }
}
