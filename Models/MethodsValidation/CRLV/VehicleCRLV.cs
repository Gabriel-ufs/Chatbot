using CoreBot.Fields;
using CoreBot.Models.MethodsValidation.License;
using CoreBot.Services.WSDLService.obterEmissaoCRLV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models.Methods
{
    /// <summary>
    /// Classe responsável por conter funções auxiliares aos diálogos de CRLV.
    /// </summary>
    public class VehicleCRLV
    {
        /// <summary>
        /// Função responsável por chamar o método de obterEmissãoCRLV do WebService através do Código de Segurança.
        /// </summary>
        /// <param name="SecureCode"></param>
        /// <param name="plataforma"></param>
        /// <returns></returns>
        public async Task<wsDetranChatBot.obterEmissaoCrlvResult> ValidationSecureCode(string SecureCode, string plataforma)
        {
            try
            {
                ObterEmissaoCRLV obter = new ObterEmissaoCRLV();
                var result = await obter.obterEmissaoCRLV(Convert.ToDouble(SecureCode), plataforma);
                return result;
            }
            catch (Exception err)
            {
                return null;
            }
        }

        /// <summary>
        /// Função responsável por chamar o método de obterEmissãoCRLV do WebService através da placa.
        /// </summary>
        /// <param name="Placa"></param>
        /// <param name="plataforma"></param>
        /// <returns></returns>
        public async Task<wsDetranChatBot.obterEmissaoCrlvResult> ValidationPlaca(string Placa, string plataforma)
        {
            try
            {
                ObterEmissaoCRLV obter = new ObterEmissaoCRLV();
                var result = await obter.obterEmissaoCRLV(Placa, plataforma);
                return result;
            }
            catch (Exception err)
            {
                return null;
            }
        }

        // <summary>
        /// OBJETIVO: Verificar se o veículo possui código de segurança.
        /// </summary>
        /// <returns>True ou False, a depender se o código de segurança em CRLVDialogDetails é maior que 0.</returns>
        public static bool ExistSecureCodePlaca(string codSegurancaOut)
        {
            if (Convert.ToDouble(codSegurancaOut) > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Função responsável por verificar se o código de segurança possui formato válido.
        /// </summary>
        /// <param name="SecureCode"></param>
        /// <returns></returns>
        public bool ValidationString(string SecureCode)
        {
            if (SecureCode.Length > 0 && Format.Input.ValidationFormat.IsNumber(SecureCode) == true) return true;
            return false;
        }

        /// <summary>
        /// Função responsável por verificar se a placa possui formato válido.
        /// </summary>
        /// <param name="placa"></param>
        /// <returns></returns>
        public bool ValidationStringPlaca(string placa)
        {
            if (placa != "") return true;
            return false;
        }
    }
}
