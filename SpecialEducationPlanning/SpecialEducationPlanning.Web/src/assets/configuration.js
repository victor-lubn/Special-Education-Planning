const country_env = {
  GBR,
  IRL,
  FRA,
};

const active_env = {
  local: "local",
  development: "development",
  development_sec: "development_sec",
  qa: "qa",
  preproduction: "preproduction",
  migration: "migration",
  production: "production",
};

window.ELECTRON_ENVIROMENTS = country_env.GBR;
window.ELECTRON_ENVIROMENTS.active = active_env.local;

window.VERSION = {
  tdp: "6.1.0",
  commit: "__Build.SourceVersion__",
};

const google_analytics_mode = {
  enabled: true,
  disabled : false,
}

const google_analytics_tracking_tag = {
  GBR: "G-VVH25BX5L7",
  IRL : "G-WS53LKCPS0",
  FRA : "G-JZLWW99TY3",
  NA : ""
}

//Google analytics is disabled by default
//Only needs enabling when creating the front-end package for a Production deployment

//Uncomment the GA tracking tags as needed
//window.GOOGLE_ANALYTICS_TOKEN = google_analytics_tracking_tag.GBR;
//window.GOOGLE_ANALYTICS_TOKEN = google_analytics_tracking_tag.IRL;
//window.GOOGLE_ANALYTICS_TOKEN = google_analytics_tracking_tag.FRA;
window.GOOGLE_ANALYTICS_TOKEN = google_analytics_tracking_tag.NA;
//Set this to enabled when deploying to Production
//window.GOOGLE_ANALYTICS_MODE= google_analytics_mode.enabled;
window.GOOGLE_ANALYTICS_MODE = google_analytics_mode.disabled;

const appdynamics_key = {
  PREPROD_GBR: "EC-AAB-TUK",
  PROD_GBR: "EC-AAB-DGT",
  PREPROD_IRL : "EC-AAB-UAW",
  PROD_IRL : "EC-AAB-TWB",
  PREPROD_FRA : "EC-AAB-TZW",
  PROD_FRA : "EC-AAB-TZX",
  NA : ""
}

const creatio_env_urls = {
  PREPRODUCTION: "pre-aiep.creatio.com",
  PRODUCTION: "aiep.creatio.com",
  QA: "stage-aiep.creatio.com",
  LOCAL: "stage-aiep.creatio.com",
  DEVELOPMENT: "stage-aiep.creatio.com",
  MIGRATION: "stage-aiep.creatio.com",
  DEVELOPMENT_SEC: "stage-aiep.creatio.com"
}

window.CREATIO_ENV_URLS = creatio_env_urls;

window.APP_DYNAMICS_KEY = appdynamics_key.NA;

window.HEALTHCHECK_TIMEOUT = 10000;

window.THREE_DC_CLOUD = 'test';

window.THREE_DC_TIMEOUT = 10000;

window.THREE_DC_PUBLISH_TYPES = [{translationKey: 'pictureHd', value: 2}];
