using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Fields
{
    /// <summary>
    /// Classe responsável por conter diálogos básicos da aplicação.
    /// </summary>
    public class TextGlobal
    {
        public static string Ajuda { get; set; } = "Posso ajudar em algo mais? " + Emojis.Rostos.Sorriso;
        public static string Prosseguir { get; set; } = "Deseja prosseguir? \r\n1. Sim \n2. Não";
        public static string Choice { get; set; } = "\r\n1. Sim \n2. Não";
        public static string Desculpe { get; set; } = "Desculpe, não consegui entender. " + Emojis.Veiculos.Carro + " \r\n";
        public static string ChoiceDig { get; set; } = "\r\n Digite 1 para SIM ou 2 para NÃO";
        public static string Agradecimento { get; set; } = "Agradeço pelo contato!";
    }
}
