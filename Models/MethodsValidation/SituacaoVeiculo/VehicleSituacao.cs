using CoreBot.Fields;
using CoreBot.Models.MethodsValidation.License;
using CoreBot.Services.WSDLService.ConsultaSituacaoVeiculo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models
{
    public class VehicleSituacao
    {
        public async Task<wsDetranChatBot.realizarSituacaoVeiculoResult> ObterSituacaoVeiculo(string plataforma, string renavam, string codSeguranca)
        {
            try
            {
                ConsultaSituacaoVeic obter = new ConsultaSituacaoVeic();
                var result = await obter.obterSituacaoVeiculo(plataforma, Convert.ToDouble(renavam), Convert.ToDouble(codSeguranca));
                return result;
            }
            catch (Exception err)
            {
                return null;
            }
        }
    }
}
