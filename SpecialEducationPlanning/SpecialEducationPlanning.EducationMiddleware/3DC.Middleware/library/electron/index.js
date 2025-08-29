'use strict';

const _bootstrap = require('./init');
const { lib, functions, constants } = require('../common');
const send = require('./communication');
const placeEventFile = require('./function/fn/placeEventFile');

/**
 * Electron class.
 */
class Electron {

	/**
     * Default constructor.
     */
	constructor() {
		this._bootstrap = new _bootstrap();
		this.childWindow = null;
		this.mainWindow = null;
		this.threeDcToken = null;
		this.model = null;
		this.plannerWindowInitialized = false;
	}

	closePlannerWindow() {
		if (this.childWindow) {
			this.childWindow.close();
			this.childWindow = null;
		}
		this.threeDcToken = null;
		this.model = null;
	}

	maximizePlannerWindow() {
		if (this.childWindow) {
			this.childWindow.maximize();
		}
	}

	bootstrap(mainWindow) {
		this._bootstrap.init();
		this.mainWindow = mainWindow;
	}

	createPlannerWindow(model, url, threeDcToken, EducationViewToken) {
		if (this.childWindow) {
			send(constants.FRONT_EVENTS.PLANNER_WINDOW_ALREADY_OPENED, model);
			return;
		}
		send(constants.FRONT_EVENTS.PLANNER_WINDOW_INITIALIZATION_STARTED, {});
		try {
			if (!model) {
				throw new Error('Model is required to create a planner window');
			}
			this.model = model;
			this.threeDcToken = threeDcToken;

			this.childWindow = new lib.BrowserWindow({
				show: true,
				webPreferences: {
					nodeIntegration: false,
					contextIsolation: true,
					preload: lib.path.join(__dirname, '../preload.js')
				}
			});

			this.childWindow.maximize();
			this.childWindow.openDevTools();

			this.subscribeToEvents();

			this.childWindow.webContents.on('did-stop-loading', () => {
				// TODO NOT ONLY FOR SHOWING THE SPINNER! need timeout minimum 2 ms, reason: onCompleted event runs after this (handle 404)
				setTimeout(() => {
					if (this.childWindow) {
						this.plannerWindowInitialized = true;
						this.childWindow.webContents.send('loaded');
						send(constants.FRONT_EVENTS.CHILD_WINDOW_RESPONSIVE, {arg: 'Window created'});
						// this.mainWindow.minimize();
					}
					// this.maximizePlannerWindow();
				}, 10);
				// process.nextTick(() => {
				// 	if (this.childWindow) {
				// 		this.plannerWindowInitialized = true;
				// 		this.childWindow.webContents.send('loaded');
				// 		send(constants.FRONT_EVENTS.CHILD_WINDOW_RESPONSIVE, {arg: 'Window created'});
				// 	}
				// 	this.maximizePlannerWindow();
				// });
				lib.con('Planner window loaded successfully');
			});
			const baseUrl = `https://aiep-cert.3dcloud.io/?planid=${model.planId}`;
			// const baseUrl = `http://localhost:3000/redirect?planid=${model.planId}`;
			const urlTemp = model.versionNotes && model.versionNotes.trim()
  				? `${baseUrl}&uuid=${encodeURIComponent(model.versionNotes)}`
  				: baseUrl;

			// const urlTemp = 'http://localhost:3000/redirect';

			// for testing purposes
			// const urlTemp = 'http://localhost:4567';

			this.childWindow.webContents.session.webRequest.onCompleted((details) => {
				if (details.statusCode === 404 && !this.plannerWindowInitialized) {
					send(constants.FRONT_EVENTS.CHILD_WINDOW_UNRESPONSIVE, {close3DCImmediately: true});
				}

				if (details.url.includes('/authenticate')) {

					const responseHeaders = details.responseHeaders;

					if (responseHeaders && responseHeaders['authorization']) {
						this.threeDcToken = responseHeaders['authorization'][0];
					}
				}
				if (details.url.includes('/files')) {
					//const formData = details.requestBody.formData;
					send(constants.FRONT_EVENTS.PLAN_CREATED, { planId : model.planId, EducationTool3DCPlanId: '7a8c166f-4329-4126-8e48-92b63a450f24' });
					this.childWindow.destroy();
				}
			});

			this.childWindow.loadURL(urlTemp);
			// Open DevTools in development mode
			if (process.env.NODE_ENV === 'development') {
				this.childWindow.webContents.openDevTools();
			}

			// Handle window close
			this.childWindow.on('closed', () => {
				this.childWindow = null;
				this.mainWindow.maximize();
			});
			this.childWindow.on('unresponsive', () => {
				if (this.mainWindow && !this.mainWindow.isDestroyed()) {
					send(constants.FRONT_EVENTS.CHILD_WINDOW_UNRESPONSIVE, {});
					functions.log.error(`3DC planner window is unresponsive, plan id: ${this.model.planId}, version: ${this.model.versionNumber}`);
				}
			});

			this.childWindow.on('responsive', () => {
				if (this.mainWindow && !this.mainWindow.isDestroyed()) {
					send(constants.FRONT_EVENTS.CHILD_WINDOW_RESPONSIVE, {});
				}
			});

		} catch (exception) {
			lib.con(exception);
			send(constants.FRONT_EVENTS.CHILD_WINDOW_UNRESPONSIVE, {});
			lib.con(`Error creating planner window: ${JSON.stringify(exception)}`);
		}
	}

	subscribeToEvents() {
		for (const property in constants.FRONT_EVENTS) {
			const eventName = constants.FRONT_EVENTS[property];
			lib.ipcMain.removeAllListeners(eventName);
			lib.ipcMain.on(eventName, (event, arg) => {
				if (eventName === constants.FRONT_EVENTS.SESSION_ERROR) {
					placeEventFile(eventName + '_' + Date.now(), arg);
				}
				send(constants.FRONT_EVENTS[property], {
					event: arg,
					model: this.model
				});
			});
		}
	}
}



module.exports = Electron;

