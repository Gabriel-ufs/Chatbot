using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Services.WSDLService.consultarCEP
{
    public class ConsultaCEP
    {
        public async Task<wsDetranChatBot.ConsultaCEP> ConsultarCEP (string plataforma, decimal cep)
        {
            try
            {
                wsDetranChatBot.wsChatbotSoapClient wsClientDev = Authentication.WsClient();
                wsDetranChatBot.autenticacao auth = Authentication.Auth();

                var soap = await wsClientDev.consultarCEPAsync(auth, plataforma, cep);
                return soap.consultarCEPResult;

            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }

    
}
