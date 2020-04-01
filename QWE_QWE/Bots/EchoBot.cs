// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class EchoBot : ActivityHandler
    {
        public static int kolvo(ushort N)
        {
            int k = 0;
            for (ushort i = 1 << 15; i > 0; i = (ushort)(i >> 1))
            {
                if ((N & i) == 0) k++;
            }
            return k;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            int choose = int.Parse(turnContext.Activity.Text);
            if (choose == 1)
            {
                await turnContext.SendActivityAsync("¬ведите x1");
                ushort x1 = ushort.Parse(turnContext.Activity.Text);
                string result = kolvo(x1) + "";
                await turnContext.SendActivityAsync("–езультат задачи є1{0}", result);

            }

        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
