using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreBot.Models.MethodsValidation.License;
using CoreBot.Services.WSDLService.validarRenovacaoHabilitacao;
using CoreBot.Services.WSDLService.obterCalendarioLicenciamento;
using CoreBot.Services.WSDLService.consultarCEP;


namespace CoreBot.Models
{
    public class CnhRenovation
    {

        public async Task<wsDetranChatBot.ConsultaCEP> obterCEP(string plataforma, int cep)
        {
            try
            {
                ConsultaCEP obter = new ConsultaCEP();
                var result = await obter.ConsultarCEP(plataforma, cep);
                return result;
            }

            catch (Exception err)
            {
                return null;
            }
        }


        public async Task<wsDetranChatBot.validarRenovacaoHabilitacaoResult> obterRenovacao(string plataforma, string cpf, string numIdentidade, string digito, string OrgaoEmissor, string UfEmissao, string TipoDocumentoArrecadacao)
        {
            try
            {
                validarRenovacaoHabilitacao obter = new validarRenovacaoHabilitacao();
                var result = await obter.ValidarRenovacaoHabilitacao(plataforma, cpf, numIdentidade, digito, OrgaoEmissor, UfEmissao, TipoDocumentoArrecadacao);
                return result;
            }
            catch (Exception err)
            {
                return null;
            }

        }

    }
}
