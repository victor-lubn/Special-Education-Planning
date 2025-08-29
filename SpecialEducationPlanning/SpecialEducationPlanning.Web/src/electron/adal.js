

const EVENT_NAME_NAVIGATE = 'will-navigate';
const EVENT_NAME_REDIRECT = 'will-redirect';
const URL_REDIRECT = 'https://login.microsoftonline.com/7b222d55-2e58-49ae-b44c-9e46db40788d/oauth2/logout?post_logout_redirect_uri=file';
const URL_LOCAL = 'http://localhost:4200/#id_token';
const path = require('path');
const URL_LOGOUT = "https://login.microsoftonline.com/7b222d55-2e58-49ae-b44c-9e46db40788d/oauth2/logout?post_logout_redirect_uri=http://localhost:4200/#/login/callback#";


function willNavigate(win, url, authSettings) {
    const session = win.webContents.session
    const contents = win.webContents;

    contents.on(EVENT_NAME_NAVIGATE, (event, nextUrl) => {
        if (nextUrl.startsWith(URL_REDIRECT)) {
            event.preventDefault();
            //session.clearStorageData({ storages: ['cookies'] });
            //_deleteCookies(session, ()=>{
            _oauthLogoutRedirectHandler(event, win, url);
            //});
        }
    });
}

//const variables = ['CKtst','x-ms-gateway-slice','stsservicecookie','AADSSO','SSOCOOKIEPULLED','esctx','wlidperf','MSCC','ESTSAUTHLIGHT','ESTSSC','ESTSAUTHPERSISTENT','ESTSAUTH','buid','SignInStateCookie','fpc']
const variables = ['ESTSAUTHLIGHT', 'ESTSSC', 'ESTSAUTH', 'SignInStateCookie'];
//const variables = ['AADSSO','SSOCOOKIEPULLED','MSCC','ESTSAUTHLIGHT','ESTSSC','ESTSAUTH','SignInStateCookie']

function _deleteCookies(session, fn) {
    session.cookies.get({}, (error, cookies) => {
        if (cookies && cookies.length) {
            cookies.forEach((cookie, index) => {
                if (!variables.includes(cookie.name)) {
                    session.cookies.remove(_getUrl(cookie), cookie.name, _error);
                }
            });
        }
        fn(); 
    });
}
function _getUrl(cookie) {
    let url = '';
    // get prefix, like https://www.
    url += cookie.secure ? 'https://' : 'http://';
    url += cookie.domain.charAt(0) === '.' ? 'www' : '';
    // append domain and path
    url += cookie.domain;
    url += cookie.path;
    return url;
}
function _error(error) {
    console.log(error);
}

function willRedirect(win, url) {
    win.webContents.on(EVENT_NAME_REDIRECT, (event, nextUrl) => {
        if (nextUrl.startsWith(URL_LOCAL)) {
            event.preventDefault();
            const oauthToken = nextUrl.split("#")[1];
            if (oauthToken && oauthToken.includes('id_token')) {
                win.loadURL(`http://localhost:4200/#/login/callback?${oauthToken}`);
            }
        }
    });

    win.webContents.on('did-finish-load', (event, nextUrl) => {
        console.log(nextUrl);
    });
    win.webContents.on('did-redirect-navigation', (event, nextUrl) => {
        console.log(nextUrl);
    });
    win.webContents.on('did-navigate', (event, nextUrl) => {
        if (nextUrl.startsWith(URL_LOGOUT + "a")) {
            event.preventDefault();
            setTimeout(() => {
                win.loadURL(url.format({
                    pathname: path.join(__dirname, "index.html"),
                    protocol: 'file',
                    slashes: true,
                    hash: `/login`
                }));
            }, 300);
        }
    });
}

function _oauthLogoutRedirectHandler(event, win, url) {
    event.preventDefault();
    win.loadURL(URL_LOGOUT + "a");
    // win.loadURL(url.format({
    //     pathname: path.join(__dirname, "index.html"),
    //     protocol: 'file',
    //     slashes: true   
    // }));
}

module.exports = function (win, url, authSettings) {
    willNavigate(win, url, authSettings);
    willRedirect(win, url);
}