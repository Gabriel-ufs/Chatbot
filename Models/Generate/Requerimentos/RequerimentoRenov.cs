using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO; // Entrada e saída de arquivos
using iTextSharp; // Biblioteca ITextSharp
using iTextSharp.text; // Extensão 1 - Text
using iTextSharp.text.pdf; // Extensão 2 - PDF
using iTextSharp.text.html.simpleparser;
using CoreBot.Models.Generate;
using Microsoft.BotBuilderSamples;
using CoreBot.Fields;
using CoreBot.Models.MethodsValidation.License;

namespace CoreBot.Models
{
    /// <summary>
    /// OBJETIVO: Geração do PDF do Documento de Arrecadação (DUA)
    /// </summary>
    public class RequerimentoRenov
    {


        //public static void GenerateInvoice()
        //{
        //    Document doc = new Document(PageSize.A4, 2F, 2F, 25F, 10F);

        //    string caminho = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        //    FileStream file = new FileStream(caminho + "/DUA" + ".pdf", FileMode.Create);

        //    PdfWriter writer = PdfWriter.GetInstance(doc, file);

        //    WriteDocument(doc, writer);
        //}

        public static byte[] GenerateInvoice2(RenovationFields renovationFields)
        {
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            Document document = new Document(PageSize.A4, 2F, 2F, 25F, 10F);
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

            //FieldsGenerate data = new FieldsGenerate();

            WriteDocument(document, writer, renovationFields);

            byte[] bytes = memoryStream.ToArray();

            return bytes;
        }


        public static void WriteDocument(Document doc, PdfWriter writer, RenovationFields renovationFields)
        {
            doc.Open();
            Rectangle page = doc.PageSize;
            Font Titulo = FontFactory.GetFont("Verdana", 12F, Font.BOLD, BaseColor.BLACK);
            Font Subtitulo = FontFactory.GetFont("Verdana", 11F, Font.BOLD, BaseColor.BLACK);
            Font FontePadrao = FontFactory.GetFont("Verdana", 9F, Font.NORMAL, BaseColor.BLACK);
            Font FonteSenha = FontFactory.GetFont("Verdana", 9F, Font.BOLD, BaseColor.BLACK);
            Font FonteAviso = FontFactory.GetFont("Verdana", 7F, Font.NORMAL, BaseColor.BLACK);
            Font Cabecalho = FontFactory.GetFont("Verdana", 7F, Font.BOLD, BaseColor.BLACK);
            Paragraph parag = new Paragraph(new Phrase("\n"));

            //string pathImageDetran = Path.Combine(Environment.CurrentDirectory, @"Assets/Docs", "detran.jpeg");
            string pathImageGov = "https://www.detran.se.gov.br/portal/img/governo/logo_detran.png";
            string pathImageBanese = "https://www.detran.se.gov.br/portal/images/banese.jpg";

            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(pathImageGov);
            iTextSharp.text.Image imageBanese = iTextSharp.text.Image.GetInstance(pathImageBanese);

            doc.Add(Header());

            doc.Add(tableGOV(Cabecalho, image));

            doc.Add(tableDUA(Titulo));

            doc.Add(tableDados(FontePadrao, page, image, renovationFields, Subtitulo, FonteSenha));

            doc.Add(tableCondutor(FontePadrao, page, image, renovationFields, Subtitulo));

            doc.Add(tableEndereco(FontePadrao, page, image, renovationFields, Subtitulo));

            doc.Add(tableEntrega(FontePadrao, page, image, renovationFields, Subtitulo, FonteAviso));

            doc.Add(tableTaxas(FontePadrao, page, image, renovationFields, Subtitulo));

            doc.Add(tableObservacao(FonteAviso, page, image, renovationFields, Subtitulo));

            doc.Add(tableAtencao(FontePadrao, page, image, renovationFields, Subtitulo));

            doc.Close();
        }


        public static Paragraph Header()
        {
            DateTime thisDay = DateTime.Now;
            string dados = "DETRAN/SE - Portal de Serviços - Doc gerado eletronicamente em " + thisDay.ToString("dd/MM/yyyy") + " às " + thisDay.ToString("HH:mm:ss");
            Paragraph cabecalho = new Paragraph(dados, new Font(Font.NORMAL, 9));
            cabecalho.Alignment = Element.ALIGN_CENTER;
            return cabecalho;
        }

