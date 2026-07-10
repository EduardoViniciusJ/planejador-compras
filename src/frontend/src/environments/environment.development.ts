import { getRuntimeConfig } from './runtime-config';

const runtimeConfig = getRuntimeConfig();

export const environment = {
  apiBaseUrl: runtimeConfig.apiBaseUrl ?? 'https://localhost:7064',
  googleClientId: runtimeConfig.googleClientId ?? '',
};
