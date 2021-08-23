using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using wsDetranChatBot;

namespace CoreBot.Fields
{
    public class RenovationFields
    {
        public int cepCont = 1;
        public string plataforma = "web";
        public string value;
        public int controler = 0;
        public string municipioDescricao;
        public string txtAtvRemunerada;
        public string txtTipoPagamento;

        //contadores

        public int contCpf = 1;
        public int contUF = 1;
        public int contID = 1;
        public int contDigit = 1;
        public int contEmail = 1;
        public int contPhone = 1;
        public int contNumEnd = 1;
        public int txtNumId = 0;

        //IDENTIFICAÇÃO DO CONDUTOR, PERMISSIONÁRIO OU CANDIDATO

        public string identidade;
        public string digitoID;
        public string ufIdentidadeIn;
        public string tipoOrgao;
        public string orgaoEmissorIn;
        public string orgaoDescricao;



        public string [] ufSiglas = { "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA", "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI", "RJ", "RN", "RS", "RO", "RR", "SC", "SP", "SE", "TO" };


        public string[] MinisteriosIn = {"MA - MINISTERIO DA AERONAUTICA","MB - MARINHA DO BRASIL","MD - MINISTERIO DA DEFESA","MEX - MINIS DEFESA EXERCITO BRASILEIRO","MM - MINISTERIO DA MARINHA","MP - MINISTERIO PUBLICO","MREX - MINISTERIO DAS RELACOES EXTERIORES","MT - MINISTERIO DO TRABALHO","MTE - MINISTERIO DO TRABALHO E EMPREGO"};


        public string[] ConselhosIn = { "CAU - CONSELHO ARQUITETURA E URBANISMO",
    "CBM - CORPO DE BOMBEIRO MILITAR",
    "CFE - CONSELHO FEDERAL DE ENFERMAGEM",
    "CFEA - CONS FED DE ENGENHARIA E AGRONOMIA",
    "CFEA - CONSELHO FEDERAL DE ENG E AGRONOMIA",
    "CFF - CONSELHO FEDERAL DE FONOAUDIOLOGIA",
    "CFRO - CONSELHO FEDERAL REGIONAL ODONTOLOGIA",
    "CFRO - CONSELHO FED E REG DE ODONTOLOGIA",
    "CGPI - COORD GERAL DE POLICIA DE IMIGRACAO",
    "CGPMAF - COORD G POLIC MARIT AEROP FRONTEIRA",
    "CNTR - CONSELHO NACIONAL TECNICO RADIOLOGIA",
    "COMAER - COMANDO MINISTERIO DA AERONAUTICA",
    "CONFEA - CONS FEDERAL ENGENHARIA E AGRONOMIA",
    "CORECON - CONSELHO REGIONAL DE ECONOMIA",
    "COREN - CONSELHO FEDERAL DE ENFERMAGEM",
    "CORENSE - CONSELHO REG DE ENFERMAGEM SERGIPE",
    "CRA - CONSELHO REGIONAL DE ADMINISTRAÇÃO",
    "CRB - CONSELHO REGIONAL DE BIOLOGIA",
    "CRC - CONSELHO REGIONAL DE CONTABILIDADE",
    "CRE - CONSELHO REGIONAL DE ECONOMIA",
    "CREA - CONS. REG. DE ENG, ARQ E AGRONOMIA",
    "CRECI - CONSELHO REG DE CORRETORES DE IMOVEIS",
    "CRECIRJ - IDENTIDADE",
    "CREF - CONSELHO REGIONAL DE EDUCACAO FISICA",
    "CRESS - CONSELHO REGIONAL DE SERVICO SOCIAL",
    "CRFA - CONSELHO NACIONAL DE FONOAUDIOLOGIA",
    "CRM - CONSELHO REGIONAL DE MEDICINA",
    "CRMV - CONS REG DE MEDICINA VETERINARIA",
    "CRMV - CONS REGIONAL MEDICINA VETERINARIA",
    "CRO - CONSELHO REGIONAL DE ODONTOLOGIA",
    "CRP - CONSELHO REGIONAL DE PSICOLOGIA",
    "CRQ - CONSELHO REGIONAL DE QUIMICA",
    "OAB - ORDEM DOS ADVOGADOS DO BRASIL"};



