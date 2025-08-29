'use strict';

const { functions} = require('../../../common');
const {forEachProcess} = require('./utils');

module.exports = function focusProgram(programName) {
	return new Promise((resolve)=>{
		forEachProcess(programName, focus);
		function focus(proc, processWindows, handle){
			functions.log.info(`Show ${programName} with pid ${handle}`);
			//Show - 5 , Max - 3
			functions.FMUser32Functions.ShowWindow(handle, 5);
			functions.FMUser32Functions.ShowWindow(handle, 3);
			processWindows.focusWindow(proc);
			resolve(true);
		}
	});
}