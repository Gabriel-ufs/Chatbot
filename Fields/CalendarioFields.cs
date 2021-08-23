using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Fields
{
    public class CalendarioFields
    {
        public int cont = 0;
        public string plataforma = "EMULATOR";
        public string terminacaoPlacaIn;
        public string terminacaoPlacaOut;
        public string anoLicenciamentoIn;
        public string anoLicenciamentoOut;
        public string finalPlaca;
        public string anoCalendario { get; set; }
        
        public bool IsNumeric(string value)
        {
            return value.All(char.IsNumber);
        }

        public int contFinalPLaca;
        public int contAno;


        //saída WS

        public int anoLicenciamento;
        public int termincaoPLaca;
        public string dataPrimeiraParc;
        public string dataSegundaParc;
        public string dataTerceiraParcCotaUnica;
        public string dataPagToSemDesconto { get; set; }
        public string dataDesconto;
        public string dataFiscalizacao { get; set; }

       

    }
}