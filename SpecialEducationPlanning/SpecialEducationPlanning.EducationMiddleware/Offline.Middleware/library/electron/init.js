const { createFolders } = require('./folder');
const { emptyFolder } = require('./file');
const { constants, functions } = require('../common');

/**
 * Electron class init.
 */
class Init {

	constructor() {
		this.builder = require('./function/builder');
		this.functions = require('../common').functions;
		this.constants = require('../common').constants;
		this.builder();
	}

	/*
	 * Init function. Instanciate all system events.
	 * For the communication with the front application, uses EventHandShaking
	 */
	init() {
		try {
			this.functions.log.createLog();
			functions.log.info(`-----------------------------------------------------------------`);
			functions.log.info(`[OFFLINE-MIDDLEWARE STARTS]`);
			functions.log.info("[Offline-Middleware starts] - Checked and created: " + this.constants.APPLICATION_CONSTANTS.FOLDER_LOG);
			functions.log.info("[Offline-Middleware starts] - Create Folders");
			// createFolders(this.constants.APPLICATION_CONSTANTS.FILE_PATH_JSON);
			functions.log.info("[Offline-Middleware starts] - Checked and created: " + this.constants.APPLICATION_CONSTANTS.FILE_PATH_JSON);
			//this._emptyFiles();			
			functions.log.info(`[OFFLINE-MIDDLEWARE ENDS] please note that, maybe we have pending some async methods pending (depens of the pc you have)`);
			functions.log.info(`-----------------------------------------------------------------`);
		} catch (exception) {
			communication.send(exception);
			functions.log.error(`The open action throws ${JSON.stringify(exception)} for the plan ${model ? JSON.stringify(model) : 'no plan'}`);
		}
	}

	/**
	 * Empty MSG folder. 
	 */
	_emptyFiles() {
		emptyFolder(this.constants.APPLICATION_CONSTANTS.FILE_PATH_JSON);
	}
}
module.exports = Init;