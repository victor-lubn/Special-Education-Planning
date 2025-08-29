const {
  app,
  BrowserWindow,
  Menu,
  ipcMain,
  shell,
  dialog,
  session,
  globalShortcut
} = require('electron');
const path = require('path');
// const url = require('url');
const fs = require('fs');
const os = require('os');
const {
  FMElectron
} = require('fusion-middleware');
const middleware = new FMElectron();
const {
  OMElectron
} = require('offline-middleware');
const offlineMiddleware = new OMElectron();
const {
  ThreeDCElectron
} = require('3dc-middleware');
const threeDCMiddleware = new ThreeDCElectron();
let threeDcToken = null;
// Import room planner events module
// const roomPlannerEvents = require('./room-planner-events');
function initRoomPlannerEvents(mainWindow) {
  // Initialize the 3DC middleware with the main window reference
  threeDCMiddleware.bootstrap(mainWindow);

  ipcMain.on('close-three-dc-window', function(event, data) {
    threeDCMiddleware.closePlannerWindow();
    mainWindow.maximize();
  });

  ipcMain.on('open-in-new-window', function(event, data) {
    threeDCMiddleware.createPlannerWindow(data.model, data.url, data.token, threeDcToken);
  });
  ipcMain.on('maximize-3dc-window', function(event, data) {
    if (threeDCMiddleware.childWindow && !threeDCMiddleware.childWindow.isDestroyed()) {
      mainWindow.minimize();
      threeDCMiddleware.childWindow.maximize();
      threeDCMiddleware.childWindow.focus();
    }
  });

  ipcMain.on('set-3dc-token', function(event, token) {
    // If the event came from a room planner window, send it to the main window
    if (event.sender !== mainWindow.webContents) {
      threeDcToken = token;
    }
  });
}

const appDataPath = (process.env.APPDATA || process.env.HOME + (process.platform == 'darwin' ? '/Library/Application Support' : '/.config')) + '/TDP';
const logFolderPath = appDataPath + '/logs';
const hostname = os.hostname();

let win;
let fusionIsOpen = true;
let isOffline = false;
const args = process.argv.slice(1);
const dev = args.some(val => val === '--dev');
let authSettings = {
  logoutRedirectURL: 'http://localhost:4200/#/login'
};

if (dev) {
  require('electron-reload')(__dirname);
}

function createWindow() {
    win = new BrowserWindow({
        show: false,
        backgroundColor: '#ffffff',
        icon: path.join(__dirname, 'assets/icons/Education_view_icon.ico'),
        webPreferences: {
            plugins: true,
            preload: path.join(__dirname, './preload.js'),
        }
    });
    initRoomPlannerEvents(win);
    // Handle external link opening
    ipcMain.handle('open-link-externally', async (event, url) => {
        try {
            await shell.openExternal(url);
            return { success: true };
        } catch (error) {
            return { success: false, error: error.message };
        }
    });

  const gotTheLock = app.requestSingleInstanceLock();

    if (!gotTheLock) {
        dialog.showMessageBox({
            type: 'error',
            title: 'Education View Error',
            message: 'Only one instance of Education View is allowed'
        }, function(response) {
            app.quit();
        });
    } else {
        var portscanner = require('portscanner');

        // Checks the status of a EducationView (Angular) port
        portscanner.checkPortStatus(4200, 'localhost', function(error, status) {
            // Status is 'open' if currently in use or 'closed' if available
            console.log(status);
            if (status === 'closed') {
                var connect = require('connect');
                var serveStatic = require('serve-static');
                connect().use(serveStatic(__dirname)).listen(4200, function() {
                    console.log('Server running on 4200...');
                });
                win.maximize();
                win.show();
                win.loadURL('http://localhost:4200');
                middleware.bootstrap(dev);
                offlineMiddleware.bootstrap();
            } else {
                dialog.showMessageBox({
                    type: 'error',
                    icon: path.join(__dirname, 'assets/icons/Education_view_icon.ico'),
                    title: 'Error',
                    message: 'Education View is already open on this PC.',
                    detail: 'This may be because you have shared usage (with a colleague) on this PC and they already have Education View open. You can only run one session of Education View at a time. Please either use another PC to open Education View or end the first user session on this PC.'
                }, function(response) {
                    app.quit();
                });
            }
        });
        // ipcMain.once('electron-startup-middleware', bootstrapMiddleware);
    };

    const menu = Menu.buildFromTemplate([{
            label: 'View',
            submenu: [{
                    role: 'reload'
                },
                {
                    role: 'forcereload'
                },
                {
                    role: 'toggledevtools'
                },
                {
                    type: 'separator'
                },
                {
                    role: 'resetzoom'
                },
                {
                    role: 'zoomin'
                },
                {
                    role: 'zoomout'
                },
                {
                    type: 'separator'
                },
                {
                    role: 'togglefullscreen'
                }
            ]
        },
        {
            role: 'window',
            submenu: [{
                    role: 'minimize'
                },
                {
                    role: 'close'
                }
            ]
        },
        {
            label: 'Logs',
            submenu: [{
                label: 'Show logs folder',
                click: showLogsFolderHandler
            }]
        },
        {
            label: 'Others',
            submenu: [{
                label: 'Show PC name',
                click: showHostnameDialog
            }]
        }
    ]);

    if (dev) {
        Menu.setApplicationMenu(menu);
        win.webContents.openDevTools();
    } else {
        Menu.setApplicationMenu(null);
        // TODO REMOVE !!!!
      win.webContents.openDevTools();
        //
    }

    session.defaultSession.webRequest.onHeadersReceived(
        (details, callback) => {
            if (details.url.includes(`/oauth2/authorize?response_type=id_token&client_id=`)) {
                if (details.responseHeaders.location && details.responseHeaders.location.length > 0)
                    oauthLoginCallbackRedirectHandler(details.responseHeaders.location[0]);
            }
            callback({
                cancel: false
            });
        });
    win.webContents.on('will-navigate', oauthLogoutRedirectHandler);



    win.on('close', (event) => {
        if (fusionIsOpen) {
            event.preventDefault();
            const gotTheLock = app.requestSingleInstanceLock();
            if (gotTheLock) {
                middleware.closeFusion();
                fusionIsOpen = false;
            }
            setTimeout(() => {
                if (win && win.close) {
                    win.close();
                }
            }, 1500);
        }
    });

}

