import { PublishTypeValues } from '../app/shared/models/app-enums';
import { PublishTypeOption } from '../app/shared/models/publish-type-option';

declare global {
  interface Window { ELECTRON_ENVIROMENTS: any; }
  interface Window { VERSION: any; }
  interface Window { HEALTHCHECK_TIMEOUT: any; }
  interface Window { GOOGLE_ANALYTICS_TOKEN: any; }
  interface Window { GOOGLE_ANALYTICS_MODE: boolean; }
  interface Window { CREATIO_ENV_URLS: any; }
  interface Window { THREE_DC_URL: any; }
  interface Window { THREE_DC_TIMEOUT: any; }
  interface Window { THREE_DC_PUBLISH_TYPES: PublishTypeOption[]; }
}

/**
 * Please check the './assets/configuration.js' file
 * Through the country_env varialbe we will be able to select the country.
 * Through the active_env will be able to select the enviroment. This way, executing the command 'ng build --prod' makes no sense.
 * If you want to change the country environment ot active environemnt, you must change the country_env and active_env.
 */
export function getConfiguration() {
  return window.ELECTRON_ENVIROMENTS? window.ELECTRON_ENVIROMENTS[window.ELECTRON_ENVIROMENTS.active] : {};
}

export function getVersion() {
  return  window.VERSION || {};
}
export function get3DCUrl() {
  return  window.THREE_DC_URL || 'test';
}

export function getHealthcheckTimeout() {
  return window.HEALTHCHECK_TIMEOUT || {};
}

export function getGoogleAnalyticsToken(){
  return window.GOOGLE_ANALYTICS_TOKEN || {};
}

export function getGoogleAnalyticsMode() : boolean{
  return window.GOOGLE_ANALYTICS_MODE || false;
}

export function getCreatioEnvUrl(): string {
  const activeEnv = window.ELECTRON_ENVIROMENTS?.active?.toUpperCase();
  return window.CREATIO_ENV_URLS?.[activeEnv] || "";
}

export function get3DCTimeout(): number {
  return window.THREE_DC_TIMEOUT || 10000;
}

export function get3DCPublishTypes(): PublishTypeOption[] {
  return window.THREE_DC_PUBLISH_TYPES || [{translationKey: 'publishType3DCImage', value: PublishTypeValues.PictureHd}];
}
