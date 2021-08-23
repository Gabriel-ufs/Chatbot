using Microsoft.BotBuilderSamples;
using System.Threading.Tasks;

namespace CoreBot.Services.WSDLService.validarServicoLicenciamento
{
    public class ValidarServicoLicenciamento
    {
        /// <summary>
        /// Função responsável por chamar o método de validarServicoLicenciamento do WebService.
        /// </summary>
        /// <param name="renavam"></param>
        /// <param name="plataforma"></param>
        /// <param name="codSeguranca"></param>
        /// <param name="tipoDocumentoIn"></param>
        /// <param name="anoLicenciamentoIn"></param>
        /// <returns></returns>
        public async Task<wsDetranChatBot.validarServicoLicenciamentoResult> validarServicoLicenciamento(double renavam, string plataforma, double codSeguranca, string tipoDocumentoIn, double anoLicenciamentoIn)
        {
            wsDetranChatBot.wsChatbotSoapClient wsClient = Authentication.WsClient();
            wsDetranChatBot.autenticacao auth = Authentication.Auth();
             
            var soap = await wsClient.validarServicoLicenciamentoAsync(auth, plataforma, renavam, codSeguranca, tipoDocumentoIn, anoLicenciamentoIn);
            var result = soap.validarServicoLicenciamentoResult;

            return result;
        }
    }
}
