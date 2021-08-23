using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Services.WSDLService.efetuarServicoLicenciamento
{
    public class EfetuarServicoLicenciamento
    {

        /// <summary>
        /// Função responsável por chamar o método de efetuarServicoLicenciamento do WebService.
        /// </summary>
        /// <param name="plataforma"></param>
        /// <param name="renavam"></param>
        /// <param name="codSeguranca"></param>
        /// <param name="restricao"></param>
        /// <param name="anoExercicioLicenciamento"></param>
        /// <param name="tipoAutorizacaoRNTRC"></param>
        /// <param name="nroAutorizacaoRNTRC"></param>
        /// <param name="dataValidadeRNTRC"></param>
        /// <param name="isencaoIPVA"></param>
        /// <param name="tipoDocumentoIn"></param>
        /// <returns>Result com todas as variáveis preenchidas.</returns>
        public static async Task<wsDetranChatBot.efetuarServicoLicenciamentoResult> efetuarServicoLicenciamento(
            string plataforma,
            double renavam, 
            double codSeguranca, 
            string restricao, 
            double anoExercicioLicenciamento, 
            string tipoAutorizacaoRNTRC, 
            double nroAutorizacaoRNTRC, 
            string dataValidadeRNTRC, 
            string isencaoIPVA, 
            string tipoDocumentoIn
            )
        {
            wsDetranChatBot.wsChatbotSoapClient wsClient = Authentication.WsClient();
            wsDetranChatBot.autenticacao auth = Authentication.Auth();

            var soap = await wsClient.efetuarServicoLicenciamentoAsync(
                auth, 
                plataforma,
                renavam, 
                codSeguranca, 
                restricao, 
                anoExercicioLicenciamento, 
                tipoAutorizacaoRNTRC, 
                nroAutorizacaoRNTRC, 
                dataValidadeRNTRC, 
                isencaoIPVA, 
                tipoDocumentoIn
                );

            var result = soap.efetuarServicoLicenciamentoResult;

            return soap.efetuarServicoLicenciamentoResult;
        }
    }
}
