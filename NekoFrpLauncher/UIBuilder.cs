using System;
using System.Drawing;
using System.Windows.Forms;

namespace NekoFrpLauncher
{
    public static class UIBuilder
    {
        public static Font MainFont = new Font("微软雅黑", 9F);
        public static Font BoldFont = new Font("微软雅黑", 9F, FontStyle.Bold);
        public static Font CodeFont = new Font("Consolas", 10F);

        // --- 基础控件 ---
        public static Label CreateLabel(string text, int x, int y, bool isBold = false)
        {
            return new Label { Text = text, Location = new Point(x, y + 4), AutoSize = true, Font = isBold ? BoldFont : MainFont };
        }

        public static TextBox CreateTextBox(string defaultVal, int x, int y, int width = 220)
        {
            return new TextBox { Text = defaultVal, Location = new Point(x, y), Width = width, Font = MainFont };
        }

        // 【新增】创建下拉框
        public static ComboBox CreateComboBox(string[] items, int x, int y, int width = 220)
        {
            ComboBox cb = new ComboBox
            {
                Location = new Point(x, y),
                Width = width,
                Font = MainFont,
                DropDownStyle = ComboBoxStyle.DropDownList // 禁止手动输入，只能选
            };
            cb.Items.AddRange(items);
            if (items.Length > 0) cb.SelectedIndex = 0; // 默认选中第一个
            return cb;
        }

        public static CheckBox CreateCheckBox(string text, int x, int y)
        {
            return new CheckBox { Text = text, Location = new Point(x, y), AutoSize = true, Font = MainFont };
        }

        public static RichTextBox CreateEditor()
        {
            return new RichTextBox { Dock = DockStyle.Fill, Font = CodeFont, BorderStyle = BorderStyle.None, BackColor = Color.WhiteSmoke };
        }

        public static PictureBox CreatePictureBox(string imagePath, int x, int y, int w, int h)
        {
            PictureBox pb = new PictureBox { Location = new Point(x, y), Size = new Size(w, h), SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Transparent };
            try { pb.Image = Image.FromFile(imagePath); } catch { pb.BorderStyle = BorderStyle.FixedSingle; }
            return pb;
        }

        // --- 底部主控按钮 ---
        public static Button CreateMainButton(string text, int w, int h)
        {
            return new Button
            {
                Text = text,
                Size = new Size(w, h),
                Font = MainFont,
                FlatStyle = FlatStyle.System,
                Margin = new Padding(3, 10, 10, 3)
            };
        }

        // --- 操作按钮 ---
        public static Button CreateActionButton(string text)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(80, 32),
                Font = MainFont,
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.Black,
                Margin = new Padding(5, 8, 5, 5)
            };
            btn.FlatAppearance.BorderColor = Color.LightGray;
            btn.FlatAppearance.BorderSize = 1;

            btn.MouseEnter += (s, e) => {
                btn.BackColor = Color.FromArgb(235, 245, 255);
                btn.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
                btn.ForeColor = Color.FromArgb(0, 120, 215);
            };

            btn.MouseLeave += (s, e) => {
                btn.BackColor = Color.White;
                btn.FlatAppearance.BorderColor = Color.LightGray;
                btn.ForeColor = Color.Black;
            };

            return btn;
        }
    }
}
