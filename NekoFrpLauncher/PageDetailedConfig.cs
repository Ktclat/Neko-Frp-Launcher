using System;
using System.Drawing;
using System.Windows.Forms;

namespace NekoFrpLauncher
{
    public class PageDetailedConfig : TabPage
    {
        private FrpCore _core;
        private RichTextBox txtEditor;

        public PageDetailedConfig(FrpCore core)
        {
            _core = core;
            this.Text = "📝 详细配置";
            this.BackColor = Color.White;

            // --- 底部按钮面板 (流式布局) ---
            FlowLayoutPanel pnlAction = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Color.WhiteSmoke,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 5, 10, 5)
            };

            Button btnApply = UIBuilder.CreateActionButton("✔ 应用");
            Button btnCancel = UIBuilder.CreateActionButton("✖ 取消");

            btnApply.Click += (s, e) =>
            {
                try { _core.SaveConfig(txtEditor.Text); MessageBox.Show("详细配置已保存！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information); }
                catch (Exception ex) { MessageBox.Show("保存失败: " + ex.Message); }
            };

            btnCancel.Click += (s, e) =>
            {
                txtEditor.Text = _core.LoadConfig();
                MessageBox.Show("已重置。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            pnlAction.Controls.Add(btnApply);
            pnlAction.Controls.Add(btnCancel);
            this.Controls.Add(pnlAction);

            // --- 编辑器 ---
            Panel pnlEditorWrap = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            txtEditor = UIBuilder.CreateEditor();
            pnlEditorWrap.Controls.Add(txtEditor);

            this.Controls.Add(pnlEditorWrap);
            pnlEditorWrap.BringToFront();

            this.Enter += (s, e) => txtEditor.Text = _core.LoadConfig();
        }
    }
}