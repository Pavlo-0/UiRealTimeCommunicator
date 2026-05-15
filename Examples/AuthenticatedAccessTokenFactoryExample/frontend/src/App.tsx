import { useRef, useState } from "react";
import { authService, AuthenticatedUser, LoginResponse } from "./authService";
import {
  uiRtc,
  uiRtcCommunication,
  uiRtcSubscription,
} from "./communication/contract";
import { ChatMessageResponse } from "./communication/AuthenticatedAccessTokenFactoryExample.Backend.Communication.Models";

const serverUrl = "https://localhost:5001/";

type Subscription = {
  unsubscribe: () => void;
};

function App() {
  const [credentials, setCredentials] = useState({ userName: "demo1", password: "demo1" });
  const [user, setUser] = useState<AuthenticatedUser | null>(null);
  const [message, setMessage] = useState("Hello from an authenticated UiRtc client");
  const [messages, setMessages] = useState<ChatMessageResponse[]>([]);
  const [status, setStatus] = useState("Signed out");
  const [unauthenticatedStatus, setUnauthenticatedStatus] = useState("");
  const subscriptionRef = useRef<Subscription | null>(null);

  const loginAsync = async (userName = credentials.userName, password = credentials.password) => {
    setStatus("Signing in");

    const response = await fetch(`${serverUrl}auth/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ userName, password }),
    });

    if (!response.ok) {
      setStatus("Invalid credentials");
      return;
    }

    const login = (await response.json()) as LoginResponse;
    authService.setAccessToken(login.accessToken);
    setUser({ userId: login.userId, userName: login.userName });
    setMessages([]);

    await connectChatAsync();
    setStatus(`Connected as ${login.userName}`);
  };

  const connectChatAsync = async () => {
    subscriptionRef.current?.unsubscribe();
    subscriptionRef.current = null;
    await uiRtc.disposeAsync("All");

    await uiRtc.initAsync({
      serverUrl,
      activeHubs: "All",
      accessTokenFactory: () => authService.getAccessToken(),
    });

    subscriptionRef.current = uiRtcSubscription.Chat.AuthenticatedMessage((data) => {
      setMessages((current) => [data, ...current]);
    });
  };

  const logoutAsync = async () => {
    subscriptionRef.current?.unsubscribe();
    subscriptionRef.current = null;
    authService.clearAccessToken();
    setUser(null);
    setMessages([]);
    setStatus("Signed out");
    await uiRtc.disposeAsync("All");
  };

  const sendMessageAsync = async () => {
    setStatus("Sending message");
    await uiRtcCommunication.Chat.SendAuthenticatedMessage({ message });
    setStatus(`Connected as ${user?.userName ?? "authenticated user"}`);
  };

  const tryUnauthenticatedCallAsync = async () => {
    setUnauthenticatedStatus("Trying protected call without a token");
    authService.clearAccessToken();
    subscriptionRef.current?.unsubscribe();
    subscriptionRef.current = null;
    await uiRtc.disposeAsync("All");

    let receivedProtectedResponse = false;
    await uiRtc.initAsync({
      serverUrl,
      activeHubs: "All",
    });

    const unauthenticatedSubscription = uiRtcSubscription.Chat.AuthenticatedMessage(() => {
      receivedProtectedResponse = true;
    });

    await uiRtcCommunication.Chat.SendAuthenticatedMessage({
      message: "This unauthenticated call should not receive a protected response",
    });

    await new Promise((resolve) => window.setTimeout(resolve, 900));
    unauthenticatedSubscription.unsubscribe();
    await uiRtc.disposeAsync("All");

    setUnauthenticatedStatus(
      receivedProtectedResponse
        ? "Unexpected protected response received"
        : "No protected response received without a token"
    );
  };

  return (
    <main className="shell">
      <section className="panel">
        <div className="topline">
          <div>
            <h1>UiRtc JWT Chat</h1>
            <p>{status}</p>
          </div>
          {user && (
            <button className="secondary" onClick={logoutAsync}>
              Sign out
            </button>
          )}
        </div>

        {!user ? (
          <div className="login-grid">
            <form
              className="login-form"
              onSubmit={(event) => {
                event.preventDefault();
                void loginAsync();
              }}
            >
              <label>
                User name
                <input
                  value={credentials.userName}
                  onChange={(event) =>
                    setCredentials((current) => ({
                      ...current,
                      userName: event.target.value,
                    }))
                  }
                />
              </label>
              <label>
                Password
                <input
                  type="password"
                  value={credentials.password}
                  onChange={(event) =>
                    setCredentials((current) => ({
                      ...current,
                      password: event.target.value,
                    }))
                  }
                />
              </label>
              <button type="submit">Sign in</button>
            </form>

            <div className="quick-logins">
              <button onClick={() => void loginAsync("demo1", "demo1")}>Demo User 1</button>
              <button onClick={() => void loginAsync("demo2", "demo2")}>Demo User 2</button>
              <button className="secondary" onClick={() => void tryUnauthenticatedCallAsync()}>
                Try without token
              </button>
              {unauthenticatedStatus && <p>{unauthenticatedStatus}</p>}
            </div>
          </div>
        ) : (
          <div className="chat">
            <div className="composer">
              <input
                value={message}
                onChange={(event) => setMessage(event.target.value)}
                placeholder="Message"
              />
              <button onClick={() => void sendMessageAsync()}>Send</button>
            </div>

            <div className="message-list">
              {messages.map((item, index) => (
                <article className="message" key={`${item.connectionId}-${item.sentAtUtc}-${index}`}>
                  <strong>{item.userName}</strong>
                  <span>{item.userId}</span>
                  <p>{item.message}</p>
                  <small>Connection {item.connectionId}</small>
                </article>
              ))}
            </div>
          </div>
        )}
      </section>
    </main>
  );
}

export default App;
