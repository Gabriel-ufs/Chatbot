using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoreBot.Models.MethodsValidation.License
{
    /// <summary>
    /// Classe responsável por formatações utilizadas na aplicação.
    /// </summary>
    public class Format
    {
        public class Input
        {
            public class ValidationFormat
            {
                /// <summary>
                /// Função responsável por verificar se somente contém números no valor recebido.
                /// </summary>
                /// <param name="secureCode">Código de Segurança</param>
                /// <returns>True ou False</returns>
                public static bool IsNumber(string secureCode)
                {
                    return !secureCode.Any((e) => !Char.IsDigit(e));
                }
            }
        }

        public class Output
        {
            /// <summary>
            /// Método responsável por converter e formatar a data para o tipo dd/MM/aaaa
            /// </summary>
            /// <param name="dataDouble">Data em valor Double</param>
            /// <returns>String de data formatada</returns>
            public static string FormatData(double dataDouble)
            {
                var cont = 1;
                string dataTotal = "";
                string data = dataDouble.ToString();

                if (data.Length < 8) data = '0' + data;

                for (int i = 0; i < data.Length; i++)
                {
                    if (cont == 2)
                    {
                        dataTotal += data[i - 1].ToString() + data[i].ToString() + "/";
                    }
                    if (cont == 4)
                    {
                        dataTotal += data[i - 1].ToString() + data[i].ToString() + "/";
                    }
                    if (cont > 4)
                    {
                        dataTotal += data[i].ToString();
                    }
                    cont++;
                }

                return dataTotal;
            }

            /// <summary>
            /// Método responsável por converter e formatar a data para o tipo dd/MM/aaaa
            /// </summary>
            /// <param name="data">Data em valor String</param>
            /// <returns>String de data formatada</returns>
            public static string FormatData1(string data)
            {
                var cont = 1;
                string dataTotal = "";

                for (int i = 0; i < data.Length; i++)
                {
                    if (cont == 2)
                    {
                        dataTotal += data[i - 1].ToString() + data[i].ToString() + "/";
                    }
                    if (cont == 4)
                    {
                        dataTotal += data[i - 1].ToString() + data[i].ToString() + "/";
                    }
                    if (cont > 4)
                    {
                        dataTotal += data[i].ToString();
                    }
                    cont++;
                }

                return dataTotal;
            }

            /// <summary>
            /// Método responsável por converter e formatar a data para o tipo aaaa/MM/yyyy
            /// </summary>
            /// <param name="data">Data string ao contrário</param>
            /// <param name="cont">Contador</param>
            /// <returns></returns>
            public static string FormatData(string data, int cont)
            {
                string dataTotal = "";

                for (int i = 0; i < data.Length; i++)
                {
                    if (cont == 2)
                    {
                        dataTotal += data[i - 1].ToString() + data[i].ToString() + data[i + 1].ToString() + data[i + 2].ToString() + "/";
                    }
                    if (cont == 5)
                    {
                        dataTotal += data[i].ToString() + data[i + 1].ToString() + "/";
                    }
                    if (cont > 6)
                    {
                        dataTotal += data[i].ToString() + data[i + 1].ToString();
                        break;
                    }
                    cont++;
                }

                return dataTotal;
            }

            /// <summary>
            /// Função responsável por remover 0 à esquerda de valores.
            /// </summary>
            /// <param name="value">Valor com 0 a esquerda</param>
            /// <returns>Valor em string.</returns>
            public static string FormatValue(string value)
            {
                string valorTotal = value.TrimStart('0');
                return valorTotal;
            }

            /// <summary>
            /// Função responsável por receber uma data invertida em String, adicionar "/" e desinverte-la.
            /// </summary>
            /// <param name="Word"></param>
            /// <returns>Data no formato dd/MM/yyyy</returns>
            public static string reverseDate(string Word)
            {
                var invertida = FormatData(Word, 1);
                var NewDate = DateTime.Parse(invertida, new CultureInfo("pt-PT"));
                invertida = ZerosData(NewDate.Day.ToString()) + "/" + ZerosData(NewDate.Month.ToString()) + "/" + NewDate.Year.ToString();
                return invertida;
            }

            /// <summary>
            /// Adiciona 0 à esquerda de dias ou meses.
            /// </summary>
            /// <param name="num">Número de tamanho 1 ou 2.</param>
            /// <returns>Número normal ou com zero à esquerda</returns>
            public static string ZerosData (string num)
            {
                if (num.Length < 2) num = "0" + num;
                return num;
            }

        }
    }
}
