import { getRuntimeConfig } from './runtime-config';

const runtimeConfig = getRuntimeConfig();

export const environment = {
  apiBaseUrl: runtimeConfig.apiBaseUrl ?? 'http://localhost:5005',
  googleClientId: runtimeConfig.googleClientId ?? '',
};
