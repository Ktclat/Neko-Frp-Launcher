using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace NekoFrpLauncher
{
    public class PageAbout : TabPage
    {
        private PictureBox pbLogo;
        private Label lblName;
        private Label lblVer;
        private Label lblDesc;
        private LinkLabel lnkGithub;
        private Label lblCredits;
        private Label lblLicense;

        public PageAbout()
        {
            this.Text = "ℹ️ 关于";
            this.BackColor = Color.White;

            // 1. Logo 图片
            pbLogo = new PictureBox
            {
                Size = new Size(110, 110),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                string resName = "NekoFrpLauncher.icons.512x512.png";
                using (var stream = assembly.GetManifestResourceStream(resName))
                {
                    if (stream != null) pbLogo.Image = Image.FromStream(stream);
                }
            }
            catch { pbLogo.Visible = false; }

            // 2. 项目名称 (通常保留英文或中英双语)
            lblName = new Label
            {
                Text = "Neko Frp Launcher",
                AutoSize = true,
                Font = new Font("微软雅黑", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40)
            };

            // 3. 版本号
            lblVer = new Label
            {
                Text = "Version 1.0 (Release)",
                AutoSize = true,
                Font = new Font("微软雅黑", 9),
                ForeColor = Color.Gray
            };

            // 4. 简介 (已汉化)
            lblDesc = new Label
            {
                Text = "一款简单、高效、原生的 Windows FRP 启动器。\n专为极致性能打造，让联机更简单。",
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("微软雅黑", 10),
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            // 5. GitHub 链接
            lnkGithub = new LinkLabel
            {
                Text = "GitHub: Ktclat/Neko-Frp-Launcher",
                AutoSize = true,
                Font = new Font("Segoe UI", 10),
                LinkColor = Color.FromArgb(0, 102, 204),
                Cursor = Cursors.Hand
            };
            lnkGithub.Click += (s, e) => OpenUrl("https://github.com/Ktclat/Neko-Frp-Launcher");

            // 6. 底部致谢与协议 (已汉化)
            lblCredits = new Label
            {
                Text = "核心基于 fatedier/frp 项目开发",
                AutoSize = true,
                Font = new Font("微软雅黑", 8),
                ForeColor = Color.Silver
            };

            lblLicense = new Label
            {
                Text = "遵循 MIT 开源协议",
                AutoSize = true,
                Font = new Font("微软雅黑", 8),
                ForeColor = Color.Silver
            };

            // 添加控件
            this.Controls.Add(pbLogo);
            this.Controls.Add(lblName);
            this.Controls.Add(lblVer);
            this.Controls.Add(lblDesc);
            this.Controls.Add(lnkGithub);
            this.Controls.Add(lblCredits);
            this.Controls.Add(lblLicense);

            // 绑定布局事件
            this.SizeChanged += (s, e) => RecenterControls();
            RecenterControls();
        }

        private void OpenUrl(string url)
        {
            try { Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true }); }
            catch (Exception ex) { MessageBox.Show("无法打开链接: " + ex.Message); }
        }

        private void RecenterControls()
        {
            int centerX = this.ClientSize.Width / 2;
            int y = 30; // 顶部起始位置

            // --- 上半部分 ---
            if (pbLogo.Visible)
            {
                pbLogo.Location = new Point(centerX - (pbLogo.Width / 2), y);
                y += pbLogo.Height + 10;
            }
            else { y += 40; }

            lblName.Location = new Point(centerX - (lblName.Width / 2), y);
            y += lblName.Height + 2;

            lblVer.Location = new Point(centerX - (lblVer.Width / 2), y);
            y += lblVer.Height + 20;

            lblDesc.Location = new Point(centerX - (lblDesc.Width / 2), y);
            y += lblDesc.Height + 15;

            lnkGithub.Location = new Point(centerX - (lnkGithub.Width / 2), y);

            // --- 底部沉底 ---
            int bottomY = this.ClientSize.Height - 45;

            lblLicense.Location = new Point(centerX - (lblLicense.Width / 2), bottomY);
            bottomY -= 18;

            lblCredits.Location = new Point(centerX - (lblCredits.Width / 2), bottomY);
        }
    }
}
