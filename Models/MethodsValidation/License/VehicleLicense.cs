using CoreBot.Fields;
using CoreBot.Models.MethodsValidation.License;
using CoreBot.Services.WSDLService.validarServicoLicenciamento;
using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreBot.Models
{
    /// <summary>
    /// Esta classe contém o metodos básicos de validação para lincenciamento.
    /// </summary>
    public class VehicleLicense
    {
        /// <summary>
        /// OBJETIVO: Função responsável por validar formato de entrada.
        /// </summary>
        /// <param name="SecureCode"></param>
        /// <returns></returns>
        /// 
        public bool ValidationString(string SecureCode)
        {
            if (SecureCode.Length > 0 && Format.Input.ValidationFormat.IsNumber(SecureCode) == true) return true;
            return false;
        }

        /// <summary>
        /// Função responsável por chamar validarServicoLicenciamento no WebService. Ano "0".
        /// </summary>
        /// <param name="SecureCode">Código de Segurança</param>
        /// <param name="tipoDocumentoIn">Tipo de documento "D" ou "F"</param>
        /// <param name="plataforma">Tipo da plataforma</param>
        /// <returns></returns>
        public async Task<wsDetranChatBot.validarServicoLicenciamentoResult> ValidationSecureCodeLicenciamento(string SecureCode, string tipoDocumentoIn, string plataforma)
        {
            try
            {
                ValidarServicoLicenciamento obter = new ValidarServicoLicenciamento();
                var result = await obter.validarServicoLicenciamento(0, plataforma, Convert.ToDouble(SecureCode), tipoDocumentoIn, 0);
                return result;
            }
            catch (Exception err)
            {
                return null;
            }
        }

        /// <summary>
        /// Função responsável por chamar validarServicoLicenciamento no WebService. Passa ano como parâmetro.
        /// </summary>
        /// <param name="SecureCode">Código de Segurança</param>
        /// <param name="tipoDocumentoIn">Tipo de documento "D" ou "F"</param>
        /// <param name="year">Ano desejado para licenciamento</param>
        /// <param name="plataforma">Tipo da plataforma</param>
        /// <returns></returns>
        public async Task<wsDetranChatBot.validarServicoLicenciamentoResult> ValidationSecureCodeLicenciamento(string SecureCode, string tipoDocumentoIn, double year, string plataforma)
        {
            try
            {
                ValidarServicoLicenciamento obter = new ValidarServicoLicenciamento();
                var result = await obter.validarServicoLicenciamento(0, plataforma, Convert.ToDouble(SecureCode), tipoDocumentoIn, year);
                return result;
            }
            catch (Exception err)
            {
                return null;
            }
        }

        /// <summary>
        /// OBJETIVO: Validar formato de entrada e realizar chamada ao metodo assincrono passando como parâmetro
        ///           somento renavam.
        /// </summary>
        /// <param name="Renavam"></param>
        /// <returns></returns>
        public async Task<wsDetranChatBot.validarServicoLicenciamentoResult> ValidationRenavam(string Renavam, string tipoDocumentoIn, string plataforma)
        {
            try
            {
                ValidarServicoLicenciamento obter = new ValidarServicoLicenciamento();
                var result = await obter.validarServicoLicenciamento(Convert.ToDouble(Renavam), plataforma, 0, tipoDocumentoIn, 0);
                return result;
            }
            catch (Exception err)
            {
                //erro de conexao
                // atribuir erro
                return null;
            }

            return null;
        }

        /// <summary>
        /// OBJETIVO: Verificar a existência de pendencias, mediante o ano de licenciamento.
        /// </summary>
        /// <returns></returns>
        public static bool Pendency(double[] anoLicenciamento)
        {
            if (anoLicenciamento != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// OBJETIVO: Verificar quantidade de anos que existem para licenciar.
        /// </summary>
        /// <returns></returns>
        public static bool ValidationYear(double contadorAnoLicenciamento)
        {
            if (contadorAnoLicenciamento > 1)
            {
                return true;
            }
            return false;
        }
    }


}
