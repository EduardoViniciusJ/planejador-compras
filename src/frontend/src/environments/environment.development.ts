import { getRuntimeConfig } from './runtime-config';

const runtimeConfig = getRuntimeConfig();

export const environment = {
  apiBaseUrl: runtimeConfig.apiBaseUrl ?? 'http://localhost:5005',
  googleClientId:
    runtimeConfig.googleClientId ??
    '531131914424-ifs3rndk9u7te8ti7l6lrmfqo7p7l6gi.apps.googleusercontent.com',
};
