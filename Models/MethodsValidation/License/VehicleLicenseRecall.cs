using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models.MethodsValidation.License
{
    /// <summary>
    /// Classe responsável por verificar se veículo possui chamada de Recall.
    /// </summary>
    public class VehicleLicenseRecall
    {
        /// <summary>
        /// Função responsável por verificar se veículo possui chamada de Recall.
        /// </summary>
        /// <param name="recallCodigo">"0" ou "1"</param>
        /// <returns></returns>
        public static bool ValidationVehicleRecall(int recallCodigo)
        {
            if(recallCodigo != 0)
            {
                return true;
            }
            return false;
        }
    }
}
