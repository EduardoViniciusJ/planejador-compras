import { Injectable, NgZone, inject } from '@angular/core';

import {
  GoogleAuthorizationCodeResult,
  GoogleCodeClient,
} from './google-identity.types';

const GOOGLE_IDENTITY_SCRIPT_SRC = 'https://accounts.google.com/gsi/client';

@Injectable({
  providedIn: 'root',
})
export class GoogleIdentityService {
  private readonly ngZone = inject(NgZone);
  private googleIdentityScriptPromise: Promise<void> | null = null;
  private googleLoginSetupPromise: Promise<void> | null = null;
  private googleCodeClient: GoogleCodeClient | null = null;
  private pendingAuthorizationCodeRequest:
    | {
        resolve: (code: string) => void;
        reject: (error: Error) => void;
      }
    | null = null;

  prepare(clientId: string): Promise<void> {
    const normalizedClientId = clientId.trim();

    if (!normalizedClientId) {
      return Promise.reject(new Error());
    }

    if (this.googleCodeClient) {
      return Promise.resolve();
    }

    if (this.googleLoginSetupPromise) {
      return this.googleLoginSetupPromise;
    }

    this.googleLoginSetupPromise = this.initialize(normalizedClientId);

    return this.googleLoginSetupPromise;
  }

  requestAuthorizationCode(): Promise<string> {
    if (!this.googleCodeClient || this.pendingAuthorizationCodeRequest) {
      return Promise.reject(new Error());
    }

    return new Promise<string>((resolve, reject) => {
      this.pendingAuthorizationCodeRequest = { resolve, reject };

      try {
        this.googleCodeClient?.requestCode();
      } catch {
        this.pendingAuthorizationCodeRequest = null;
        reject(new Error());
      }
    });
  }

  private async initialize(clientId: string): Promise<void> {
    try {
      await this.loadGoogleIdentityScript();

      if (!window.google?.accounts.oauth2) {
        throw new Error();
      }

      this.googleCodeClient = window.google.accounts.oauth2.initCodeClient({
        client_id: clientId,
        scope: 'openid email profile',
        ux_mode: 'popup',
        callback: (result) => this.handleAuthorizationCodeResult(result),
      });
    } catch (error) {
      this.googleLoginSetupPromise = null;
      throw error;
    }
  }

  private handleAuthorizationCodeResult(result: GoogleAuthorizationCodeResult): void {
    const pendingRequest = this.pendingAuthorizationCodeRequest;

    if (!pendingRequest) {
      return;
    }

    this.pendingAuthorizationCodeRequest = null;

    this.ngZone.run(() => {
      if (result.error || !result.code) {
        pendingRequest.reject(new Error());
        return;
      }

      pendingRequest.resolve(result.code);
    });
  }

  private loadGoogleIdentityScript(): Promise<void> {
    if (window.google?.accounts.oauth2) {
      return Promise.resolve();
    }

    if (this.googleIdentityScriptPromise) {
      return this.googleIdentityScriptPromise;
    }

    this.googleIdentityScriptPromise = new Promise<void>((resolve, reject) => {
      const existingScript = document.querySelector<HTMLScriptElement>(
        `script[src="${GOOGLE_IDENTITY_SCRIPT_SRC}"]`,
      );

      if (existingScript) {
        existingScript.addEventListener('load', () => resolve(), { once: true });
        existingScript.addEventListener('error', () => reject(new Error()), { once: true });
        return;
      }

      const script = document.createElement('script');
      script.src = GOOGLE_IDENTITY_SCRIPT_SRC;
      script.async = true;
      script.defer = true;
      script.onload = () => resolve();
      script.onerror = () => reject(new Error());

      document.head.appendChild(script);
    }).catch((error: unknown) => {
      this.googleIdentityScriptPromise = null;
      throw error;
    });

    return this.googleIdentityScriptPromise;
  }
}
