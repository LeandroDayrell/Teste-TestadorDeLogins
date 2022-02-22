using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestadorDeLogin
{
    public partial class FormPrincipal : Form
    {
        public ChromiumWebBrowser chromeBrowser;
        public FormPrincipal()
        {
            InitializeComponent();
        }

        private async void FormPrincipal_Load(object sender, EventArgs e)
        {
            var browser = InitializeChromium();

            await Fluxos.FluxoTestaLogin.LogarSite(browser);
        }

        private ChromiumWebBrowser InitializeChromium()
        {
            CefSettings configuracoes = new CefSettings();

            Cef.Initialize(configuracoes);
            var browser = new ChromiumWebBrowser(ConfigurationManager.AppSettings["SiteLogin"]);

            this.Controls.Add(browser);

            browser.Dock = DockStyle.Fill;

            return browser;
        }

        private void FormPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
        }
    }
}