        public string[] InstitutosIn = { "IFP - INSTITUTO FELIX PACHECO",
    "IIACM - INST. IDENT. ANDERSON C. DE MELO",
    "IIAMP - INST. IDENTIF. AROLDO MENDES PAIVA",
    "IICM - INST. IDENTIF. CARLOS MENEZES",
    "IIGP - INST. IDENTIF. GONCALO PEREIRA",
    "IIJDM - INST. IDENTIF. JOAO DE DEUS MARTINS",
    "IIPC - INST DE IDENTIFICACAO POLICIA CIVIL",
    "IIRGD - INST. IDENTIF. RICARDO GLAMBETON D.",
    "IIRHM - INST. IDENTIF. RAIMUNDO H. DE MELLO",
    "IISEP - INST. IDENT. SEC. DO ESTADO PUBLICO",
    "INIMJ - INST NAC IDENT MINISTERIO JUSTICA",
    "IPF - INSTITUTO PEREIRA FAUSTINO",
    "ITCP - INSTITUTO TECNICO CIENTIFICO",
    "ITEP - INSTITUTO TECNICO CIENT DE POLICIA"};


        public string[] PoliciaIn = { "AÇÃO - POLICIA CIVIL INSTITUTO DE IDENTIFI",
    "DGPC - DIRETORIA GERAL DA POLICIA CIVIL",
    "DPF - DEPARTAMENTO DE POLICIA FEDERAL",
    "ONTEIRA - SERGIPE DIV POL MAR AREIA FRONTEIRA",
    "PC - POLICIA CIVIL",
    "PCEMG - P. CIVIL DO ESTADO DE MINAS GERAIS",
    "PCII - POLICIA CIVIL I. IDENTIFICAÇÃO",
    "PCMG - POLICIA CIVIL DE MINAS GERAIS",
    "PF - POLICIA FEDERAL",
    "PM - POLICIA MILITAR",
    "PMERJ - POLICIA MILITAR ESTADO R DE JANEIRO",
    "POLITEC - PERICIA OFICIAL E INDENTIF. TECNICA",
    "PTC - POLICIA TECNICO CIENTIFICA",
    "RGPM - REGISTRO POLICIA MILITAR",
    "SEDPMAF - SERGIPE DIV POL MAR AREA FRONTEIRA",
    "SGPC - SUPERINT GERAL DE POLICIA CIVIL",
    "SPTC - SUPERINT. DE POLICIA TEC-CIENTIFICA",
    "SRDPF - SUPERINT REG DEPTO DA POL FEDERAL"};


        public string[] OutrasSecsIn = { 
    "SCC - SECRETARIA DE ESTADO DA CASA CIVIL",
    "SCJDS - SEC COORD DE JUSTICA E DEF SOCIAL",
    "SDS - SECRETARIA DA DEFESA SOCIAL",
    "SE - SE",
    "SECC - SEC. DE ESTADO CASA CIVIL/RJ",
    "SEDC - SEC EST SEG PUBLICA E DEF CIDADAO",
    "SEDPMAF - SERGIPE DIV POL MAR AREA FRONTEIRA",
    "SEGUP - SECRETARIA DE SEGURANCA PUBLICA",
    "SEJSP - SEC DE EST DA JUSTICA E SEG PUBLICA",
    "SEJUSP - SEC ESTADO JUSTIÇA SEGURANÇA PÚBLIC",
    "SEJUSPC - SEC DE EST DE JUS E SEG PUB CEARA",
    "SEP - INCLUSAO TEMPORARIA P BIOMETRIA",
    "SEPC - SEC DE ESTADO DA POLICIA CIVIL",
    "SESC - SEC DE EST DA SEGURANCA CIDADA",
    "SESDC - SEC EST DA SEG DEFESA E CIDADANIA",
    "SESDEC - SEC EST SEGURANCA DEF E CIDADANIA",
    "SESP - SECR DE ESTADO DE SEGURANCA PUBLICA",
    "SESPDC - SEC EST SEG PúBL E DEF DO CIDADãO",
    "SESPDS - SEC. EST.SEG.PUB. DEF. SOCIAL",
    "SESPPC - INCLUIDO TEMPORARIAMENTE",
    "SJ - SECRETARIA DA JUSTICA",
    "SJDS - SECR COORD DE JUSTICA E DEF. SOC.",
    "SJDS/DI - SEC JUSTIçA DESEN SOCIAL DI",
    "SJS - SECRETARIA DE JUSTICA E SEGURANCA",
    "SJS/DI - SEC JUSTIÇA SEGURANÇA DI",
    "SJS/II - SECRETARIA JUSTICA E SEGURANCA/II",
    "SJSP - SECRETARIA DE JUSTICA E SEG.PUBLICA",
    "SJTC - SECRET JUSTICA TRABALHO E CIDADANIA",
    "SPSP - SEC POLIXIA SEG PUBLICA",
    "SSDC - SEC. DE SEG. DEFESA E CIDADANIA",
    "SSDS - SEC DA SEGURANCA E DA DEFESA SOCIAL",
    "SSI - SEC DE SEGURANCA E INFORMACOES",
    "SSIPT - SECRETARIA SEG INT PUBLICA",
    "SSP/DI - SEC SEGURANÇA PÚBLICA DI",
    "SSP/PC - SEC SEGURANCA PUBLIC POLICIA CIVIL",
    "SSPCGP - SEC DA SEG PUBLICA COOD G PERICIAS",
    "SSPDC - SSP E DEFESA DO CIDADAO",
    "SSPDS - SEC SEG PUBLICA E DEFESA SOCIAL",
    "SSPPC - SEC SEG PUBLICA POLICIA CIVIL",
    "SSSP - SECRETARIA SERGIPANA DE SEG PUBLICA",};


