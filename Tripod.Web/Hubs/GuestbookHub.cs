using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Tripod.Domain.Guestbook;

namespace Tripod.Web.Hubs
{
    [UsedImplicitly]
    public class GuestbookHub : Hub, IHandleEvent<CreateGuestbookEntry>
    {
        private readonly IProcessQueries _queries;

        public GuestbookHub(IProcessQueries queries)
        {
            _queries = queries;
        }

        public override Task OnConnected()
        {
            Clients.All.ReceiveLog("connected");
            return base.OnConnected();
        }

        [UsedImplicitly]
        public void Hello()
        {
            Clients.All.ReceiveHello();
        }

        public void Handle(CreateGuestbookEntry e)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<GuestbookHub>();
            context.Clients.All.ReceiveLog("event happened!");
        }
    }
}