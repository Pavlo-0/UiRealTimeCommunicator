# Authenticated accessTokenFactory example

This example shows a small JWT-authenticated UiRealTimeCommunicator flow:

- the backend issues a demo JWT from `POST /auth/login`;
- the generated TypeScript client passes that token to SignalR with `accessTokenFactory`;
- the UiRtc handler reads `IUiRtcProxyContext.UserIdentifier`, `IUiRtcProxyContext.User`, and `IUiRtcProxyContext.ConnectionId`;
- the backend sends the response only to the connection that invoked the action.

The demo accepts these hardcoded credentials:

| User name | Password |
| --- | --- |
| `demo1` | `demo1` |
| `demo2` | `demo2` |
| `demo` | `demo` |

## Run the backend

From this folder:

```sh
cd backend
dotnet run
```

The launch profile listens on:

- `https://localhost:5001`
- `http://localhost:5000`

The frontend is configured to use `https://localhost:5001/`.

## Generate the TypeScript client

The frontend already contains a generated `src/communication/contract.ts` for convenience. To regenerate it after changing the backend contracts, run this from the repository root:

```sh
dotnet run --project .\UiRtc.TypeScriptGenerator\UiRtc.TypeScriptGenerator.csproj -- generator -p .\examples\AuthenticatedAccessTokenFactoryExample\backend\AuthenticatedAccessTokenFactoryExample.Backend.csproj -o .\examples\AuthenticatedAccessTokenFactoryExample\frontend\src\communication\contract.ts
```

If you have installed the generator as a global tool, the equivalent command is:

```sh
dotnet-uirtc -p .\examples\AuthenticatedAccessTokenFactoryExample\backend\AuthenticatedAccessTokenFactoryExample.Backend.csproj -o .\examples\AuthenticatedAccessTokenFactoryExample\frontend\src\communication\contract.ts
```

The generated client imports `IHttpConnectionOptions` and supports:

```ts
await uiRtc.initAsync({
  serverUrl: "https://localhost:5001/",
  activeHubs: "All",
  accessTokenFactory: () => authService.getAccessToken(),
});
```

## Run the frontend

From this folder:

```sh
cd frontend
npm install
npm run dev
```

Open the Vite URL, usually `http://localhost:5173`.

## What to verify

1. Start the backend.
2. Start the frontend.
3. Use the `Try without token` button. The backend rejects the protected real-time handler and no protected response is rendered.
4. Sign in as `demo1` or `demo2`.
5. Send a chat message. The response shows the authenticated user id, user name, and current SignalR connection id.

## How JWT reaches SignalR

The frontend stores the access token locally and returns it through the generated client's `accessTokenFactory`. SignalR calls that function when it creates the hub connection.

For WebSockets and Server-Sent Events, SignalR may transmit the token on the `access_token` query string. The backend configures `JwtBearerEvents.OnMessageReceived` in `backend/Program.cs` and reads that query string token for the UiRtc `Chat` hub path.

## Where the authenticated user is read

The protected real-time handler is:

```text
backend/Communication/Handlers/SendAuthenticatedMessageHandler.cs
```

It reads:

- `context.UserIdentifier`
- `context.User`
- `context.ConnectionId`

Unauthenticated calls throw a `HubException`. Authenticated calls send `ChatMessageResponse` with:

```csharp
await senderService.Send<ChatSenderContract>(connectionId).SendAuthenticatedMessage(...);
```

Passing `connectionId` targets only the current caller connection.
