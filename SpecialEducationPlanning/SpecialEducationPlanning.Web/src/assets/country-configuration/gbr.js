GBR = {
    local: {
        text: "local",
        country: "GBR",
        production: false,
        appDynamics: false,
        ONLINE_API: 'http://localhost:19226/api',
        ONLINE_JOBS: 'http://localhost:19226/jobs',
        OFFLINE_API: '',
        EducationerWalletUrl: 'http://localhost:4201',
        fusionUserPrefix: 'CD',
        AuthSettings: {
            auth: {
                authority: 'https://login.microsoftonline.com/7b222d55-2e58-49ae-b44c-9e46db40788d',
                clientId: '4e704e14-7361-4fd8-95b0-1a8bcbeaafc3',
                redirectUri: 'http://localhost:4200',
                navigateToLoginRequestUrl: false
            },
            cache: {
                //cacheLocation: BrowserCacheLocation.SessionStorage,
                cacheLocation: 'sessionStorage',
                storeAuthStateInCookie: false
                //storeAuthStateInCookie: isIE, // set to true for IE 11. Remove this line to use Angular Universal
            }
        },
        Middleware: {
            /**
             * Active the module.
             */
            active: true,
            /**
             * Enable debug mode.
             */
            debug: false,
            /**
             * Enable debug pop-ups mode.
             */
            debugPopups: false,
            /**
             * Enable check licence.
             */
            checkLicence: true
        }
    },
    development: {
        text: "development",
        country: "GBR",
        production: false,
        appDynamics: false,
        ONLINE_API: 'https://tdp-dev.servdev.howdev.corp/api',
        ONLINE_JOBS: 'https://tdp-dev.servdev.howdev.corp/jobs',
        OFFLINE_API: '',
        EducationerWalletUrl: 'http://localhost:4201',
        fusionUserPrefix: 'CD',
        AuthSettings: {
            auth: {
                authority: 'https://login.microsoftonline.com/7b222d55-2e58-49ae-b44c-9e46db40788d',
                clientId: '4e704e14-7361-4fd8-95b0-1a8bcbeaafc3',
                redirectUri: 'http://localhost:4200',
                navigateToLoginRequestUrl: false
            },
            cache: {
                cacheLocation: 'sessionStorage',
                storeAuthStateInCookie: false
            }
        },
        Middleware: {
            /**
             * Active the module.
             */
            active: true,
            /**
             * Enable debug mode.
             */
            debug: false,
            /**
             * Enable debug pop-ups mode.
             */
            debugPopups: false,
            /**
             * Enable check licence.
             */
            checkLicence: true
        }
    },
    development_sec: {
        text: "development_sec",
        country: "GBR",
        production: false,
        appDynamics: false,
        ONLINE_API: 'https://tdp-dev-sec.servdev.howdev.corp/api',
        ONLINE_JOBS: 'https://tdp-dev-sec.servdev.howdev.corp/jobs',
        OFFLINE_API: '',
        EducationerWalletUrl: 'http://localhost:4201',
        fusionUserPrefix: 'CD',
        AuthSettings: {
            auth: {
                authority: 'https://login.microsoftonline.com/7b222d55-2e58-49ae-b44c-9e46db40788d',
                clientId: '4e704e14-7361-4fd8-95b0-1a8bcbeaafc3',
                redirectUri: 'http://localhost:4200',
                navigateToLoginRequestUrl: false
            },
            cache: {
                cacheLocation: 'sessionStorage',
                storeAuthStateInCookie: false
            }
        },
        Middleware: {
            /**
             * Active the module.
             */
            active: true,
            /**
             * Enable debug mode.
             */
            debug: false,
            /**
             * Enable debug pop-ups mode.
             */
            debugPopups: false,
            /**
             * Enable check licence.
             */
            checkLicence: true
        }
    },
    qa: {
        text: "qa",
        country: "GBR",
        production: false,
        appDynamics: false,
        ONLINE_API: 'https://tdp-qa.servdev.howdev.corp/api',
        ONLINE_JOBS: 'https://tdp-qa.servdev.howdev.corp/jobs',
        OFFLINE_API: '',
        EducationerWalletUrl: 'http://localhost:4201',
        fusionUserPrefix: 'CD',
        AuthSettings: {
            auth: {
                authority: 'https://login.microsoftonline.com/7b222d55-2e58-49ae-b44c-9e46db40788d',
                clientId: '4e704e14-7361-4fd8-95b0-1a8bcbeaafc3',
                redirectUri: 'http://localhost:4200',
                navigateToLoginRequestUrl: false
            },
            cache: {
                cacheLocation: 'sessionStorage',
                storeAuthStateInCookie: false
            }
        },
        Middleware: {
            /**
             * Active the module.
             */
            active: true,
            /**
             * Enable debug mode.
             */
            debug: false,
            /**
             * Enable debug pop-ups mode.
             */
            debugPopups: false,
            /**
             * Enable check licence.
             */
            checkLicence: true
        }
    },
    preproduction: {
        text: "preproduction",
        country: "GBR",
        production: false,
        appDynamics: true,
        ONLINE_API: 'https://tdp-preprod.services.aiep.corp/api',
        ONLINE_JOBS: 'https://tdp-preprod.services.aiep.corp/jobs',
        OFFLINE_API: '',
        EducationerWalletUrl: 'http://localhost:4201',
        fusionUserPrefix: 'CD',
        AuthSettings: {
            auth: {
                authority: 'https://login.microsoftonline.com/7b222d55-2e58-49ae-b44c-9e46db40788d',
                clientId: '4e704e14-7361-4fd8-95b0-1a8bcbeaafc3',
                redirectUri: 'http://localhost:4200',
                navigateToLoginRequestUrl: false
            },
            cache: {
                cacheLocation: 'sessionStorage',
                storeAuthStateInCookie: false
            }
        },
        Middleware: {
            /**
             * Active the module.
             */
            active: true,
            /**
             * Enable debug mode.
             */
            debug: false,
            /**
             * Enable debug pop-ups mode.
             */
            debugPopups: false,
            /**
             * Enable check licence.
             */
            checkLicence: true
        }
    },
    migration: {
        text: "migration",
        country: "GBR",
        production: false,
        appDynamics: false,
        ONLINE_API: 'https://tdp-migration.services.aiep.corp/api',
        ONLINE_JOBS: 'https://tdp-migration.services.aiep.corp/jobs',
        OFFLINE_API: '',
        EducationerWalletUrl: 'http://localhost:4201',
        fusionUserPrefix: 'CD',
        AuthSettings: {
            auth: {
                authority: 'https://login.microsoftonline.com/c7966bb5-26c0-4227-b26e-2c5fb17db44a',
                clientId: 'b9971835-b172-4116-a589-9891e59d6191',
                redirectUri: 'http://localhost:4200',
                navigateToLoginRequestUrl: false
            },
            cache: {
                cacheLocation: 'sessionStorage',
                storeAuthStateInCookie: false
            }
        },
        Middleware: {
            /**
             * Active the module.
             */
            active: true,
            /**
             * Enable debug mode.
             */
            debug: false,
            /**
             * Enable debug pop-ups mode.
             */
            debugPopups: false,
            /**
             * Enable check licence.
             */
            checkLicence: true
        }
    },
    production: {
        text: "production",
        country: "GBR",
        production: true,
        appDynamics: true,
        ONLINE_API: 'https://tdp.services.aiep.corp/api',
        ONLINE_JOBS: 'https://tdp.services.aiep.corp/jobs',
        OFFLINE_API: '',
        EducationerWalletUrl: 'http://localhost:4201',
        fusionUserPrefix: 'CD',
        AuthSettings: {
            auth: {
                authority: 'https://login.microsoftonline.com/c7966bb5-26c0-4227-b26e-2c5fb17db44a',
                clientId: 'b9971835-b172-4116-a589-9891e59d6191',
                redirectUri: 'http://localhost:4200',
                navigateToLoginRequestUrl: false
            },
            cache: {
                cacheLocation: 'sessionStorage',
                storeAuthStateInCookie: false
            }
        },
        Middleware: {
            /**
             * Active the module.
             */
            active: true,
            /**
             * Enable debug mode.
             */
            debug: false,
            /**
             * Enable debug pop-ups mode.
             */
            debugPopups: false,
            /**
             * Enable check licence.
             */
            checkLicence: true
        }
    }
}
