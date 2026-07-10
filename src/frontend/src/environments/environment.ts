import { getRuntimeConfig } from './runtime-config';

const runtimeConfig = getRuntimeConfig();

export const environment = {
  apiBaseUrl: runtimeConfig.apiBaseUrl ?? '',
  googleClientId: runtimeConfig.googleClientId ?? '',
};
