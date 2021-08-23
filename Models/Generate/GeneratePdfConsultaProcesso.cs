using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;

using System.IO; // Entrada e saída de arquivos
using iTextSharp; // Biblioteca ITextSharp
using iTextSharp.text; // Extensão 1 - Text
using iTextSharp.text.pdf; // Extensão 2 - PDF
using iTextSharp.text.html.simpleparser;
using CoreBot.Models.Generate;
using Microsoft.BotBuilderSamples;
using CoreBot.Fields;
using CoreBot.Models.MethodsValidation.License;

namespace CoreBot.Models.Generate
{
    /// <summary>
    /// OBJETIVO: Geração do PDF de Consulta de Processo
    /// </summary>
    public class GeneratePdfConsultaProcesso
    {
        public static byte[] GenerateInvoice2(ConsultFields ProcessFields)
        {
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            Document document = new Document(PageSize.A4, 2F, 2F, 25F, 10F);
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

            //FieldsGenerate data = new FieldsGenerate();
            WriteDocument(document, writer, ProcessFields);

            byte[] bytes = memoryStream.ToArray();

            return bytes;
        }

        public static void WriteDocument(Document doc, PdfWriter writer, ConsultFields ProcessFields)
        {
            doc.Open();
            Rectangle page = doc.PageSize;
            Font Titulo = FontFactory.GetFont("Verdana", 12F, Font.BOLD, BaseColor.BLACK);
            Font Subtitulo = FontFactory.GetFont("Verdana", 10F, Font.BOLD, BaseColor.BLACK);
            Font FontePadrao = FontFactory.GetFont("Verdana", 9F, Font.NORMAL, BaseColor.BLACK);
            Paragraph parag = new Paragraph(new Phrase("\n"));

            //string pathImageDetran = Path.Combine(Environment.CurrentDirectory, @"Assets/Docs", "detran.jpeg");
            string pathImageDetran = "https://www.detran.se.gov.br/portal/images/detran.jpeg";
            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(pathImageDetran);

            doc.Add(Header());
            doc.Add(parag);
            doc.Add(tableProcesso(Titulo));
            doc.Add(tableDados(FontePadrao, page, image, ProcessFields));
            doc.Add(tablePontuacao(FontePadrao, Subtitulo, ProcessFields));
            doc.Add(parag);
            doc.Add(tableAutoInfracao(FontePadrao, Subtitulo, ProcessFields, page));
            doc.Add(parag);
            doc.Add(tableBloqueio(FontePadrao, Subtitulo, ProcessFields));
            doc.Add(parag);
            doc.Add(tableCursos(FontePadrao, Subtitulo, ProcessFields));
            doc.Add(parag);
            doc.Add(tableSuspensao(FontePadrao, Subtitulo, ProcessFields));
            doc.Add(parag);


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

        public static PdfPTable tableProcesso(Font Titulo)
        {
            PdfPTable tableProcesso = new PdfPTable(1);
            PdfPCell cell0 = new PdfPCell(new Phrase("CONSULTA DE PROCESSO DE SUSPENSÃO/CASSAÇÃO DO DIREITO DE DIRIGIR", Titulo));
            cell0.HorizontalAlignment = 1;
            cell0.Border = 1;
            tableProcesso.AddCell(cell0);
            return tableProcesso;
        }

        public static PdfPTable tableDados(Font FontePadrao, Rectangle page, iTextSharp.text.Image image, ConsultFields ProcessFields)
        {

            PdfPTable table1 = new PdfPTable(5);
            table1.TotalWidth = page.Width;

            PdfPCell cell1 = new PdfPCell();

            AddImageInCell(cell1, image, 50f, 50f, 1);
            cell1.HorizontalAlignment = 0;
            cell1.Border = 1;
            cell1.Padding = 4f;
            cell1.Rowspan = 4;
            table1.AddCell(cell1);

            float[] widths = new float[] { 70f, 170f, 150f, 100f, 100f };
            table1.SetWidths(widths);

            // LINHA 1
            PdfPCell cell2 = new PdfPCell(new Phrase("CPF: " + ProcessFields.cpfCondutor, FontePadrao));
            cell2.HorizontalAlignment = 0;
            cell2.Border = 1;
            table1.AddCell(cell2);

            PdfPCell cell3 = new PdfPCell(new Phrase("Formulário CNH: " + ProcessFields.nomeCondutor, FontePadrao));
            cell3.HorizontalAlignment = 0;
            cell3.Border = 1;
            table1.AddCell(cell3);

            PdfPCell cell4 = new PdfPCell(new Phrase("Número do Registro: " + ProcessFields.nroRegistro, FontePadrao));
            cell4.HorizontalAlignment = 0;
            cell4.Border = 1;
            cell4.Colspan = 2;
            table1.AddCell(cell4);

            // LINHA 2
            PdfPCell cell5 = new PdfPCell(new Phrase("Nome: " + ProcessFields.cnh, FontePadrao));
            cell5.Colspan = 2;
            cell5.HorizontalAlignment = 0;
            cell5.Border = 1;
            table1.AddCell(cell5);

            PdfPCell cell6 = new PdfPCell(new Phrase("Formuário RENACH: " + ProcessFields.renach, FontePadrao));
            cell6.Colspan = 2;
            cell6.HorizontalAlignment = 0;
            cell6.Border = 1;
            table1.AddCell(cell6);

            // LINHA 3
            PdfPCell cell7 = new PdfPCell(new Phrase("Categoria: " + ProcessFields.categoria, FontePadrao));
            cell7.HorizontalAlignment = 0;
            cell7.Border = 1;
            table1.AddCell(cell7);

            PdfPCell cell8 = new PdfPCell(new Phrase("Validade: " + Format.Output.FormatData(ProcessFields.dataValidadeCNH), FontePadrao));
            cell8.HorizontalAlignment = 0;
            cell8.Border = 1;
            table1.AddCell(cell8);

            PdfPCell cell9 = new PdfPCell(new Phrase("Observações:\n" + ProcessFields.observacoes, FontePadrao));
            cell9.HorizontalAlignment = 0;
            cell9.Border = 1;
            table1.AddCell(cell9);

            PdfPCell cell10 = new PdfPCell(new Phrase("Optou por CNH Digital: " + ProcessFields.optCNHDigital, FontePadrao));
            cell10.HorizontalAlignment = 0;
            cell10.Border = 1;
            table1.AddCell(cell10);


            return table1;
        }

        public static PdfPTable tablePontuacao(Font FontePadrao, Font Subtitulo, ConsultFields processFields)
        {
            PdfPTable tablePontuacao = new PdfPTable(2);
            float[] Discwidths = new float[] { 400f, 200f };
            tablePontuacao.SetWidths(Discwidths);

            PdfPCell cellDisc0 = new PdfPCell(new Phrase("Pontuação", Subtitulo));
            cellDisc0.HorizontalAlignment = 1;
            cellDisc0.Border = 1;
            cellDisc0.Colspan = 2;
            tablePontuacao.AddCell(cellDisc0);



            return tablePontuacao;
        }

        public static PdfPTable tableAutoInfracao(Font FontePadrao, Font Subtitulo, ConsultFields processFieldsn, Rectangle page)
        {
            PdfPTable tableAutoInfracao = new PdfPTable(6);
            tableAutoInfracao.TotalWidth = page.Width;

            /*PdfPCell cellDisc0 = new PdfPCell(new Phrase("DISCRIMINAÇÃO DOS DÉBITOS/CRÉDITOS", Subtitulo));
            cellDisc0.HorizontalAlignment = 1;
            cellDisc0.Border = 1;
            cellDisc0.Colspan = 2;
            tableAutoInfracao.AddCell(cellDisc0);*/

            PdfPCell cellDisc1 = new PdfPCell(new Phrase("Auto(s)", Subtitulo));
            cellDisc1.HorizontalAlignment = 1;
            cellDisc1.Border = 0;
            //cellDisc1.Colspan = 6;
            tableAutoInfracao.AddCell(cellDisc1);

            PdfPCell cellDisc2 = new PdfPCell(new Phrase("Data da Autuação", Subtitulo));
            cellDisc2.HorizontalAlignment = 1;
            cellDisc2.Border = 0;
            //ellDisc2.Colspan = 6;
            tableAutoInfracao.AddCell(cellDisc2);

            PdfPCell cellDisc3 = new PdfPCell(new Phrase("Orgão Autuador", Subtitulo));
            cellDisc3.HorizontalAlignment = 1;
            cellDisc3.Border = 0;
            //cellDisc3.Colspan = 6;
            tableAutoInfracao.AddCell(cellDisc3);

            PdfPCell cellDisc4 = new PdfPCell(new Phrase("Competência", Subtitulo));
            cellDisc4.HorizontalAlignment = 1;
            cellDisc4.Border = 0;

            tableAutoInfracao.AddCell(cellDisc4);

            PdfPCell cellDisc5 = new PdfPCell(new Phrase("Situação", Subtitulo));
            cellDisc5.HorizontalAlignment = 1;
            cellDisc5.Border = 0;
            tableAutoInfracao.AddCell(cellDisc5);

            PdfPCell cellDisc6 = new PdfPCell(new Phrase("Pontos", Subtitulo));
            cellDisc6.HorizontalAlignment = 1;
            cellDisc6.Border = 0;
            tableAutoInfracao.AddCell(cellDisc6);


            return tableAutoInfracao;
        }



        public static PdfPTable tableBloqueio(Font FontePadrao, Font Subtitulo, ConsultFields ProcessFields)
        {
            PdfPTable tableBloqueio = new PdfPTable(2);
            float[] Discwidths = new float[] { 400f, 200f };
            tableBloqueio.SetWidths(Discwidths);

            PdfPCell cellDisc0 = new PdfPCell(new Phrase("BLOQUEIO(S)", Subtitulo));
            cellDisc0.HorizontalAlignment = 1;
            cellDisc0.Border = 1;
            cellDisc0.Colspan = 2;
            tableBloqueio.AddCell(cellDisc0);

            PdfPCell cellDisc1 = new PdfPCell(new Phrase("Data do Bloqueio: " + ProcessFields.dataBloqueio, FontePadrao));
            cellDisc1.HorizontalAlignment = 0;
            cellDisc1.Border = 1;
            tableBloqueio.AddCell(cellDisc1);

            PdfPCell cellDisc2 = new PdfPCell(new Phrase("Data de Inicio Penalidade: " + ProcessFields.dataInicioPenalidade, FontePadrao));
            cellDisc2.HorizontalAlignment = 0;
            cellDisc2.Border = 1;
            tableBloqueio.AddCell(cellDisc2);

            PdfPCell cellDisc3 = new PdfPCell(new Phrase("Data Fim da Penalidade: " + ProcessFields.dataFimPenalidade, FontePadrao));
            cellDisc3.HorizontalAlignment = 0;
            cellDisc3.Border = 1;
            tableBloqueio.AddCell(cellDisc3);

            PdfPCell cellDisc4 = new PdfPCell(new Phrase("Motivo do Bloqueio: " + ProcessFields.motivoBloqueio, FontePadrao));
            cellDisc4.HorizontalAlignment = 0;
            cellDisc4.Border = 1;
            tableBloqueio.AddCell(cellDisc4);

            return tableBloqueio;

        }

        public static PdfPTable tableCursos(Font FontePadrao, Font Subtitulo, ConsultFields processFields)
        {
            PdfPTable tableCursos = new PdfPTable(2);
            float[] Discwidths = new float[] { 400f, 200f };
            tableCursos.SetWidths(Discwidths);

            PdfPCell cellDisc0 = new PdfPCell(new Phrase("CURSOS ESPECIALIZADOS", Subtitulo));
            cellDisc0.HorizontalAlignment = 1;
            cellDisc0.Border = 1;
            cellDisc0.Colspan = 2;
            tableCursos.AddCell(cellDisc0);



            return tableCursos;
        }

        public static PdfPTable tableSuspensao(Font FontePadrao, Font Subtitulo, ConsultFields ProcessFields)
        {
            PdfPTable tableSuspensao = new PdfPTable(2);
            float[] Discwidths = new float[] { 400f, 200f };
            tableSuspensao.SetWidths(Discwidths);

            PdfPCell cellDisc0 = new PdfPCell(new Phrase("PROCESSOS DE SUSPENSÃO DO DIREITO DE DIRIGIR", Subtitulo));
            cellDisc0.HorizontalAlignment = 1;
            cellDisc0.Border = 1;
            cellDisc0.Colspan = 2;
            tableSuspensao.AddCell(cellDisc0);

            PdfPCell cellDisc1 = new PdfPCell(new Phrase("Tipo: " + ProcessFields.tipoSuspensao, FontePadrao));
            cellDisc1.HorizontalAlignment = 0;
            cellDisc1.Border = 1;
            tableSuspensao.AddCell(cellDisc1);

            PdfPCell cellDisc2 = new PdfPCell(new Phrase("Data: " + ProcessFields.dataSuspensao, FontePadrao));
            cellDisc2.HorizontalAlignment = 0;
            cellDisc2.Border = 1;
            tableSuspensao.AddCell(cellDisc2);

            PdfPCell cellDisc3 = new PdfPCell(new Phrase("Processo: " + ProcessFields.processoSuspensao, FontePadrao));
            cellDisc3.HorizontalAlignment = 0;
            cellDisc3.Border = 1;
            tableSuspensao.AddCell(cellDisc3);

            PdfPCell cellDisc4 = new PdfPCell(new Phrase("Situação: " + ProcessFields.situacaoSuspensao, FontePadrao));
            cellDisc4.HorizontalAlignment = 0;
            cellDisc4.Border = 1;
            tableSuspensao.AddCell(cellDisc4);

            return tableSuspensao;

        }




        private static void AddImageInCell(PdfPCell cell, iTextSharp.text.Image image, float fitWidth, float fitHight, int Alignment)
        {
            image.ScaleToFit(fitWidth, fitHight);
            image.Alignment = Alignment;
            cell.AddElement(image);
        }


    }
}
