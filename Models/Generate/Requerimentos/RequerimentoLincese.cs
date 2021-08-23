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
    public class RequerimentoLincese
    {


        //public static void GenerateInvoice()
        //{
        //    Document doc = new Document(PageSize.A4, 2F, 2F, 25F, 10F);

        //    string caminho = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        //    FileStream file = new FileStream(caminho + "/DUA" + ".pdf", FileMode.Create);

        //    PdfWriter writer = PdfWriter.GetInstance(doc, file);

        //    WriteDocument(doc, writer);
        //}

        public static byte[] GenerateInvoice2(LicenseFields LicenseFields)
        {
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            Document document = new Document(PageSize.A4, 2F, 2F, 25F, 10F);
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

            //FieldsGenerate data = new FieldsGenerate();

            WriteDocument(document, writer, LicenseFields);

            byte[] bytes = memoryStream.ToArray();

            return bytes;
        }


        public static void WriteDocument(Document doc, PdfWriter writer, LicenseFields LicenseFields)
        {
            doc.Open();
            Rectangle page = doc.PageSize;
            Font Titulo = FontFactory.GetFont("Verdana", 12F, Font.BOLD, BaseColor.BLACK);
            Font Subtitulo = FontFactory.GetFont("Verdana", 11F, Font.BOLD, BaseColor.BLACK);
            Font FontePadrao = FontFactory.GetFont("Verdana", 10F, Font.NORMAL, BaseColor.BLACK);
            Font Cabecalho = FontFactory.GetFont("Verdana", 7F, Font.NORMAL, BaseColor.BLACK);
            Paragraph parag = new Paragraph(new Phrase("\n"));

            //string pathImageDetran = Path.Combine(Environment.CurrentDirectory, @"Assets/Docs", "detran.jpeg");
            string pathImageGov = "https://www.detran.se.gov.br/portal/img/governo/logo_detran.png";
            string pathImageBanese = "https://www.detran.se.gov.br/portal/images/banese.jpg";

            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(pathImageGov);
            iTextSharp.text.Image imageBanese = iTextSharp.text.Image.GetInstance(pathImageBanese);

            doc.Add(Header());

            doc.Add(tableGOV(Cabecalho, image));

            doc.Add(tableDUA(Titulo));

            doc.Add(tableDados(FontePadrao, page, image, LicenseFields, Subtitulo));

            doc.Add(tableDadosPagamento(FontePadrao, page, image, LicenseFields, Subtitulo));

            doc.Add(tableRecebimentoCRLV(FontePadrao, page, image, LicenseFields, Subtitulo));

            doc.Add(tableEmail(FontePadrao, page, image, LicenseFields, Subtitulo));

            doc.Add(tableDadosRequerimento(FontePadrao, page, image, LicenseFields, Subtitulo));

            doc.Add(tableAtencao(FontePadrao, page, image, LicenseFields, Subtitulo));

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

            PdfPCell cell1 = new PdfPCell();

            AddImageInCell(cell1, image, 50f, 50f, 1);
            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
            cell1.Border = 1;
            //cell1.Padding = 4f;
            cell1.Rowspan = 4;
            tableGOV.AddCell(cell1);

            /*float[] widths = new float[] { 10f, 10f, 10f, 10f, 10f };
            tableGOV.SetWidths(widths);*/

            PdfPCell cell0 = new PdfPCell(new Phrase("Governo de Sergipe \n Secretaria de Estado da Segurança Pública \n Departamento Estadual de Trânsito DETRAN/SE \n 29/07/2021 11:05", Cabecalho));
            cell0.HorizontalAlignment = Element.ALIGN_CENTER;
            cell0.Border = 1;
            tableGOV.AddCell(cell0);
            return tableGOV;
        }


        public static PdfPTable tableDUA(Font Titulo)
        {
            PdfPTable tableDUA = new PdfPTable(1);
            PdfPCell cell0 = new PdfPCell(new Phrase("Comprovante do Requeriemento de Serviço", Titulo));
            cell0.HorizontalAlignment = 1;
            cell0.Border = 1;
            tableDUA.AddCell(cell0);
            return tableDUA;
        }


        public static PdfPTable tableDados(Font FontePadrao, Rectangle page, iTextSharp.text.Image image, LicenseFields LicenseFields, Font Subtitulo)
        {

            PdfPTable table1 = new PdfPTable(1);
            table1.SpacingAfter = 10f;

            PdfPCell cell = new PdfPCell(new Phrase("1. Dados do veículo", Subtitulo));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table1.AddCell(cell);


            cell = new PdfPCell(new Phrase("Renavam: " + LicenseFields.renavamOut, FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER + Rectangle.RIGHT_BORDER;
            table1.AddCell(cell);

            cell = new PdfPCell(new Phrase("Placa: " + LicenseFields.placa, FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER + Rectangle.RIGHT_BORDER;
            table1.AddCell(cell);

            cell = new PdfPCell(new Phrase("Proprietário/Arrendatário: " + LicenseFields.nomeProprietario, FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER + Rectangle.RIGHT_BORDER + Rectangle.BOTTOM_BORDER;
            table1.AddCell(cell);

            return table1;
        }


        public static PdfPTable tableDadosPagamento(Font FontePadrao, Rectangle page, iTextSharp.text.Image image, LicenseFields LicenseFields, Font Subtitulo)
        {
            PdfPTable table2 = new PdfPTable(1);
            table2.SpacingAfter = 10f;

            PdfPCell cell = new PdfPCell(new Phrase("2. Informações Sobre Pagamento", Subtitulo));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table2.AddCell(cell);

            cell = new PdfPCell(new Phrase("Vencimento: " + Format.Output.reverseDate(LicenseFields.vencimento.ToString()), FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER + Rectangle.RIGHT_BORDER;
            table2.AddCell(cell);

            cell = new PdfPCell(new Phrase("Exercício: " + LicenseFields.exercicio, FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER + Rectangle.RIGHT_BORDER;
            table2.AddCell(cell);

            cell = new PdfPCell(new Phrase("Parcelas: " + "Cota Unica", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER + Rectangle.RIGHT_BORDER + Rectangle.BOTTOM_BORDER;
            table2.AddCell(cell);


            return table2;
        }

        public static PdfPTable tableRecebimentoCRLV(Font FontePadrao, Rectangle page, iTextSharp.text.Image image, LicenseFields LicenseFields, Font Subtitulo)
        {
            PdfPTable table3 = new PdfPTable(1);
            table3.SpacingAfter = 10f;

            PdfPCell cell = new PdfPCell(new Phrase("3. Recebimento do CRLV", Subtitulo));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table3.AddCell(cell);

            cell = new PdfPCell(new Phrase("CPF do Procurador: " + LicenseFields.cpfCnpjPagador, FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER + Rectangle.RIGHT_BORDER;
            table3.AddCell(cell);

            cell = new PdfPCell(new Phrase("Telefone para contato: ", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER + Rectangle.RIGHT_BORDER + Rectangle.BOTTOM_BORDER;
            table3.AddCell(cell);

            return table3;
        }

        public static PdfPTable tableEmail(Font FontePadrao, Rectangle page, iTextSharp.text.Image image, LicenseFields LicenseFields, Font Subtitulo)
        {
            PdfPTable table4 = new PdfPTable(1);
            table4.SpacingAfter = 10f;
            PdfPCell cell = new PdfPCell(new Phrase("4. Notificação por E-mail", Subtitulo));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table4.AddCell(cell);

            cell = new PdfPCell(new Phrase("Receber E-mail: " + " - ", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER + Rectangle.RIGHT_BORDER;
            table4.AddCell(cell);

            cell = new PdfPCell(new Phrase("E-mail: " + "-", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER + Rectangle.RIGHT_BORDER + Rectangle.BOTTOM_BORDER;
            table4.AddCell(cell);

            return table4;
        }

        public static PdfPTable tableDadosRequerimento(Font FontePadrao, Rectangle page, iTextSharp.text.Image image, LicenseFields LicenseFields, Font Subtitulo)
        {
            PdfPTable table5 = new PdfPTable(1);
            table5.SpacingAfter = 10f;

            PdfPCell cell = new PdfPCell(new Phrase("5. Dados do Requerimento", Subtitulo));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table5.AddCell(cell);

            cell = new PdfPCell(new Phrase("Tipo do licenciamento: " + "LIC ANUAL - PGTO BANESE", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER + Rectangle.RIGHT_BORDER;
            table5.AddCell(cell);

            cell = new PdfPCell(new Phrase("Protocolo: " + "671310968", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER + Rectangle.RIGHT_BORDER;
            table5.AddCell(cell);

            cell = new PdfPCell(new Phrase("Número(s) do DUA: " + "294353141,0,0,0,", FontePadrao));
            cell.Colspan = 2;
            cell.Border = 0;
            cell.Border = Rectangle.LEFT_BORDER + Rectangle.RIGHT_BORDER + Rectangle.BOTTOM_BORDER;
            table5.AddCell(cell);

            return table5;
        }

        public static PdfPTable tableAtencao(Font FontePadrao, Rectangle page, iTextSharp.text.Image image, LicenseFields LicenseFields, Font Subtitulo)
        {

            PdfPTable table6 = new PdfPTable(1);

            PdfPCell cell = new PdfPCell(new Phrase("Atenção", Subtitulo));
            cell.Colspan = 1;
            cell.Border = 0;
            table6.AddCell(cell);

            cell = new PdfPCell(new Phrase("A IMPRESSÃO DESTE FORMULÁRIO É OPCIONAL E APENAS USO DO CLIENTE, NÃO TENDO QUALQUER VALOR COMO DOCUMENTO OFICIAL DO DETRAN/SE.", FontePadrao));
            cell.Colspan = 1;
            cell.Border = 0;
            cell.PaddingLeft = 25;
            cell.PaddingRight = 25;
            cell.PaddingTop = 10;
            cell.PaddingBottom = 20;
            table6.AddCell(cell);

            return table6;
        }


        private static void AddImageInCell(PdfPCell cell, iTextSharp.text.Image image, float fitWidth, float fitHight, int Alignment)
        {
            image.ScaleToFit(166, 55);
            image.Alignment = Alignment;
            cell.AddElement(image);
        }
    }
}