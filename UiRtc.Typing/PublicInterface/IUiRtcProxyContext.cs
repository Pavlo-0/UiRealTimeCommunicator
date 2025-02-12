using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace UiRtc.Typing.PublicInterface
{

    public interface IUiRtcProxyContext
    {
        string ConnectionId { get; }

        /// <summary>
        /// Gets the user identifier.
        /// </summary>
        string? UserIdentifier { get; }

        /// <summary>
        /// Gets the user.
        /// </summary>
        ClaimsPrincipal? User { get; }

        /// <summary>
        /// Gets a key/value collection that can be used to share data within the scope of this connection.
        /// </summary>
        IDictionary<object, object?> Items { get; }

        /// <summary>
        /// Gets the collection of HTTP features available on the connection.
        /// </summary>
        IFeatureCollection Features { get; }

        /// <summary>
        /// Gets a <see cref="CancellationToken"/> that notifies when the connection is aborted.
        /// </summary>
        CancellationToken ConnectionAborted { get; }

        /// <summary>
        /// Aborts the connection.
        /// </summary>
        void Abort();
    }
}
