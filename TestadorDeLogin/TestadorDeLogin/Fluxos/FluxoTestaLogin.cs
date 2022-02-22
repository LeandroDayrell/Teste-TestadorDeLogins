using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Net;


namespace TestadorDeLogin.Fluxos
{
    internal class FluxoTestaLogin
    {
        public static int Contador { get; set; }
        public static async Task LogarSite(ChromiumWebBrowser browser)
        {

            var path = ConfigurationManager.AppSettings["PastaDeOndePegaOsLogins"];
            var todosOsLogins = File.ReadAllLines(path);
            var email = String.Empty;
            var senha = String.Empty;


            
            foreach (var login in todosOsLogins) // carregamento de logins
            {
                browser.Load("");
                email = login.Split('	')[0];
                senha = login.Split('	')[1];

                // Aguarda a tela carregar e espera 2 segundos
                while (browser.IsLoading)
                {
                    await Task.Delay(2000);
                }

                do
                {
                    await TestarLogin(browser, email, senha);
                    Contador++;
                    
                    while (browser.IsLoading)
                    {
                        await Task.Delay(2000);
                    }

                    if (Contador > int.Parse(ConfigurationManager.AppSettings["NumeroTentativas"]))//NumeroTentativas
                    {
                        break;
                    }
                } while (!await VerificaSeLogou(browser));

                if (!await VerificaSeLogou(browser))
                {
                    SalvarLogins(email, senha);
                }

                Contador = 0;
                browser.Load(ConfigurationManager.AppSettings["SiteLogout"].ToString());
            }

            // Verifica quantas linhas existem na pasta salva pra informar no discord 
            var ArquivoDeSalvar = File.ReadLines(ConfigurationManager.AppSettings["PastaOndeVaiSalvar"]).Count();
            string mensagem = String.Format("**Mensagem Teste** \nExistem " + ArquivoDeSalvar + " logins com problema no client Alianca ");
            DiscordSendMessage(mensagem);
            Environment.Exit(0);
            // ADICIONAR AQUI O DISCORD
        }

        // Enviar mensagem para o Discord
        public static void DiscordSendMessage(string mensagemTeste)
        {
            WebClient wc = new WebClient();
            var url = ConfigurationManager.AppSettings["LinkDiscord"]; // Busca dentro do APPConfig o link do arquivo
            try
            {
                wc.UploadValues(url, new NameValueCollection
            {{
                        "content", mensagemTeste
            },});}
            catch (WebException ex)
            {
                Console.WriteLine(ex.ToString()); 
            }
        }

        // Salvar os logins no arquivo.
        public static void SalvarLogins(string email, string senha)
        {
            var caminhoDoArquivo = ConfigurationManager.AppSettings["PastaOndeVaiSalvar"];
            File.AppendAllText(caminhoDoArquivo, email + "," + senha + "\r\n");
            

        }
        // Verifica se logou e traz um retorno.
        public static async Task<bool> VerificaSeLogou(ChromiumWebBrowser browser)
        {
            bool resposta = false;
            while (!browser.CanExecuteJavascriptInMainFrame)
            {
                await Task.Delay(2000);
            }

            var script = "(function() \n" +
                  "{ \n" +
                      "var verificaLogou = document.getElementsByName('j_username')[0]; \n" +

                      "if (verificaLogou == undefined) \n" +
                          "return true; \n" +
                      "else \n" +
                          "return false; \n" +
                  "})();";

            await browser.EvaluateScriptAsync(script).ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    resposta = bool.Parse(response.Result.ToString());
                }
            });
            await CaixaDeDialog(browser);
            Contador++;
            return resposta;
        }

        // Tentativa de correcao de caixa de Dialog que aparece quando solicita token em algum login
        public static async Task CaixaDeDialog(ChromiumWebBrowser browser)
        {

            while (!browser.CanExecuteJavascriptInMainFrame)
            {
                await Task.Delay(200);
            }
            var script = "(function() \n" +
                "{ \n" +
                "alert('close'); \n" +
                "})";

            await browser.EvaluateScriptAsync(script).ContinueWith(x =>
            {
                var response = x.Result;
                if (response.Success && response.Result != null)
                {
                }
            });
        }


        // Testa o login e a senha no site
        public static async Task TestarLogin(ChromiumWebBrowser browser, string email, string senha)
        {
            if (await VerificadorDoSistema.AguardarCarregamentoElementoHTML(browser, "td", "Autenticação", 10))
            {
                await Task.Delay(1000);
                var script = "(function() \n" +
                   "{ \n" +
                       "var login = document.getElementsByName('j_username')[0]; \n" +
                       "var password = document.getElementsByName('j_password')[0]; \n" +

                       "var evtFocus = new Event('focus'); \n" +
                       "var evtBlur = new Event('blur'); \n" +

                       "login.dispatchEvent(evtFocus); \n" +
                       $"login.value = '{email}'; \n" +
                       "login.dispatchEvent(evtBlur); \n" +

                       "password.dispatchEvent(evtFocus); \n" +
                       $"password.value = '{senha}'; \n" +
                       "password.dispatchEvent(evtBlur); \n" +

                   "})();";

                await browser.EvaluateScriptAsync(script).ContinueWith(x =>
                {
                    var response = x.Result;

                    if (response.Success && response.Result != null)
                    {

                    }
                });

                await Task.Delay(1000);

                var scriptLogin = "(function() \n" +
                   "{ \n" +
                       "var evtFocus = new Event('focus'); \n" +

                       "var botoes = document.getElementsByName('button')[0]; \n" +
                       "botoes.focus(); \n" +
                       "botoes.click(); \n" +
                   "})();";

                await browser.EvaluateScriptAsync(scriptLogin).ContinueWith(x =>
                {
                    var response = x.Result;

                    if (response.Success && response.Result != null)
                    {

                    }
                });
            }
        }
    }
}
