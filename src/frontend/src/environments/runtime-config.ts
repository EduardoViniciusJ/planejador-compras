export interface RuntimeConfig {
  readonly apiBaseUrl?: string;
  readonly googleClientId?: string;
}

declare global {
  interface Window {
    planejadorConfig?: RuntimeConfig;
  }
}

export function getRuntimeConfig(): RuntimeConfig {
  return typeof window === 'undefined' ? {} : window.planejadorConfig ?? {};
}
