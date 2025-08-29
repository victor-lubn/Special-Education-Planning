'use strict';

const { lib, constants, functions } = require('../../../common');
const communication = require('../../communication');
var exec = require('child_process').execFile;

let noteProcess = null;

/**
 * Creates the main process to open the application and add the appropriate events.
 * 
 * @param {String} path The file path.
 */
function createProccess(path, execName) {
	if (path !== '') {
		noteProcess = exec(execName, [], { cwd: path },errorFn);
		functions.log.info(`Fusion from ${path} \\ ${execName} has been launched.`);
		addProcessEvents(noteProcess);
		functions.log.info('Events added.');
		return noteProcess;
	}
}

/**
 * 
 */
function closeProcess(){
	if(noteProcess && noteProcess.kill){
		noteProcess.kill();
		noteProcess = null;
	}
}
/**
 * Adds file events.
 * 
 * @param {Object} noteProcess The process object.
 */
function addProcessEvents(noteProcess) {
	noteProcess.stderr.on('data', (arg) => {
		functions.log.error('Fusion has sent this error data', arg);
		communication.send(constants.FRONT_EVENTS.ERROR_OPEN_DOCUMENT, 'ERROR_OPEN_DOCUMENT');
		functions.log.error('The program close all plans', arg);
	});
	noteProcess.on('data', (arg) => {
		functions.log.info('Fusion has sent this data', arg);
		communication.send(constants.FRONT_EVENTS.DATA_DOCUMENT, 'DATA_DOCUMENT');
	});

	noteProcess.on('close', (arg) => {
		functions.log.info('Fusion has been closed.', arg);
		communication.send(constants.FRONT_EVENTS.CLOSE_DOCUMENT, null);
		//documentUtils.closeAll(model.planCode);
	});
	noteProcess.on('exit', (arg) => {
		functions.log.info('Fusion has been exit.', arg);
		//let updateModel = documentUtils.getFileCurrentStatus(model);
		communication.send(constants.FRONT_EVENTS.EXIT_APPLICATION, 'Fusion has been closed');

	});

	noteProcess.on('error', (arg) => {
		functions.log.info('Fusion has an Error.', arg);
		//let updateModel = documentUtils.getFileCurrentStatus(model);
		communication.send(constants.FRONT_EVENTS.EXIT_APPLICATION, 'Fusion has been closed');

	});
}

/**
 * Adds the necessary watchers to launch the events. 
 * 
 * @param {String} path The file path.
 */
function addWatcher(path) {
	return lib.watch(path, { recursive: true }, function (evt, name) {
		if (evt === 'update') {
			communication.send(constants.FRONT_EVENTS.UPDATE_DOCUMENT, 'UPDATE_DOCUMENT');
		}
		if (evt == 'remove') {
			communication.send(constants.FRONT_EVENTS.REMOVE_DOCUMENT, 'REMOVE_DOCUMENT');
		}
	});
}

/**
 * Error function.
 * @param {String} error The error message.
 * @param {Object} stdout The pipe.
 */
function errorFn(error, stdout) {
	if (error) {
		functions.log.error(`Fusion has been sent this exec error: ${error}`);
		communication.send(constants.FRONT_EVENTS.ERROR_OPEN_DOCUMENT, `exec error: ${error}`);
		return;
	}
	if (stdout){

		try{
			stdout.on('exit', () => {
				functions.log.info('Fusion has exit');
				communication.send(constants.FRONT_EVENTS.EXIT_APPLICATION, 'EXIT_APPLICATION');
			});
		}
		catch (error) {
			console.log(error, error.stack);
		  }
		  
	}
	
}


		

/**
 * Exports
 */
module.exports = {
	addWatcher, createProccess, closeProcess
};