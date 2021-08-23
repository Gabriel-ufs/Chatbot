using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Fields
{
    public class ConsultaDebitosFields
    {

        public string renavam;
        public string codSeguranca;
        public string plataforma = "EMULATOR";
        public string multas;
        public int contRenavam = 1;
        public int contSecCode = 1;
        public int cont = 0;
        //retornos
        public int totalregistros;
        public string[] vetDescDebitos;
        public decimal[] vetValorCotaUnica;


        public bool IsNumeric(string value)
        {
            return value.All(char.IsNumber);
        }

    }
}
