import { getConfiguration, getVersion, getHealthcheckTimeout, getGoogleAnalyticsToken, getGoogleAnalyticsMode, getCreatioEnvUrl } from './configuration';

export const environment = getConfiguration();
export const version = getVersion();
export const healthcheckTimeout = getHealthcheckTimeout();
export const googleAnalyticsToken = getGoogleAnalyticsToken();
export const googleAnalyticsMode = getGoogleAnalyticsMode();
export const creatioEnvUrl = getCreatioEnvUrl();