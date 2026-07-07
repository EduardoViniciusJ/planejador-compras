import { environment } from '../../../environments/environment';

const normalizedApiBaseUrl = normalizeApiBaseUrl(environment.apiBaseUrl);

export function buildApiUrl(path: `/${string}`): string {
  return `${normalizedApiBaseUrl}${path}`;
}

export function isApiUrl(url: string): boolean {
  if (!normalizedApiBaseUrl) {
    return url.startsWith('/api/');
  }

  return url.startsWith(`${normalizedApiBaseUrl}/api/`);
}

function normalizeApiBaseUrl(apiBaseUrl: string): string {
  const value = apiBaseUrl.trim();

  if (!value) {
    return '';
  }

  try {
    const url = new URL(value);

    if (url.protocol !== 'http:' && url.protocol !== 'https:') {
      throw new Error('Unsupported API protocol.');
    }
  } catch {
    throw new Error(`Invalid API base URL: ${apiBaseUrl}`);
  }

  return value.replace(/\/+$/, '');
}
