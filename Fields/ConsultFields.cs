using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace CoreBot.Fields
{
    public class ConsultFields
    {
        public int cont = 0;
        public string plataforma;
        public string value;
        public string cnh1;
        public string register1;
        public string date1;
        public int contTry = 1;
        public int contProtocol = 1;
        public int contCnh = 1;


        public string protocol { get; set; }
        public string date { get; set; }
        public string register { get; set; }
        public string CNH { get; set;  }

        public bool IsNumeric(string value)
        {
            return value.All(char.IsNumber);
        }



        //public string plataforma;
        public string dataNascimento;
        //public string nroProtocolo;
        public string nroProtocoloIn;
        public string nroProtocoloOut;

        public string cpfCondutor;
        public string nomeCondutor;
        public string nroRegistro;
        public string cnh;
        public string renach;
        public string categoria;
        public double dataValidadeCNH;
        public string observacoes;
        public string optCNHDigital;



        public string dataBloqueio;
        public string dataInicioPenalidade;
        public string dataFimPenalidade;
        public string motivoBloqueio;

        public string tipoSuspensao;
        public string dataSuspensao;
        public string processoSuspensao;
        public string situacaoSuspensao;




        public int codigoRetorno;
        public int erroCodigo;
        public string documentoProcessoPdf;

        public int Count { get; set; }
















        //public int codigoRetorno;
        public string[] vetDescDebitos = new string[25];
        public string marcaModelo;
        public string placa;
        public string renavamOut;


        //public int erroCodigo;
        public string erroMensagem;
        public string erroTrace;


        public string codSegurancaIn;
        public string renavamIn;


        public int recallCodigo;
        public string recallMensagem;
        public string[] recallDescricao;
        public double recallDefeito;


        /// <summary>
        ///Saídas para o método validarServicoLicenciamento
        /// </summary>
        public string codSegurancaOut;
        public string nomeProprietario;


        public string temRNTRC;
        public string tipoAutorizacaoRNTRCIn;
        public string tipoAutorizacaoRNTRCOut;
        public string nroAutorizacaoRNTRCIn;
        public string nroAutorizacaoRNTRCOut;
        public string dataValidadeRNTRC;
        public string temIsençãoIPVA;
        public string restricao;
        public double[] anoLicenciamento = new double[4];
        public double contadorAnoLicenciamento;
        public int contadorRegistro;
        public double[] vetValorCotaUnica = new double[20];
        public double[] vetParcela1 = new double[20];
        public double[] vetParcela2 = new double[20];
        public double[] vetParcela3 = new double[20];
        public double totalParcela1;
        public double totalParcela2;
        public double totalParcela3;
        public double totalCotaUnica;
        public int indiceMensagem;
        public int[] vetCodMensagem = new int[10];
        public string[] vetMensagemDua = new string[80];

        /// <summary>
        ///Saídas para o método efetuarServicoLicenciamento
        /// </summary>
        public string cpfProcurador;
        public double numeroDocumento;
        public string tipoDocumentoIn;
        public string tipoDocumentoOut;
        public string cor;
        public double[] vetTaxas = new double[17];
        public double dataProcessamento;
        public double exercicio;
        public double ind;
        public string nome;
        public string tipo;
        public string[] vetValorA = new string[17];
        public string valorApagar;
        public double vencimento;
        public string agencia;
        public string mensagem1;
        public string mensagem2;
        public string mensagem3;
        public string mensagem4;
        public string mensagem5;
        public string totalA;
        public string linhaDig;
        public string linhaCodBarra;
        public string codBarra;
        public string asBace1;
        public double indDescricao;
        public string[] vetDescInfracao = new string[80];
        public double indMensagem;
        public string[] vetDuaMensagem = new string[80];
        public string chassiSNG;
        public string tituloVenc;
        public string datsVenc;
        public double indParc;
        public double[] vetDuaParc = new double[9];
        public string[] vetValorA1Parc = new string[8];
        public string[] vetLinhaDigParc = new string[51];
        public string[] vetLinhaCodBarra = new string[44];
        public string[] vetCodBarraParc = new string[54];
        public string[] vetASBACE1Parc = new string[25];
        public string[] vetValorA2Parc = new string[8];
        public string[] vetValorA3Parc = new string[8];
        public string[] vetTotalAParc = new string[10];
        public double[] vetVencimentoParc = new double[8];
        public string flagParc1A;
        public string flagParc2A;
        public string flagParc3A;
        public string cpfCnpjPagador;
        public string enderecoPagador;
        public string cepPagador;
        public string bairroPagador;
        public string municipioPagador;
        public string ufPagador;
        public string nossoNumero;

        public string Banco { get; set; }
        //public int Count { get; set; }
        public bool SecureCodeBool { get; set; }
        public string ErrorService { get; set; }
        public string ErrorAuthentication { get; set; }

        public string IsencaoIPVA;
        public double[] anoLicenciamentoIn;
        //public string plataforma;


    }


}
