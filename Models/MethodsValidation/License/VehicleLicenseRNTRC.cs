using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Models.MethodsValidation.License
{
    /// <summary>
    /// Classe responsável por verificar se veículo possui RNTRC.
    /// </summary>
    public class VehicleLicenseRNTRC
    {
        /// <summary>
        /// Função responsável por verificar se veículo possui RNTRC.
        /// </summary>
        /// <param name="temRNTRC"></param>
        /// <returns>True ou False</returns>
        public static bool ValidationVehicleType(string temRNTRC)
        {
            if (temRNTRC == "S")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Função responsável por verificar o tipo de autorização.
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="tipoAutorizacaoRNTRCOut"></param>
        /// <returns>True ou False</returns>
        public static bool ValidationTypeAuthorization(string tipo, string tipoAutorizacaoRNTRCOut)
        {
            if (tipo == "1" && tipoAutorizacaoRNTRCOut == "ETC") return true;
            else if (tipo == "2" && tipoAutorizacaoRNTRCOut == "CTC") return true;
            else if (tipo == "3" && tipoAutorizacaoRNTRCOut == "TAC") return true;
            else return false;
        }

        /// <summary>
        /// Função responsável por verificar a validade da data informada pelo usuário.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Data</returns>
        public static string ValidationDate(string data)
        {
            if (data.Length >= 8)
            {
                if(Format.Input.ValidationFormat.IsNumber(data)) {
                    data = Format.Output.FormatData1(data);
                }

                var date = DateTime.Parse(data, CultureInfo.GetCultureInfo("pt-BR"));
                string result;
                var dia = date.Day;
                var mes = date.Month;
                var ano = date.Year;

                if (ano >= DateTime.Now.Year)
                {
                    if(mes >= DateTime.Now.Month || ano >= DateTime.Now.Year)
                    {
                        if(dia > DateTime.Now.Day || ano >= DateTime.Now.Year)
                        {

                            result = ano.ToString() + Format.Output.ZerosData(mes.ToString()) + Format.Output.ZerosData(dia.ToString());
                            return result;
                        }
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Função responsável por verificar se o número de autorização é válido.
        /// </summary>
        /// <param name="nroAutorizacaoRNTRCIn"></param>
        /// <param name="nroAutorizacaoRNTRCOut"></param>
        /// <returns>True ou False</returns>
        public static bool ValidationNumber(string nroAutorizacaoRNTRCIn, string nroAutorizacaoRNTRCOut)
        {
            if (Format.Input.ValidationFormat.IsNumber(nroAutorizacaoRNTRCIn) == true)
            {
                if (nroAutorizacaoRNTRCIn == nroAutorizacaoRNTRCOut)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
