'use strict';

const logger = require('simple-node-logger');
var logError = {}, logInfo ={};
let constants  = require('../constants');

function createLog(){
	const optsError = {
		errorEventName:'fatal',
		logDirectory: constants.APPLICATION_CONSTANTS.FOLDER_LOG, // NOTE: folder must exist and be writable...
		fileNamePattern:'error-<DATE>.log',
		dateFormat:'YYYY.MM.DD'
	};
	const optsInfo = {
		errorEventName:'info',
		logDirectory:constants.APPLICATION_CONSTANTS.FOLDER_LOG, // NOTE: folder must exist and be writable...
		fileNamePattern:'info-<DATE>.log',
		dateFormat:'YYYY.MM.DD'
	};

	logError = logger.createRollingFileLogger(optsError);
	logInfo = logger.createRollingFileLogger(optsInfo);
}

function war(...arg) {
	console.log(arg);
}


function error(...arg) {
	console.log(arg);
	logError.fatal(arg);
}

function info(...arg) {
	console.log(arg);
	logInfo.info(arg);
}

module.exports = {
	error,
	info,
	war,
	createLog
};