app.commandLine.appendSwitch('ignore-certificate-errors', 'true');

// TODO revert this, only for testing until 3DC ready
app.on('ready', () => {
    createWindow();

    // Register a shortcut to open DevTools in both dev and production modes
    globalShortcut.register('CommandOrControl+Shift+I', () => {
        if (win && !win.isDestroyed()) {
            win.webContents.toggleDevTools();
        }
    });
});
//

app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') {
        app.quit()
    }
});

// TODO revert this, only for testing until 3DC ready
app.on('will-quit', () => {
    globalShortcut.unregisterAll();
});
//

app.on('activate', () => {
    if (win === null) {
        createWindow();
    }
});

/* App Event listeners */
ipcMain.on('new-tdp-log', newTdpLogHandler);
ipcMain.on('get-hostname', function(event, arg) {
    event.returnValue = hostname;
});
ipcMain.on('electron-reload-middleware', (event, networkStatus) => {
    if (fusionIsOpen) {
        event.preventDefault();
        const gotTheLock = app.requestSingleInstanceLock();
        if (gotTheLock) {
            middleware.closeFusion();
            fusionIsOpen = false;
        }
    }
    bootstrapMiddleware(event, networkStatus);
    fusionIsOpen = true;
});

/* Event handlers */
function bootstrapMiddleware(event, networkStatus) {
    isOffline = !networkStatus;
    middleware.bootstrap(dev, isOffline);
    offlineMiddleware.bootstrap();
}

function showLogsFolderHandler() {
    if (fs.existsSync(logFolderPath)) {
        shell.openPath(logFolderPath);
    } else {
        dialog.showMessageBox({
            type: 'warning',
            title: 'Folder not found',
            message: 'Logs folder does not exist'
        });
    }
}

function newTdpLogHandler(event, newLog) {
    if (!fs.existsSync(appDataPath)) {
        fs.mkdirSync(appDataPath);
    }
    if (!fs.existsSync(logFolderPath)) {
        fs.mkdirSync(logFolderPath);
    }
    fs.appendFileSync(logFolderPath + `/${new Date().toISOString().slice(0, 10)}-tdp-logs.log`, newLog + '\n', 'utf8');
}

function showHostnameDialog() {
    dialog.showMessageBox({
        type: 'info',
        title: 'Show PC name',
        message: 'The name of your machine is ' + hostname
    });
}

function oauthLoginCallbackRedirectHandler(redirectUrl) {
    const oauthToken = redirectUrl.split("#")[1];
    if (oauthToken && oauthToken.includes('id_token')) {
        win.loadURL(`http://localhost:4200/#${oauthToken}`);
    }
}

function oauthLogoutRedirectHandler(event, nextUrl) {
    if (nextUrl.includes(`/oauth2/logout?`)) {
        event.preventDefault();
        win.loadURL(authSettings.logoutRedirectURL);
    }
}

