const {functions, constants} = require('../common');
const send = require('./communication');
const createFolder = require('./folder/folder');

/**
 * Electron class init.
 */
class Init {

	constructor() {
		this.builder = require('./function/builder');
		this.lib = require('../common').lib;
		this.functions = require('../common').functions;
		this.constants = require('../common').constants;
		this.builder();
	}

	init() {
		try {
			this.functions.log.createLog();
			createFolder(constants.APPLICATION_CONSTANTS.FOLDER_THREE_DC_EVENTS);
			functions.log.info('-----------------------------------------------------------------');
			functions.log.info('[THREE_DC-MIDDLEWARE STARTS]');
			functions.log.info('[THREE_DC-MIDDLEWARE ENDS]');
			functions.log.info('-----------------------------------------------------------------');
		} catch (exception) {
			send(constants.FRONT_EVENTS.SESSION_ERROR, exception);
			functions.log.error(`The open action throws ${JSON.stringify(exception)}`);
		}
	}
}

module.exports = Init;
