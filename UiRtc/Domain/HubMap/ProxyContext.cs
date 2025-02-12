using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using UiRtc.Typing.PublicInterface;

namespace UiRtc.Public
{
    internal class ProxyContext : IUiRtcProxyContext
    {
        private readonly HubCallerContext _context;
        public ProxyContext(HubCallerContext context)
        {
            _context = context;
        }

        public string ConnectionId => _context.ConnectionId;

        public string? UserIdentifier => _context.UserIdentifier;

        public ClaimsPrincipal? User => _context.User;

        public IDictionary<object, object?> Items => _context.Items;

        public IFeatureCollection Features => _context.Features;

        public CancellationToken ConnectionAborted => _context.ConnectionAborted;

        public void Abort()
        {
            _context.Abort();
        }
    }
}
