using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Components.Widgets
{
    /// <summary>
    /// Classe responsável por conter imagens a serem exibidas no diálogo.
    /// </summary>
    public class ImageCard
    {
        /// <summary>
        /// Função responsável por adicionar uma imagem a partir de uma url passada como parâmetro.
        /// </summary>
        /// <param name="url">Endereço da imagem desejada.</param>
        /// <returns></returns>
        public IMessageActivity addImageCard(string url)
        {
            AdaptiveCard card = new AdaptiveCard("1.0")
            {
                Body =
                    {
                        new AdaptiveImage()
                        {
                            Type = "Image",
                            Size = AdaptiveImageSize.Auto,
                            Style = AdaptiveImageStyle.Default,
                            HorizontalAlignment = AdaptiveHorizontalAlignment.Center,
                            Separator = true,
                            Url = new Uri(url)
                        }
                    }
            };

            IMessageActivity attachment = MessageFactory.Attachment(new Attachment
            {
                Content = card,
                ContentType = "application/vnd.microsoft.card.adaptive",
                Name = "cardName"
            });

            return attachment;
        }


        /// <summary>
        /// Função responsável por adicionar imagem do Código de Segurança.
        /// </summary>
        /// <returns></returns>
        public IMessageActivity addImageCardSecureCode()
        {
            AdaptiveCard card = new AdaptiveCard("1.0")
            {
                Body =
                    {
                        new AdaptiveImage()
                        {
                            Type = "Image",
                            Size = AdaptiveImageSize.Auto,
                            Style = AdaptiveImageStyle.Default,
                            HorizontalAlignment = AdaptiveHorizontalAlignment.Center,
                            Separator = true,
                            Url = new Uri("https://www.detran.se.gov.br/portal/images/codigoseg_crlve.jpeg")
                        }
                    }
            };

            IMessageActivity attachment = MessageFactory.Attachment(new Attachment
            {
                Content = card,
                ContentType = "application/vnd.microsoft.card.adaptive",
                Name = "cardName"
            });

            return attachment;
        }

        /// <summary>
        /// Função responsável por adicionar imagem do Renavam/Placa.
        /// </summary>
        /// <returns></returns>
        public IMessageActivity addImageCardRenavamPlaca()
        {
            AdaptiveCard card = new AdaptiveCard("1.0")
            {
                Body =
                    {
                        new AdaptiveImage()
                        {
                            Type = "Image",
                            Size = AdaptiveImageSize.Auto,
                            Style = AdaptiveImageStyle.Default,
                            HorizontalAlignment = AdaptiveHorizontalAlignment.Center,
                            Separator = true,
                            Url = new Uri("https://www.detran.se.gov.br/portal/images/crlve_instrucoes_renavam_placa.jpeg")
                        }
                    }
            };

            IMessageActivity attachment = MessageFactory.Attachment(new Attachment
            {
                Content = card,
                ContentType = "application/vnd.microsoft.card.adaptive",
                Name = "cardName"
            });

            return attachment;
        }
    }
}