        public static PdfPTable tableGOV(Font Cabecalho, iTextSharp.text.Image image)
        {
            PdfPTable tableGOV = new PdfPTable(1);
            tableGOV.SpacingBefore = 5f;
            tableGOV.SpacingAfter = 5f;

            PdfPCell cell1 = new PdfPCell();

            AddImageInCell(cell1, image, 50f, 50f, 1);
            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
            cell1.Border = 1;
            //cell1.Padding = 4f;
            cell1.Rowspan = 4;
            tableGOV.AddCell(cell1);


            PdfPCell cell0 = new PdfPCell(new Phrase("Governo de Sergipe \n Secretaria de Estado da Segurança Pública \n Departamento Estadual de Trânsito DETRAN/SE", Cabecalho));
            cell0.HorizontalAlignment = Element.ALIGN_CENTER;
            cell0.Border = 1;
            tableGOV.AddCell(cell0);
            return tableGOV;
        }


        public static PdfPTable tableDUA(Font Titulo)
        {
            PdfPTable tableDUA = new PdfPTable(1);
            tableDUA.SpacingBefore = 5f;
            tableDUA.SpacingAfter = 5f;

            PdfPCell cell0 = new PdfPCell(new Phrase("Comprovante do Requeriemento de Serviço", Titulo));
            cell0.HorizontalAlignment = 1;
            cell0.Border = 1;
            tableDUA.AddCell(cell0);
            return tableDUA;
        }

