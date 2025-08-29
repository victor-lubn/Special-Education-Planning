const { createFolders } = require('./entities/folder');
const setFusionConfig = require('./entities/fusionConfig');
const { emptyFolder } = require('./entities/file');
const { checksFusion } = require('./entities/fusion');
const fusion = require('../fusion/function/checkOpenProgram').check;
const hideFusion = require('../fusion/function/hideFusion');
const hideFusionController = require('../fusion/function/hideFusionController');
const log = require('../common').functions.log;
const licence = require('./function/fn/licence');
const cleanFiles = require('./function/fn/clean');
const communication = require('./communication');
const { constants, functions } = require('../common');

/**
 * Electron class init.
 */
class Init {

	constructor() {
		this.EventHandShaking = null;
		this.lib = require('../common').lib;
		this.functions = require('../common').functions;
		this.constants = require('../common').constants.APPLICATION_CONSTANTS;
		this.builder = require('./function/builder');
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
			functions.log.info(`[MIDDLEWARE STARTS]`);
			functions.log.info("[Middleware starts] - Checked network status and PPPrefs created");
			setFusionConfig();
			functions.log.info("[Middleware starts] - Checked and created: " + this.constants.FOLDER_LOG);			
			functions.log.info("[Middleware starts] - Create Folders");
			createFolders(this.constants.PATH_DOCUMENT_BASE);
			functions.log.info("[Middleware starts] - Checked and created: " + this.constants.PATH_DOCUMENT_BASE);
			createFolders(this.constants.FILE_PATH);
			functions.log.info("[Middleware starts] - Checked and created: " + this.constants.FILE_PATH);
			createFolders(this.constants.FOLDER_PATH_WATCHER);
			functions.log.info("[Middleware starts] - Checked and created: " + this.constants.FOLDER_PATH_WATCHER);
			createFolders(this.constants.FOLDER_PATH_TEMP);
			functions.log.info("[Middleware starts] - Checked and created: " + this.constants.FOLDER_PATH_TEMP);
			createFolders(this.constants.FOLDER_LOG);
			functions.log.info("[Middleware starts] - Checked and deletedd empty files: ");
			this._emptyFiles();
			if (checksFusion()) {
				licence(null, (result) => {
					if (result) {
						this._launchFusionExec();
					}
				}, false);
				functions.log.info("[Middleware starts] - The cleanup begins");
				cleanFiles();
				functions.log.info("[Middleware starts] - The cleanup ends");
			}
			functions.log.info(`[MIDDLEWARE ENDS] please note that, maybe we have pending some async methods pending (depens of the pc you have)`);
			functions.log.info(`-----------------------------------------------------------------`);
		} catch (exception) {
			communication.send(constants.FRONT_EVENTS.ERROR_OPEN_DOCUMENT, exception);
			functions.log.error(`The open action throws ${JSON.stringify(exception)} for the plan ${model?JSON.stringify(model):'no plan'}`);
		}
	}

	/**
	 * Empty MSG folder. 
	 */
	_emptyFiles() {
		emptyFolder(this.constants.FILE_PATH);
	}

	/**
	 * Launch Fusion.
	 */
	_launchFusionExec() {
		let _this = this;
		fusion().then(() => {
			setTimeout(() => {
				_this._launchProcess();
				hideFusionController();
			}, 3000);
		});
	}

	_launchProcess() {
		let _this = this;
		setTimeout(() => {
			log.info('Hide Fusion');
			hideFusion().then(result => {
				if (!result.includes(true)) {
					_this._launchProcess();
				}
			});

		}, 1000);
	}
}
module.exports = Init;