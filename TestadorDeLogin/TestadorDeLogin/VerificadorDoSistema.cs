using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestadorDeLogin
{
    internal class VerificadorDoSistema
    {
        public static async Task<bool> AguardarCarregamentoTela(ChromiumWebBrowser browser, int secMaxEspera = 30, int msIntervalo = 100)
        {
            var temporizador = new Stopwatch();
            temporizador.Start();
            while (browser.IsLoading)
            {
                if (temporizador.Elapsed.TotalSeconds > secMaxEspera)
                {
                    return false;
                }
                await Task.Delay(msIntervalo);
            }
            temporizador.Stop();

            return true;
        }

        public static async Task<bool> AguardarCarregamentoTela(ChromiumWebBrowser browser, string enderecoUrl, int secMaxEspera = 30, int msIntervalo = 100)
        {
            var temporizador = new Stopwatch();
            temporizador.Start();
            while (browser.IsLoading || !browser.Address.Contains(enderecoUrl))
            {
                if (temporizador.Elapsed.TotalSeconds > secMaxEspera)
                {
                    return false;
                }
                await Task.Delay(msIntervalo);
            }
            temporizador.Stop();

            return true;
        }

        public static async Task<string> ObterQualUrlFoiCarregada(ChromiumWebBrowser browser, string enderecoUrlPrincipal, string enderecoUrlSecundaria, int secMaxEspera = 30, int msIntervalo = 100)
        {
            var temporizador = new Stopwatch();
            temporizador.Start();
            while (browser.IsLoading || !browser.Address.Contains(enderecoUrlPrincipal) || browser.Address.Contains(enderecoUrlSecundaria))
            {
                if (temporizador.Elapsed.TotalSeconds > secMaxEspera)
                {
                    return string.Empty;
                }
                else if (browser.Address.Contains(enderecoUrlSecundaria) && !browser.IsLoading)
                {
                    return enderecoUrlSecundaria;
                }
                await Task.Delay(msIntervalo);
            }
            temporizador.Stop();

            return enderecoUrlPrincipal;
        }

        public static async Task<bool> AguardarCarregamentoElementoHTML(ChromiumWebBrowser browser, string idElemento, int secMaxEspera = 30, int msIntervalo = 100)
        {
            var temporizador = new Stopwatch();
            temporizador.Start();



            var carregouElemento = false;
            while (carregouElemento == false)
            {
                if (temporizador.Elapsed.TotalSeconds > secMaxEspera)
                {
                    return false;
                }

                await Task.Delay(msIntervalo);

                //Reccomended to use an anon closure
                var script = "(function() \n" +
                    "{ \n" +
                        $"var elemento = document.getElementById('{idElemento}'); \n" +
                        "if (elemento != null) { \n" +
                            "return true; \n" +
                        "} \n" +
                        "else { \n" +
                            "return false; \n" +
                        "} \n" +
                    "})();";

                await browser.EvaluateScriptAsync(script).ContinueWith(x =>
                {
                    var response = x.Result;

                    if (response.Success && response.Result != null)
                    {
                        if (response.Result.ToString() == "True")
                        {
                            carregouElemento = true;
                        }
                    }
                });
            }
            temporizador.Stop();

            return carregouElemento;
        }

        /// ////////////////////// AQUI É O CERTO LEANDRO /////////////////////////////

        public static async Task<bool> AguardarCarregamentoElementoHTML(ChromiumWebBrowser browser, string tag, string innerText, int secMaxEspera = 30, int msIntervalo = 100)
        {
            var temporizador = new Stopwatch();
            temporizador.Start();

            var carregouElemento = false;
            while (carregouElemento == false)
            {
                if (temporizador.Elapsed.TotalSeconds > secMaxEspera)
                {
                    return false;
                }

                while (!browser.CanExecuteJavascriptInMainFrame)
                {
                    await Task.Delay(msIntervalo);
                }

                await Task.Delay(500);

                //CanExecuteJavascriptInMainFrame

                //Reccomended to use an anon closure
                var script = "(function() \n" +
                    "{ \n" +
                        $"var elemento = document.getElementsByTagName('{tag}'); \n" +
                        "if (elemento != undefined) { \n" +
                            "for (var i = 0; i < elemento.length; i++) { \n" +
                                $"if (elemento[i].innerText == '{innerText}')" + "{ \n" +
                                    "return true; \n" +
                                "} \n" +
                            "} \n" +
                        "} \n" +
                        "return false; \n" +
                    "})();";

                await browser.EvaluateScriptAsync(script).ContinueWith(x =>
                {
                    var response = x.Result;

                    if (response.Success && response.Result != null)
                    {
                        if (response.Result.ToString() == "True")
                        {
                            carregouElemento = true;
                        }
                    }
                });
            }
            temporizador.Stop();

            return carregouElemento;
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static async Task<string> ObterValueDoInputPorId(ChromiumWebBrowser browser, string idElemento)
        {
            //Reccomended to use an anon closure
            var script = "(function() \n" +
                "{ \n" +
                    $"var elemento = document.getElementById('{idElemento}'); \n" +
                    "if (elemento != undefined) { \n" +
                        "return elemento.value; \n" +
                    "} \n" +
                    "return ''; \n" +
                "})();";

            var elementValue = string.Empty;
            await browser.EvaluateScriptAsync(script).ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    elementValue = response.Result.ToString();
                }
            });

            return elementValue;
        }

        public static async Task<string> ObterValueDoInputPorNome(ChromiumWebBrowser browser, string nomeElemento, int index)
        {
            //Reccomended to use an anon closure
            var script = "(function() \n" +
                "{ \n" +
                    $"var elemento = document.getElementsByName('{nomeElemento}')[{index}]; \n" +
                    "if (elemento != undefined) { \n" +
                        "return elemento.value; \n" +
                    "} \n" +
                    "return ''; \n" +
                "})();";

            var elementValue = string.Empty;
            await browser.EvaluateScriptAsync(script).ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    elementValue = response.Result.ToString();
                }
            });

            return elementValue;
        }

        public static async Task<string> ObterRadioMarcadoPorId(ChromiumWebBrowser browser, string idElemento)
        {
            //Reccomended to use an anon closure
            var script = "(function() \n" +
                "{ \n" +
                    $"var elemento = document.getElementById('{idElemento}'); \n" +
                    "if (elemento != undefined) { \n" +
                        "var divEscolhida = elemento.getElementsByClassName('radio-icon selected')[0]; \n" +
                        "return divEscolhida.innerText; \n" +
                    "} \n" +
                    "return ''; \n" +
                "})();";

            var elementValue = string.Empty;
            await browser.EvaluateScriptAsync(script).ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    elementValue = response.Result.ToString();
                }
            });

            return elementValue;
        }

        public static async Task<string> ObterTextoDoElementoPorId(ChromiumWebBrowser browser, string idElemento)
        {
            //Reccomended to use an anon closure
            var script = "(function() \n" +
                "{ \n" +
                    $"var elemento = document.getElementById('{idElemento}'); \n" +
                    "if (elemento != undefined) { \n" +
                        "return elemento.innerText.replace(/\\n×/g, '').replace('×', ''); \n" +
                    "} \n" +
                    "return ''; \n" +
                "})();";

            var elementValue = string.Empty;
            await browser.EvaluateScriptAsync(script).ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    elementValue = response.Result.ToString();
                }
            });

            return elementValue;
        }

        public static async Task<string> ObterTextoDoElementoPorNameEClassName(ChromiumWebBrowser browser, string elementName, string className)
        {
            //Reccomended to use an anon closure
            var script = "(function() \n" +
                "{ \n" +
                    $"var divAlvo = document.getElementsByName('{elementName}')[0].parentElement; \n" +
                    "if (elemento != undefined) { \n" +
                        $"var spanMarcado = divAlvo.getElementsByClassName('{className}')[0]; \n" +
                        "return spanMarcado.innerText; \n" +
                    "} \n" +
                    "return ''; \n" +
                "})();";

            var elementValue = string.Empty;
            await browser.EvaluateScriptAsync(script).ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    elementValue = response.Result.ToString();
                }
            });

            return elementValue;
        }

        public static async Task<string> ObterTextoDoElementoPorClassName(ChromiumWebBrowser browser, string className)
        {
            //Reccomended to use an anon closure
            var script = "(function() \n" +
                "{ \n" +
                    $"var spanMarcado = document.getElementsByClassName('{className}')[0]; \n" +
                    "return spanMarcado.innerText; \n" +
                "})();";

            var elementValue = string.Empty;
            await browser.EvaluateScriptAsync(script).ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    elementValue = response.Result.ToString();
                }
            });

            return elementValue;
        }

        public static async Task<string> ObterIdDoElementoPorClassName(ChromiumWebBrowser browser, string className, int index, string tagName)
        {
            //Reccomended to use an anon closure
            var script = "(function() \n" +
                "{ \n" +
                    $"var id = document.getElementsByClassName('{className}')[{index}].getElementsByTagName('{tagName}')[0].getAttribute('id'); \n" +
                    "return id; \n" +
                "})();";

            var elementValue = string.Empty;
            await browser.EvaluateScriptAsync(script).ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    elementValue = response.Result.ToString();
                }
            });

            return elementValue;
        }

        public static async Task<string> ObterIdDoElementoPorName(ChromiumWebBrowser browser, string name, int index)
        {
            //Reccomended to use an anon closure
            var script = "(function() \n" +
                "{ \n" +
                    $"var id = document.getElementsByName('{name}')[{index}].getElementsByTagName('input')[0].getAttribute('id'); \n" +
                    "return id; \n" +
                "})();";

            var elementValue = string.Empty;
            await browser.EvaluateScriptAsync(script).ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    elementValue = response.Result.ToString();
                }
            });

            return elementValue;
        }

        public static async Task<bool> VerificarCampoMarcado(ChromiumWebBrowser browser, string titulo)
        {
            var marcouCampo = false;

            var script = "(function() \n" +
                "{ \n" +
                    "var spans = document.getElementsByTagName('span'); \n" +
                    "for (var i = 0; i < spans.length; i++) { \n" +
                        $"if (spans[i].innerText.indexOf('{titulo}') >= 0)" + "{ \n" +
                            $"if(spans[i].innerText == '{titulo}')" + "{ \n" +
                                "return true;  \n" +
                                "}\n" +
                            "}\n" +
                    "}\n" +
                    "return false;\n" +
                "})();";

            await browser.EvaluateScriptAsync(script).ContinueWith(x =>
            {
                var response = x.Result;

                if (response.Success && response.Result != null)
                {
                    if (response.Result.ToString() == "True")
                    {
                        marcouCampo = true;
                    }
                }
            });

            return marcouCampo;
        }
    }
}