        public static PdfPTable tableDados(Font FontePadrao, Rectangle page, iTextSharp.text.Image image, RenovationFields renovationFields, Font Subtitulo, Font FonteSenha)
        {

            PdfPTable table1 = new PdfPTable(3);
            table1.SpacingAfter = 5f;
            DateTime thisDay = DateTime.Now;


            PdfPCell cell = new PdfPCell(new Phrase("1. Informações do Requerimento", Subtitulo));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table1.AddCell(cell);

            //Linha 1
            cell = new PdfPCell(new Phrase("Objeto do Requerimento: RENOVAÇÃO", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER;
            table1.AddCell(cell);

            cell = new PdfPCell(new Phrase("Data do Requerimento: " + thisDay.ToString("dd/MM/yyyy"), FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.RIGHT_BORDER;
            table1.AddCell(cell);

            //Linha 2
            cell = new PdfPCell(new Phrase($"E-mail: {renovationFields.email}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER;
            table1.AddCell(cell);

            cell = new PdfPCell(new Phrase($"Processo: {renovationFields.vProcessoField}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.RIGHT_BORDER;
            table1.AddCell(cell);

            //Linha 3 

            cell = new PdfPCell(new Phrase($"Formulário Renach Provisório: {renovationFields.renach}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER;
            table1.AddCell(cell);

            cell = new PdfPCell(new Phrase($"DUA: {renovationFields.vDocArrecadacaoField}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.RIGHT_BORDER;
            table1.AddCell(cell);

            //Linha 4 

            cell = new PdfPCell(new Phrase($"Senha: {renovationFields.vFlagSenhaProcessoField}", FonteSenha));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER;
            table1.AddCell(cell);

            cell = new PdfPCell(new Phrase($"Local dos Exames: {renovationFields.localProve}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.RIGHT_BORDER;
            table1.AddCell(cell);

            //Linha 5

            cell = new PdfPCell(new Phrase($"Deficiência Física: {renovationFields.deficienciaFisica}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER + Rectangle.BOTTOM_BORDER;
            table1.AddCell(cell);

            cell = new PdfPCell(new Phrase($"Atividade Remunerada: {renovationFields.trabalhoRemunerado}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.RIGHT_BORDER + Rectangle.BOTTOM_BORDER;
            table1.AddCell(cell);


            return table1;
        }

        public static PdfPTable tableCondutor(Font FontePadrao, Rectangle page, iTextSharp.text.Image image, RenovationFields renovationFields, Font Subtitulo)
        {
            PdfPTable table2 = new PdfPTable(3);
            table2.SpacingAfter = 5f;

            PdfPCell cell = new PdfPCell(new Phrase("2. Informações do Condutor", Subtitulo));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table2.AddCell(cell);

            //Linha 1
            cell = new PdfPCell(new Phrase($"Nome: {renovationFields.nomeUsuario}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER;
            table2.AddCell(cell);

            cell = new PdfPCell(new Phrase($"Categoria: {renovationFields.categoria}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.RIGHT_BORDER;
            table2.AddCell(cell);

            //Linha 2
            cell = new PdfPCell(new Phrase($"CPF: {renovationFields.cpf}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER;
            table2.AddCell(cell);

            cell = new PdfPCell(new Phrase("Nova Categoria: - ", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.RIGHT_BORDER;
            table2.AddCell(cell);

            //Linha 3 

            cell = new PdfPCell(new Phrase($"Identidade: {renovationFields.identidade}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER;
            table2.AddCell(cell);

            cell = new PdfPCell(new Phrase($"Data de Nascimento: {renovationFields.dataNasc}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.RIGHT_BORDER;
            table2.AddCell(cell);

            //Linha 4 

            cell = new PdfPCell(new Phrase($"Telefone: {renovationFields.telefone}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER;
            table2.AddCell(cell);

            cell = new PdfPCell(new Phrase($"Escolaridade: {renovationFields.escolaridade}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.RIGHT_BORDER;
            table2.AddCell(cell);

            //Linha 5

            cell = new PdfPCell(new Phrase($"Nacionalidade: {renovationFields.nacionalidade}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER + Rectangle.BOTTOM_BORDER;
            table2.AddCell(cell);

            cell = new PdfPCell(new Phrase($"Nauralidade/UF: {renovationFields.naturalidade}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.RIGHT_BORDER + Rectangle.BOTTOM_BORDER;
            table2.AddCell(cell);

            return table2;
        }

        public static PdfPTable tableEndereco(Font FontePadrao, Rectangle page, iTextSharp.text.Image image, RenovationFields renovationFields, Font Subtitulo)
        {
            PdfPTable table3 = new PdfPTable(3);
            table3.SpacingAfter = 5f;

            PdfPCell cell = new PdfPCell(new Phrase("3. Endereço Residencial", Subtitulo));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table3.AddCell(cell);

            //Linha 1
            cell = new PdfPCell(new Phrase($"Endereço: {renovationFields.TipoEndereco} {renovationFields.endereco}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER;
            table3.AddCell(cell);

            cell = new PdfPCell(new Phrase($"Bairro: {renovationFields.bairro} ", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.RIGHT_BORDER;
            table3.AddCell(cell);

            //Linha 2
            cell = new PdfPCell(new Phrase($"Numero: {renovationFields.numeroEndereco}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER;
            table3.AddCell(cell);

            cell = new PdfPCell(new Phrase($"Municipio/UF: {renovationFields.municipio}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.RIGHT_BORDER;
            table3.AddCell(cell);

            //Linha 3
            cell = new PdfPCell(new Phrase($"Complemento: {renovationFields.complemento}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER + Rectangle.BOTTOM_BORDER;
            table3.AddCell(cell);

            cell = new PdfPCell(new Phrase($"CEP: {renovationFields.cep}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.RIGHT_BORDER + Rectangle.BOTTOM_BORDER;
            table3.AddCell(cell);

            return table3;
        }

        public static PdfPTable tableEntrega(Font FontePadrao, Rectangle page, iTextSharp.text.Image image, RenovationFields renovationFields, Font Subtitulo, Font FonteAviso)
        {
            PdfPTable table4 = new PdfPTable(3);
            table4.SpacingAfter = 5f;

            PdfPCell cell = new PdfPCell(new Phrase("4. Local de Entrega", Subtitulo));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table4.AddCell(cell);

            cell = new PdfPCell(new Phrase($"Setor de Atendimento: {renovationFields.localEntrega}", FontePadrao));
            cell.Colspan = 3;
            table4.AddCell(cell);

            cell = new PdfPCell(new Phrase("Seu documento estará disponível após a compensação bancária (de acordo com a forma de pagamento escolhida) e do cumprimento de todas as etapas obrigatórias neste serviço em um ponto de atendimento a ser escolhido. O documento somente será entregue ao requerente, ou seu procurador legal,munido de cópia do documento de identificação acompanhada do original para conferência e da procuração quando for o caso. A entrega do documento sedará mediante agendamento para tal fim, visto que o Detran somente atende com hora marcada.", FonteAviso));
            cell.Colspan = 3;
            table4.AddCell(cell);


            return table4;
        }

        // public static PdfPTable tableAviso(Font FonteAviso, Rectangle page, iTextSharp.text.Image image, RenovationFields renovationFields, Font Subtitulo)
        // {

        //     PdfPTable table5 = new PdfPTable(3);
        //     table5.SpacingAfter = 10f;

        //     PdfPCell cell = new PdfPCell(new Phrase("Aviso", Subtitulo));
        //     cell.Colspan = 3;
        //     table5.AddCell(cell);

        //     cell = new PdfPCell(new Phrase("Seu documento estará disponível após a compensação bancária (de acordo com a forma de pagamento escolhida) e do cumprimento de todas as etapasobrigatórias neste serviço em um ponto de atendimento a ser escolhido.O documento somente será entregue ao requerente, ou seu procurador legal,munido de cópia do documento de identificação acompanhada do original para conferência e da procuração quando for o caso. A entrega do documento sedará mediante agendamento para tal fim, visto que o Detran somente atende com hora marcada.", FonteAviso));
        //     cell.Colspan = 3;
        //     table5.AddCell(cell);

        //     return table5;
        // }

        public static PdfPTable tableTaxas(Font FontePadrao, Rectangle page, iTextSharp.text.Image image, RenovationFields renovationFields, Font Subtitulo)
        {
            PdfPTable table6 = new PdfPTable(2);
            table6.SpacingAfter = 5f;

            PdfPCell cell = new PdfPCell(new Phrase("5. Taxas a Pagar", Subtitulo));
            cell.Colspan = 2;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table6.AddCell(cell);

            cell = new PdfPCell(new Phrase($"Forma de Pagamento: {renovationFields.txtTipoPagamento}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.RIGHT_BORDER + Rectangle.LEFT_BORDER;
            table6.AddCell(cell);

            cell = new PdfPCell(new Phrase($"DUA: {renovationFields.vDocArrecadacaoField}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.RIGHT_BORDER + Rectangle.LEFT_BORDER;
            table6.AddCell(cell);

            cell = new PdfPCell(new Phrase($"RENOVACAO DA CNH/PPD: {renovationFields.valorPagar}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.RIGHT_BORDER + Rectangle.LEFT_BORDER;
            table6.AddCell(cell);

            cell = new PdfPCell(new Phrase($"Total a pagar:  {renovationFields.valorPagar}", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.RIGHT_BORDER + Rectangle.LEFT_BORDER + Rectangle.BOTTOM_BORDER;
            table6.AddCell(cell);


            return table6;
        }


        public static PdfPTable tableObservacao(Font FonteAviso, Rectangle page, iTextSharp.text.Image image, RenovationFields renovationFields, Font Subtitulo)
        {

            PdfPTable table7 = new PdfPTable(2);
            table7.SpacingAfter = 5f;

            PdfPCell cell = new PdfPCell(new Phrase("6.Observações", Subtitulo));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table7.AddCell(cell);

            cell = new PdfPCell(new Phrase("- ATUALIZACAO: JA POSSUI CERTIFICADO \n - Acompanhe o andamento do seu serviço no menu OPÇÕES PARA CONTINUAR SERVIÇOS REQUERIDOS do próprio portal. Todos os serviços adicionais também estão disponibilizados, entre eles: cancelamento do serviço, reemissão do documento de arrecadação, alteração dos dados do requerimento de serviço. Por motivos de segurança, o acesso será efetuado somente mediante utilização de senha. \n - O cancelamento do serviço somente será permitido enquanto não efetuado o pagamento e a alteração de dados do requerimento, enquanto não efetuadaa etapa de \"IDENTIFICAR-SE BIOMETRICAMENTE\" \n - Em caso de RENOVAÇÃO E/OU MUDANÇA DE CATEGORIA “C”,” D”e “E” é exigido o exame toxicológico que deverá ser feito antes da realização doexame médico. Veja os laboratórios homologados pelo Denatran no site do Detran/SE. Para não ser exigido o exame toxicológico deverá solicitar ao médico o rebaixamento para categoria “B”.", FonteAviso));
            cell.Colspan = 2;
            table7.AddCell(cell);

            return table7;
        }

        public static PdfPTable tableAtencao(Font FontePadrao, Rectangle page, iTextSharp.text.Image image, RenovationFields renovationFields, Font Subtitulo)
        {
            PdfPTable table8 = new PdfPTable(2);

            PdfPCell cell = new PdfPCell(new Phrase("ATENÇÃO", Subtitulo));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table8.AddCell(cell);

            cell = new PdfPCell(new Phrase($"ATENTE-SE PARA A INFORMAÇÃO REFERENTE AO EXERCÍCIO DA ATIVIDADE REMUNERADA PORQUE É DE FUNDAMENTAL IMPORTÂNCIA QUE A MESMA ESTEJA CORRETA: ATIVIDADE REMUNERADA - {renovationFields.txtAtvRemunerada}", FontePadrao));
            cell.Colspan = 2;
            table8.AddCell(cell);


            return table8;
        }





        private static void AddImageInCell(PdfPCell cell, iTextSharp.text.Image image, float fitWidth, float fitHight, int Alignment)
        {
            image.ScaleToFit(166, 55);
            image.Alignment = Alignment;
            cell.AddElement(image);
        }
    }
}