        public string[] OutrosIn = {
    "BA - BAHIA",
    "CA - COMANDO DA AERONAUTICA",
    "CAER - COMANDO DA AERONAUTICA",
    "CBMDF - CORPO DE BOMBEIROS M DIST FEDERAL",
    "CBMERJ - CORPO DE BOMBEIRO M EST RIO DE JAN",
    "CGPI - COORD GERAL DE POLICIA DE IMIGRACAO",
    "CGPMAF - COORD G POLIC MARIT AEROP FRONTEIRA",
    "CSM - CERTIFICADO DE SERVIçO MILITAR",
    "CTPD - CATEIRA DE TRABALHO E PREVIDENCIA S",
    "CTPS - CARTEIRA TRABALHO PREV SOCIAL",
    "CTPS - CARTEIRA PROFISSIONAL",
    "DETRAN - DETRAN",
    "DIAP - DEPTO. DE IDENTIFICACAO DO AMAPA",
    "DIC - DIRETORIA DE IDENTIFICACAO CIVIL",
    "DICC - DEP IDENTIFICACAO CIVIL E CRIMINAL",
    "DIREX - DIRETORIA EXECUTIVA",
    "DPRF - DEPART POLICIA RODOVIARIA FEDERAL",
    "DPMAF - DIVISAO POLICIA MAR AREA FRONTEIRA",
    "DPTC - DEP. DE POLICIA TECNICO CIENTIFICA",
    "DRT - DEPARTAMENTO REGIONAL DO TRABALHO",
    "EB - EXERCITO BRASILEIRO",
    "GEJUSPC - GER.DE EST.DA JUSTICA,SEG.PUB E CID",
    "GESP - GERENCIA DE EST DA SEG PUBLICA",
    "GIPM - INCLUIDO PARA ABERTURA DE PROCESSO",
    "GRJUSPC - GRUPOJUSICA",
    "SRTE - SUP REGIONAL DO TRABALHO E EMPREGO",
    "SRTE - SUPERITENDêNCIA REGIONAL DO TRABALH"};
        


        public string codigoRetorno;
        public string mensagem;
        public int erro;
        public string[] vetDescricaoSetor = new string [29];
        public string[] vetDescricaoLocal;
        public string[] vetCodigoLocal;
        public string[] vetCodigoSetor;

        public string valorCNH;

       
        //ENDEREÇO DO CONDUTOR, PERMISSIONÁRIO OU CANDIDATO

        
        public string logradouro = "";
        public string emailAut;
        public string localProve;
        public string numero;
        public string cursos;
        

        public bool IsNumeric(string value)
        {
            return value.All(char.IsNumber);
        }



        //BOLETO -- DUA
        public string codBarras;
        public double vencimento;
        public string enderecoPagador = "AVENIDA TANCREDO NEVES, S/N.";
        public string bairroPagador = "PONTO NOVO";
        public string municipioPagador = "ARACAJU";
        public string ufPagador = "SE";
        public string cepPagador = "49097510";
        public string dataProcessamento;
        public string nossoNumero = "293553708";
        public string valorPagar;
        public string mensagem1;
        public string mensagem2;
        public string linhaCodBarra;
        
        public string tipo;
        public string numeroDocumento;
        public string dataVenc;
        public string tituloVenc;
        public string[] vetDescInfracao = new string [80];
        public string agencia;
        public string renach = "";


        public string sexo;
        public string escolaridade;
        public string nacionalidade;
        public int  contadorSetor;


