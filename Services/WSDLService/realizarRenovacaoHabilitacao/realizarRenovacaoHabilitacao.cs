using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBot.Services.WSDLService.realizarRenovacaoHabilitacao
{
    public class realizarRenovacaoHabilitacao
    {
        /// <summary>
        /// Função responsavel por chamar o método de realizarServicoHabilitacao do WebService
        /// </summary>
        /// <param name="vTipoPlataforma"></param>
        /// <param name="vCpf"></param>
        /// <param name="vTipoDua"></param>
        /// <param name="vLocalEntrega"></param>
        /// <param name="vTipoEndereco"></param>
        /// <param name="vEndereco"></param>
        /// <param name="vNumeroEndereco"></param>
        /// <param name="vComplemento"></param>
        /// <param name="vBairro"></param>
        /// <param name="vMunicipio"></param>
        /// <param name="vCep"></param>
        /// <param name="vUf"></param>
        /// <param name="vTelefone"></param>
        /// <param name="vSetorEntrega"></param>
        /// <param name="vEmail"></param>
        /// <param name="vDDD"></param>
        /// <param name="vNomeCandidato"></param>
        /// <param name="vCategoria"></param>
        /// <param name="vTipoDocPrimHab"></param>
        /// <param name="vNumeroPrimHab"></param>
        /// <param name="vDigitoPrimHab"></param>
        /// <param name="vOrgaoEmissorPrimHab"></param>
        /// <param name="vUfDocPrimHab"></param>
        /// <param name="vNomeMaePrimHab"></param>
        /// <param name="vNomePai"></param>
        /// <param name="vSexo"></param>
        /// <param name="vNacionalidade"></param>
        /// <param name="vUfNaturalidade"></param>
        /// <param name="vLocalidadeNasc"></param>
        /// <param name="vNaturalidade"></param>
        /// <param name="vDataNascimento"></param>
        /// <param name="vEscolaridade"></param>
        /// <param name="vDeficienciaFisica"></param>
        /// <param name="vProvaCursoRenovacao"></param>
        /// <param name="vAtividadeRemunerada"></param>
        /// <param name="vUfPrimHab"></param>
        /// <param name="vDataPrimHab"></param>
        /// <param name="vSetorVirtual"></param>
        /// <returns></returns>
        public static async Task<wsDetranChatBot.realizarServicoRenovacaoHabilitacaoResult> realizarServicoRenovacaoHabilitacao(
                    string vTipoPlataforma,
                    string vCpf,
                    string vTipoDua, //D ou F
                    string vLocalEntrega, // local de busca da cnh
                    string vTipoEndereco,
                    string vEndereco,
                    string vNumeroEndereco,
                    string vComplemento,
                    string vBairro,
                    string vMunicipio,
                    string vCep,
                    string vUf,
                    string vTelefone,
                    int vSetorEntrega,// código
                    string vEmail,
                    string vDDD,
                    string vNomeCandidato,
                    string vCategoria,
                    int vTipoDocPrimHab, //vázio
                    string vNumeroPrimHab,// vazio
                    string vDigitoPrimHab, // vazio
                    string vOrgaoEmissorPrimHab,//vazio
                    string vUfDocPrimHab, //vazio
                    string vNomeMaePrimHab,
                    string vNomePai,
                    string vSexo,
                    int vNacionalidade,
                    string vUfNaturalidade,
                    string vLocalidadeNasc,// código
                    string vNaturalidade, // vázio
                    int vDataNascimento,
                    int vEscolaridade,
                    string vDeficienciaFisica,
                    string vProvaCursoRenovacao, // 1
                    string vAtividadeRemunerada,
                    string vUfPrimHab,
                    string vDataPrimHab, 
                    string vSetorVirtual //local de busca da cnh

            )
        {
            wsDetranChatBot.wsChatbotSoapClient wsClient = Authentication.WsClient();
            wsDetranChatBot.autenticacao auth = Authentication.Auth();

            var soap = await wsClient.realizarServicoRenovacaoHabilitacaoAsync(
                auth,
                vTipoPlataforma,
                vCpf,
                vTipoDua,
                vLocalEntrega,
                vTipoEndereco,
                vEndereco,
                vNumeroEndereco,
                vComplemento,
                vBairro,
                vMunicipio,
                vCep,
                vUf,
                vTelefone,
                vSetorEntrega,
                vEmail,
                vDDD,
                vNomeCandidato,
                vCategoria,
                vTipoDocPrimHab,
                vNumeroPrimHab,
                vDigitoPrimHab,
                vOrgaoEmissorPrimHab,
                vUfDocPrimHab,
                vNomeMaePrimHab,
                vNomePai,
                vSexo,
                vNacionalidade,
                vUfNaturalidade,
                vLocalidadeNasc,
                vNaturalidade,
                vDataNascimento,
                vEscolaridade,
                vDeficienciaFisica,
                vProvaCursoRenovacao,
                vAtividadeRemunerada,
                vUfPrimHab,
                vDataPrimHab,
                vSetorVirtual

                );

            var result = soap.realizarServicoRenovacaoHabilitacaoResult;

            return result;
        }

    }
}
