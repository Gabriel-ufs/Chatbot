using CoreBot.Fields;
using CoreBot.Models.MethodsValidation.License;
using CoreBot.Services.WSDLService.obterCalendarioLicenciamento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CoreBot.Models
{
    /// <summary>
    /// Esta classe contém o metodos básicos de validação para calendario.
    /// </summary>
    public class VehicleCalendar
    {
        public async Task<wsDetranChatBot.CalendarioLicenciamentoResult> ObtainCalendarLicenciamento(string plataforma, int finalPlaca, int anoLicenciamento)
        {
            try
            {
                ObterCalendarioLicenciamento obter = new ObterCalendarioLicenciamento();
                var result = await obter.obterCalendarioLicenciamento(plataforma, Convert.ToInt32(anoLicenciamento), Convert.ToInt32(finalPlaca));
                return result;
            }
            catch (Exception err)
            {
                return null;
            }
        }

    }
}