        public string banco;

        //-------------------------------------------
        public string cpf;
        public string tipoDocumentoIn;
        public string localEntrega = "";
        public string TipoEndereco = "";
        public string endereco = "";
        public string numeroEndereco = "";
        public string complemento = "";
        public string bairro = "";
        public string municipio = "";
        public string cep = "";
        public string uf = "SE";
        public string telefone = ""; 
        public string setorEntrega = "";///
        public string email = "";
        public string ddd = ""; 
        public string nomeUsuario = "";
        public string categoria = "";
        public int TipoDocPrimeiraHab = 1;
        public string NumeroDocPrimeiraHab = "";
        public string DigitoPrimeiraHab = "";
        public string orgaoEmissorPrimeiraHab = "";
        public string UfDocPrimeiraHab = "";
        public string nomeMae = "";
        public string nomePai = "";
        public string sexoOut = "";
        public string nacionalidadeOut = "";
        public string ufNaturalidade = "";
        public string localidadeNasc = "";
        public string naturalidade = "";
        public string dataNasc = "0";
        public string escolaridadeCod = "";
        public string deficienciaFisica = "";
        public string ProvaCursoRenovacao = "1";
        public string trabalhoRemunerado = "";
        public string UfPrimeiraHab = "";
        public string datapriemirahab = "";
        public string SetorVirtual = "";/// 



        //RETORNO SERVIÇO DE RENOVAÇÃO


        public int codigoRetornoField;

        private erro erroField;

        public string vNomeField;

        public string vTipoPessoaField;

        public string vCpfField;

        public string vNomePaiField;

        public string vNomeMaeField;

        public string vTipoDocumentoField;

        public string vNumeroDocumentoField;

        public string vDigitoNumeroDocumentoField;

        public string vOrgaoEmissorField;

        public string vUFemissaoField;

        public string vDataNascimentoField;

        public string vLocalidadeNascimentoField;

        public string vLocalidadeNascimentoDescricaoField;

        public string vUFnaturalidadeField;

        public string vUFnaturalidadeDescricaoField;

        public string vCodigoSexoField;

        public string vDescricaoSexoField;

        public string vCodigoEscolaridadeField;

        public string vDescricaoEscolaridadeField;

        public string vCodigoNacionalidadeField;

        public string vDescricaoNacionalidadeField;

        public string vTipoCondutorCodigoField;

        public string vTipoCondutorDescricaoField;

        public string vCategoriaField;

        public string vTemBiometriaField;

        public string vFlagSenhaProcessoField;

        public string vCategoriaRebaixamentoField;

        public string vPerguntaRebaixamentoField;

        public string vRegistroField;

        public string vFormularioRenachField;

        public string vAtividadeRemuneradaField;

        public string vDeficienciaFisicaField;

        public string vDataExameValidoField;

        public string vProcessoField;

        public string vTipoProcessoField;

        public string vRequerField;

        public string vProvaCursoRenovacaoField;

        public string vTransfJurisdicaoField;

        public string vRgDifereBincoField;

        public string vContProcessosSuspensaoField;

        public string vCnHcarteiraField;

        public string vFlagCnhDefinitivaField;

        public string vNumeroCnhField;

        public string vDataEmissaoCnhField;

        public string vUFdominioCnhField;

        public string vDataprimeiraCnhField;

        public string vUFprimeiraCnhField;

        public string vCodigoSegurancaCnhField;

        public string vDataValidadeCnhField;

        public string vValidadeCnhAutorizadaField;

        public string vMensagemProcessoField;

        public string[] vVetCodigoLocaisProvasExamesField;

        public string[] vVetDescricaoLocaisProvasExamesField;

        public string vContadorSetorField;

        public string[] vVetDescricaoSetorField;

        public string[] vVetCodigoSetorField;

        public double vDocArrecadacaoField;

        public decimal[] vVetTaxasField; 

        public string[] vVetDescricaoDebitosField;

        public string vDataProcessamentoField;

        public string vNomeCondutorField;

        public string[] vValor_AField;

        public string vValorApagarField;

        public string vVencimentoField;

        public string vAgenciaField;

        public string vTotal_AField;

        public string vLinhaDigField;

        public string vLinhaCodBarraField;

        public string vIdentidadeField;

        public string vASBACE1Field;

        public string vFichaCompField;

        public string[] vVetDescricaoField;

        public string vRenachField;

        public string vDataEmissaoField;

        public string[] vVetArrayMensagemField;

        
    }
}











