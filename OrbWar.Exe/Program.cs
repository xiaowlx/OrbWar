using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace OrbWar.Exe
{
    /// <summary>
    /// 离线桌面外壳：用系统自带的 WebView2（Win11 预装）渲染内嵌的 index.html，
    /// 不依赖网络、不写任何外部文件（仅 WebView2 自身在临时目录建缓存）。
    /// </summary>
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GameForm());
        }
    }

    public sealed class GameForm : Form
    {
        private readonly WebView2 _web;

        public GameForm()
        {
            Text = "OrbWar — 球球领土战争";
            WindowState = FormWindowState.Maximized;
            BackColor = System.Drawing.Color.Black;
            DoubleBuffered = true;

            _web = new WebView2
            {
                Dock = DockStyle.Fill,
                // 首次加载前尺寸为 0 也无妨：WebView2 会在父容器尺寸确定后自适应
            };
            Controls.Add(_web);
            _web.BringToFront();

            Load += GameForm_Load;
        }

        private async void GameForm_Load(object? sender, EventArgs e)
        {
            try
            {
                // 使用系统预装的 WebView2 Evergreen 运行时；离线可用
                await _web.EnsureCoreWebView2Async(null);

                var settings = _web.CoreWebView2.Settings;
                settings.AreDevToolsEnabled = false;
                settings.IsStatusBarEnabled = false;
                settings.AreDefaultContextMenusEnabled = false;
                settings.IsZoomControlEnabled = false;

                string html = LoadEmbeddedHtml();
                _web.CoreWebView2.NavigateToString(html);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "无法初始化 WebView2 渲染组件。\n\n" +
                    "本程序依赖 Windows 11 预装的 WebView2 运行时。\n" +
                    "若未安装，请从微软官网安装“WebView2 Runtime (Evergreen)”。\n\n" +
                    "技术信息：\n" + ex.Message,
                    "OrbWar 启动失败",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Close();
            }
        }

        private static string LoadEmbeddedHtml()
        {
            var asm = Assembly.GetExecutingAssembly();
            const string resourceName = "OrbWar.Exe.game.html";
            using var stream = asm.GetManifestResourceStream(resourceName);
            if (stream == null)
                return "<!doctype html><meta charset=utf-8><body style='background:#000;color:#fff;font-family:sans-serif'><h1>game.html 未嵌入</h1><p>请重新构建 exe。</p></body>";

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
