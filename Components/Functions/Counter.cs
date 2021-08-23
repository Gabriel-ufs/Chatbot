using CoreBot.Fields;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CoreBot.Components.Functions
{
    /// <summary>
    /// Classe responsável por conter funções de contagem.
    /// </summary>
    public class Counter
    {
        /// <summary>
        /// Função responsável por contar o número de tentativas de acesso (Máx. 3) e exibir mensagem quando atingir o limite.
        /// </summary>
        /// <param name="LicenseFields">Objeto utilizado para guardar varáveis dinamicamente.</param>
        /// <param name="stepContext">Contexto do diálogo que chama a função.</param>
        /// <param name="dialog">Diálogo para qual será levado caso o limite ainda não tenha sido atingido.</param>
        /// <param name="cancellationToken">Token de candelamento.</param>
        /// <param name="data">(["a/o"]+[" "]+["Informação"])Artigo e nome da informação que deve ser encontrada pelo usuário</param>
        /// <returns>DialogTurnResult com o replace de um diálogo ou fim do diálogo atual.</returns>
        public async Task<DialogTurnResult> ThreeTimes(LicenseFields LicenseFields, WaterfallStepContext stepContext, string dialog, CancellationToken cancellationToken, string data)
        {
            LicenseFields.Count += 1;
            if (LicenseFields.Count < 3)
            {
                return await stepContext.ReplaceDialogAsync(dialog, LicenseFields, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Acho que você não esta conseguindo encontrar " + data + "!\r\n" +
                                                            "Nesse caso, vou pedir para que procure e volte a falar comigo novamente depois " +
                                                            "ou entre em contato com o DETRAN, para obter mais informações");
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }


        /// <summary>
        /// Função responsável por contar o número de tentativas de acesso (Máx. 3) e exibir mensagem quando atingir o limite.
        /// </summary>
        /// <param name="CRLVeFields">Objeto utilizado para guardar varáveis dinamicamente.</param>
        /// <param name="stepContext">Contexto do diálogo que chama a função.</param>
        /// <param name="dialog">Diálogo para qual será levado caso o limite ainda não tenha sido atingido.</param>
        /// <param name="cancellationToken">Token de candelamento.</param>
        /// <param name="data">(["a/o"]+[" "]+["Informação"])Artigo e nome da informação que deve ser encontrada pelo usuário</param>
        /// <returns>DialogTurnResult com o replace de um diálogo ou fim do diálogo atual.</returns>
        public async Task<DialogTurnResult> ThreeTimes(CRLVeFields CRLVeFields, WaterfallStepContext stepContext, string dialog, CancellationToken cancellationToken, string data)
        {
            CRLVeFields.Count += 1;
            if (CRLVeFields.Count < 3)
            {
                return await stepContext.ReplaceDialogAsync(dialog, CRLVeFields, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Acho que você não esta conseguindo encontrar " + data + "!\r\n" +
                                                            "Nesse caso, vou pedir para que procure e volte a falar comigo novamente depois " +
                                                            "ou entre em contato com o DETRAN, para obter mais informações");
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }
    }
}
