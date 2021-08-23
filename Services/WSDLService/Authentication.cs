using Microsoft.BotBuilderSamples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Services.WSDLService
{
    static class Authentication
    {
        /// <summary>
        /// Função responsável por linkar endpoint do WebService.
        /// </summary>
        /// <returns></returns>
        public static wsDetranChatBot.wsChatbotSoapClient WsClient()
        {
            try
            {
                //wsDetranChatBot.wsChatbotSoapClient wsClient = new wsDetranChatBot.wsChatbotSoapClient(new wsDetranChatBot.wsChatbotSoapClient.EndpointConfiguration(), "http://172.28.64.58:9176/homologa/serviceChatBot");
                //wsDetranChatBot.wsChatbotSoapClient wsClient = new wsDetranChatBot.wsChatbotSoapClient(new wsDetranChatBot.wsChatbotSoapClient.EndpointConfiguration(), "http://172.28.64.58:8176/serviceChatbot");
                wsDetranChatBot.wsChatbotSoapClient wsClientDev = new wsDetranChatBot.wsChatbotSoapClient(new wsDetranChatBot.wsChatbotSoapClient.EndpointConfiguration(), "http://192.168.170.117/wsChatbot.asmx");
                //wsDetranChatBot.wsChatbotSoapClient wsClientDev = new wsDetranChatBot.wsChatbotSoapClient(new wsDetranChatBot.wsChatbotSoapClient.EndpointConfiguration(), "http://172.22.7.56/wsChatbot.asmx");

                return wsClientDev;
            }
            catch (Exception err)
            {
                return null;
            }

        }


        /// <summary>
        /// Função responsável por realizar autenticação para chamadas no WebService.
        /// </summary>
        /// <returns></returns>
        public static wsDetranChatBot.autenticacao Auth()
        {
            try
            {
                //wsDetranChatBot.autenticacao auth = new wsDetranChatBot.autenticacao
                //{
                //    loginUsuario = "4030852F-26A1-4BA7-A4E0-30940E210CF3",
                //    senhaUsuario = "bfce160d0941496f935ea762806c9160"
                //};

                wsDetranChatBot.autenticacao auth = new wsDetranChatBot.autenticacao
                {
                    loginUsuario = "CFF93871-8F2F-4E9B-9EAB-51B49B8ED3A9",
                    senhaUsuario = "46c8ab41421b479bbd65778e29ebb11a"
                };

                return auth;
            }
            catch (Exception err)
            {
                return null;
            }
        }
    }
}
