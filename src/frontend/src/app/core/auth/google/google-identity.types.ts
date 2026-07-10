export interface GoogleAuthorizationCodeResult {
  readonly code?: string;
  readonly error?: string;
}

export interface GoogleCodeClientConfig {
  readonly client_id: string;
  readonly scope: string;
  readonly ux_mode?: 'popup' | 'redirect';
  readonly callback: (response: GoogleAuthorizationCodeResult) => void;
}

export interface GoogleCodeClient {
  requestCode(): void;
}

export interface GoogleOAuth2 {
  initCodeClient(config: GoogleCodeClientConfig): GoogleCodeClient;
}

export interface GoogleIdentityGlobal {
  readonly accounts: {
    readonly oauth2: GoogleOAuth2;
  };
}

declare global {
  interface Window {
    google?: GoogleIdentityGlobal;
  }
}
