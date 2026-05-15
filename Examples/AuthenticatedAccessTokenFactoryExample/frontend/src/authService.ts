const accessTokenKey = "uiRtc.authenticatedExample.accessToken";

export type AuthenticatedUser = {
  userId: string;
  userName: string;
};

export type LoginResponse = AuthenticatedUser & {
  accessToken: string;
};

export const authService = {
  getAccessToken: () => localStorage.getItem(accessTokenKey) ?? "",
  setAccessToken: (accessToken: string) => localStorage.setItem(accessTokenKey, accessToken),
  clearAccessToken: () => localStorage.removeItem(accessTokenKey),
};
