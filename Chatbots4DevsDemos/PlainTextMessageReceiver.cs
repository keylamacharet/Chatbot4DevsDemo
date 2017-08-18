using System;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client;
using Takenet.MessagingHub.Client.Listener;
using Takenet.MessagingHub.Client.Sender;
using System.Diagnostics;
using Takenet.MessagingHub.Client.Extensions.Broadcast;
using Lime.Messaging.Contents;

namespace Chatbots4DevsDemo
{
    public class PlainTextMessageReceiver : IMessageReceiver
    {
        private readonly IMessagingHubSender _sender;
        private readonly IBroadcastExtension _broadcast;

        public PlainTextMessageReceiver(IMessagingHubSender sender, IBroadcastExtension broadcast)
        {
            _sender = sender;
            _broadcast = broadcast;            
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {            
            string mylist = "cb4ddemolist@broadcast.msging.net";
            if (message.Content.ToString().ToUpper().Contains("PARTICIPAR"))
            {
                await CreateDistributionList(mylist);

                await AddUserToList(message, mylist);

                await _sender.SendMessageAsync("Obrigada, você fará parte do nosso teste!", message.From, cancellationToken);
            }
            else if (message.Content.ToString().ToUpper().Contains("SENDDEMOBROAD"))
            {
                await SendBroad(message, mylist);
            }
            else if (message.Content.ToString().ToUpper().Contains("SENDBROAD"))
            {
                await _sender.SendMessageAsync("Ah, tentando mandar broads, né? Te peguei ;-).", message.From, cancellationToken);
            }
            else
            {
                await SendDefaultMessage(message, cancellationToken);
            }
        }

        private async Task SendBroad(Message message, string mylist)
        {
            string msg = message.Content.ToString().Replace("SENDDEMOBROAD", "");
            await _broadcast.SendMessageAsync(mylist, msg);
        }
        private async Task SendDefaultMessage(Message message, CancellationToken cancellationToken)
        {
            Document doc = new MediaLink
            {
                Type = new MediaType("image", "png"),
                PreviewType = new MediaType("image", "png"),
                Uri = new Uri("http://chatbotsbrasil.take.net/wp-content/uploads/2017/05/chatbots4devs.png"),
                PreviewUri = new Uri("http://chatbotsbrasil.take.net/wp-content/uploads/2017/05/chatbots4devs.png"),
                Size = 0,
                Text = "Esse bot é só uma demonstração."
            };
            await _sender.SendMessageAsync(doc, message.From, cancellationToken);
        }

        


        private async Task CreateDistributionList(string mylist)
        {
            //Create distribution list
            try
            {
                await _broadcast.CreateDistributionListAsync(mylist);
            }
            catch
            {
                //TODO: Log error to Create Distribution List
            }
        }

        private async Task AddUserToList(Message message, string mylist)
        {
            //Add user if not part of list
            if (!(await _broadcast.HasRecipientAsync(mylist, message.From.ToIdentity())))
            {
                await _broadcast.AddRecipientAsync(mylist, message.From.ToIdentity());
            }
        }



       
    }
}
