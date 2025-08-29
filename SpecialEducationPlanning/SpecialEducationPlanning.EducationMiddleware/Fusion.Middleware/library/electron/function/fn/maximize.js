'use strict';

const { functions } = require('../../../common');
const { forEachProcess } = require('./utils');


module.exports = function minimizeProgram(programName) {

	forEachProcess(programName, minimize);

	function minimize(proc, processWindows, handle) {
		if (handle) {
			functions.log.info(`Maximize ${programName} with pid ${handle}`);
			functions.FMUser32Functions.ShowWindow(handle, 3);	
		}
	}
}