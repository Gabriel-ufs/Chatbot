using AdaptiveCards;
using CoreBot.Fields;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Components.Widgets
{
    public class ButtonCard
    {
        /// <summary>
        /// Função responsável por criar botão de link a partir de um texto e url passados por parâmetro.
        /// </summary>
        /// <param name="LicenseFields">Contexto</param>
        /// <param name="text">Texto de título do botão</param>
        /// <param name="url">Url de destino ao clicar no botão</param>
        /// <returns>IMessageActivity</returns>
        public IMessageActivity addButtonExpand(LicenseFields LicenseFields, string text, string url)
        {
            // Botão com link
            if (LicenseFields.plataforma == "EMULATOR" || LicenseFields.plataforma == "WEBCHAT")
            {
                var link = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
                {
                    Actions =
                    {
                        new AdaptiveOpenUrlAction {
                            Title = text,
                            Url = new Uri(url),
                        },
                    }
                };

                var button = new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = JObject.FromObject(link),
                };

                return MessageFactory.Attachment(button);
            }

            return MessageFactory.Text("");
        }

        /// <summary>
        /// Função responsável por criar botão de link a partir de um texto e url passados por parâmetro.
        /// </summary>
        /// <param name="CRLVeFields">Contexto</param>
        /// <param name="text">Texto de título do botão</param>
        /// <param name="url">Url de destino ao clicar no botão</param>
        /// <returns>IMessageActivity</returns>
        public IMessageActivity addButtonExpand(CRLVeFields CRLVeFields, string text, string url)
        {
            // Botão com link
            if (CRLVeFields.plataforma == "EMULATOR" || CRLVeFields.plataforma == "WEBCHAT")
            {
                var link = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
                {
                    Actions =
                    {
                        new AdaptiveOpenUrlAction {
                            Title = text,
                            Url = new Uri(url),
                        },
                    }
                };

                var button = new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = JObject.FromObject(link),
                };

                return MessageFactory.Attachment(button);
            }

            return MessageFactory.Text("");
        }

        /// <summary>
        /// Função responsável por gerar o botão de expandir imagens de Código de Segurança.
        /// </summary>
        /// <param name="LicenseFields">Variáveis utilizadas no diálogo</param>
        /// <param name="stepContext">Contexto do diálogo</param>
        /// <returns></returns>
        public IMessageActivity addButtonExpandSecureCode(LicenseFields LicenseFields)
        {
            // Botão com link
            if (LicenseFields.plataforma == "EMULATOR" || LicenseFields.plataforma == "WEBCHAT")
            {
                var link = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
                {
                    Actions =
                    {
                        new AdaptiveOpenUrlAction {
                            Title = "Ampliar imagem",
                            Url = new Uri("https://www.detran.se.gov.br/portal/images/codigoseg_crlve.jpeg"),
                        },
                    }
                };

                var button = new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = JObject.FromObject(link),
                };

                return MessageFactory.Attachment(button);
            }

            return MessageFactory.Text("");
        }

        internal IActivity addButtonExpandRenavam(ConsultaDebitosFields consultaDebitosFields)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Função responsável por gerar o botão de expandir imagens de Código de Segurança.
        /// </summary>
        /// <param name="LicenseFields">Variáveis utilizadas no diálogo</param>
        /// <param name="stepContext">Contexto do diálogo</param>
        /// <returns></returns>
        public IMessageActivity addButtonExpandSecureCode(CRLVeFields CRLVeFields)
        {
            // Botão com link
            if (CRLVeFields.plataforma == "EMULATOR" || CRLVeFields.plataforma == "WEBCHAT")
            {
                var link = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
                {
                    Actions =
                    {
                        new AdaptiveOpenUrlAction {
                            Title = "Ampliar imagem",
                            Url = new Uri("https://www.detran.se.gov.br/portal/images/codigoseg_crlve.jpeg"),
                        },
                    }
                };

                var button = new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = JObject.FromObject(link),
                };

                return MessageFactory.Attachment(button);
            }

            return MessageFactory.Text("");
        }

        /// <summary>
        /// Função responsável por gerar o botão de expandir imagens de Renavam.
        /// </summary>
        /// <param name="LicenseFields">Variáveis utilizadas no diálogo</param>
        /// <param name="stepContext">Contexto do diálogo</param>
        /// <returns></returns>
        public IMessageActivity addButtonExpandRenavam(LicenseFields LicenseFields)
        {
            // Botão com link
            if (LicenseFields.plataforma == "EMULATOR" || LicenseFields.plataforma == "WEBCHAT")
            {
                var link = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
                {
                    Actions =
                    {
                        new AdaptiveOpenUrlAction {
                            Title = "Ampliar imagem",
                            Url = new Uri("https://www.detran.se.gov.br/portal/images/crlve_instrucoes_renavam_placa.jpeg"),
                        },
                    }
                };

                var button = new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = JObject.FromObject(link),
                };

                return MessageFactory.Attachment(button);
            }

            return MessageFactory.Text("");
        }

        /// <summary>
        /// Função responsável por gerar o botão de expandir imagens de Renavam.
        /// </summary>
        /// <param name="LicenseFields">Variáveis utilizadas no diálogo</param>
        /// <param name="stepContext">Contexto do diálogo</param>
        /// <returns></returns>
        public IMessageActivity addButtonExpandRenavam(CRLVeFields CRLVeFields)
        {
            // Botão com link
            if (CRLVeFields.plataforma == "EMULATOR" || CRLVeFields.plataforma == "WEBCHAT")
            {
                var link = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
                {
                    Actions =
                    {
                        new AdaptiveOpenUrlAction {
                            Title = "Ampliar imagem",
                            Url = new Uri("https://www.detran.se.gov.br/portal/images/crlve_instrucoes_renavam_placa.jpeg"),
                        },
                    }
                };

                var button = new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = JObject.FromObject(link),
                };

                return MessageFactory.Attachment(button);
            }

            return MessageFactory.Text("");
        }


        public IMessageActivity addButtonExpandSecureCode2(ConsultaDebitosFields ConsultaDebitosFields)
        {
            // Botão com link
            if (ConsultaDebitosFields.plataforma == "EMULATOR" || ConsultaDebitosFields.plataforma == "WEBCHAT")
            {
                var link = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
                {
                    Actions =
                    {
                        new AdaptiveOpenUrlAction {
                            Title = "Ampliar imagem",
                            Url = new Uri("https://www.detran.se.gov.br/portal/images/codigoseg_crlve.jpeg"),
                        },
                    }
                };

                var button = new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = JObject.FromObject(link),
                };

                return MessageFactory.Attachment(button);
            }

            return MessageFactory.Text("");
        }

        public IMessageActivity addButtonExpandRenavamConsult (ConsultaDebitosFields ConsultaDebitosFields)
        {
            // Botão com link
            if (ConsultaDebitosFields.plataforma == "EMULATOR" )
            {
                var link = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
                {
                    Actions =
                    {
                        new AdaptiveOpenUrlAction {
                            Title = "Ampliar imagem",
                            Url = new Uri("https://www.detran.se.gov.br/portal/images/crlve_instrucoes_renavam_placa.jpeg"),
                        },
                    }
                };

                var button = new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = JObject.FromObject(link),
                };

                return MessageFactory.Attachment(button);
            }

            return MessageFactory.Text("");
        }
    }
}
