using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models.MethodsValidation.License
{
    /// <summary>
    /// Classe responsável por verificar se contém isenção.
    /// </summary>
    public class VehicleLicenseExemption
    {
        /// <summary>
        /// Função responsável por verificar se contém isenção.
        /// </summary>
        /// <param name="temIsencaoIPVA">"S" ou "N"</param>
        /// <returns></returns>
        public static bool Exemption(string temIsencaoIPVA)
        {
            if(temIsencaoIPVA == "S")
            {
                return true;
            }
            return false;
        }
    }
}
