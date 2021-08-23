using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Fields
{
    /// <summary>
    /// Classe utilizada como objeto para conter valores necessários de forma dinâmica.
    /// </summary>
    public class CRLVeFields
    {
        public string plataforma;
        public string codSegurançaOut;
        public string codSegurancaIn;
        public string renavam;
        public string nomeProprietario;
        public string placaIn;
        public string placaOut;
        public int codigoRetorno;
        public string documentoCRLVePdf;

        public int erroCodigo;
        public string erroMensagem;
        public string erroTrace;
        
        public int Count { get; set; }
        public bool secureCodeBool { get; set; }
    }
}
