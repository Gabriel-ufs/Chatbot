using CoreBot.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Services.WSDLService.obterEmissaoCRLV
{
    
    public class ObterEmissaoCRLV
    {
        /// <summary>
        /// Função responsável por chamar o método de obterEmissaoCRLV do WebService.
        /// </summary>
        /// <param name="placa"></param>
        /// <param name="codSeguranca"></param>
        /// <param name="plataforma"></param>
        /// <returns></returns>
        public async Task<wsDetranChatBot.obterEmissaoCrlvResult> obterEmissaoCRLV(string placa, double codSeguranca, string plataforma)
        {
            wsDetranChatBot.wsChatbotSoapClient wsClient = Authentication.WsClient();
            wsDetranChatBot.autenticacao auth = Authentication.Auth();

            var soap = await wsClient.obterEmissaoCrlvAsync(auth, plataforma, placa, codSeguranca);
            var result = soap.obterEmissaoCrlvResult;

            return soap.obterEmissaoCrlvResult;
        }

        /// <summary>
        /// Função responsável por chamar o método de obterEmissaoCRLV do WebService.
        /// </summary>
        /// <param name="placa"></param>
        /// <param name="plataforma"></param>
        /// <returns></returns>
        public async Task<wsDetranChatBot.obterEmissaoCrlvResult> obterEmissaoCRLV(string placa, string plataforma)
        {
            wsDetranChatBot.wsChatbotSoapClient wsClient = Authentication.WsClient();
            wsDetranChatBot.autenticacao auth = Authentication.Auth();

            var soap = await wsClient.obterEmissaoCrlvAsync(auth, plataforma, placa, 0);
            var result = soap.obterEmissaoCrlvResult;

            return soap.obterEmissaoCrlvResult;
        }

        /// <summary>
        /// Função responsável por chamar o método de obterEmissaoCRLV do WebService.
        /// </summary>
        /// <param name="codSeguranca"></param>
        /// <param name="plataforma"></param>
        /// <returns></returns>
        public async Task<wsDetranChatBot.obterEmissaoCrlvResult> obterEmissaoCRLV(double codSeguranca, string plataforma)
        {
            wsDetranChatBot.wsChatbotSoapClient wsClient = Authentication.WsClient();
            wsDetranChatBot.autenticacao auth = Authentication.Auth();

            var soap = await wsClient.obterEmissaoCrlvAsync(auth, plataforma, "", codSeguranca);
            var result = soap.obterEmissaoCrlvResult;

            return soap.obterEmissaoCrlvResult;
        }
    }
}
