using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.BotBuilderSamples;

namespace CoreBot.Services.WSDLService.obterCalendarioLicenciamento
{
    public class ObterCalendarioLicenciamento
    {
        /// <summary>
        /// Função responsável por chamar o método de obterCalendarioLicenciamento do WebService.
        /// </summary>
        /// <param name="plataforma"></param>
        /// <param name="anoLicenciamento"></param>
        /// <param name="finalPlaca"></param>
        /// <returns></returns>

        public async Task<wsDetranChatBot.CalendarioLicenciamentoResult> obterCalendarioLicenciamento(
            string plataforma,
            int anoLicenciamento,
            int finalPlaca
            )
        {
            wsDetranChatBot.wsChatbotSoapClient wsClient = Authentication.WsClient();
            wsDetranChatBot.autenticacao auth = Authentication.Auth();

            var soap = await wsClient.obterCalendarioLicenciamentoAsync(
                auth,
                plataforma,
                anoLicenciamento,
                finalPlaca
                );

            var result = soap.obterCalendarioLicenciamentoResult;

            return result;
        }

    }
}