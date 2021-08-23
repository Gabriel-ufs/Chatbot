using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Services.WSDLService.validarRenovacaoHabilitacao
{
    public class validarRenovacaoHabilitacao
    {
        public async Task<wsDetranChatBot.validarRenovacaoHabilitacaoResult> ValidarRenovacaoHabilitacao (string plataforma, string cpf, string numIdentidade, string digito, string OrgaoEmissor, string UfEmissao, string TipoDocumentoArrecadacao)
        {
            wsDetranChatBot.wsChatbotSoapClient wsClientDev = Authentication.WsClient();
            wsDetranChatBot.autenticacao auth = Authentication.Auth();

            var soap = await wsClientDev.validarRenovacaoHabilitacaoAsync(auth, plataforma, cpf, numIdentidade, digito, OrgaoEmissor, UfEmissao, TipoDocumentoArrecadacao);

            var result = soap.validarRenovacaoHabilitacaoResult;

            return result;

        }
    }
}
