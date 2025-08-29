'use strict';

const { functions } = require('../../../common');
const { forEachProcess } = require('./utils');


module.exports = function hideProgram(programName) {

	return forEachProcess(programName, hide);

	function hide(proc, processWindows, handle) {
		return functions.FMUser32Functions.ShowWindow(handle, 0);
	}
}