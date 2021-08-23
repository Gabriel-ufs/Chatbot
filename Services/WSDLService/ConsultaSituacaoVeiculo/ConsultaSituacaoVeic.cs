using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.BotBuilderSamples;

namespace CoreBot.Services.WSDLService.ConsultaSituacaoVeiculo
{
    public class ConsultaSituacaoVeic
    {

        public async Task<wsDetranChatBot.realizarSituacaoVeiculoResult> obterSituacaoVeiculo(
            string plataforma,
            double renavam,
            double codSeguranca
            )
        {
            wsDetranChatBot.wsChatbotSoapClient wsClientDev = Authentication.WsClient();
            wsDetranChatBot.autenticacao auth = Authentication.Auth();

            var soap = await wsClientDev.SituacaoVeiculoLicenciamentoAsync(
                auth,
                plataforma,
                renavam,
                codSeguranca
                );

            var result = soap.SituacaoVeiculoLicenciamentoResult;

            return result;
        }

    }
}
