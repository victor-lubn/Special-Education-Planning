'use strict';

var exports = module.exports = {};

const { processWindows, functions } = require('../../../common');

var PROCESSES = [];
var HANDLES = {};

exports.forEachProcess = function (programName, action) {
	return new Promise((resolve) => {
		startProcess().then((Processes) => {
			let responses = [];
			var process = Processes.filter(p => p.processName.indexOf(programName) >= 0);
			if (process.length && process.length > 0) {
				process.forEach(proc => {
					responses.push(action(proc, processWindows, getHandle(proc)));
				});
			}
			resolve(responses);
		});
	});
}

function getHandle(proc) {
	let handle = 0;
	if (proc.mainWindowTitle) {
		handle = functions.FMUser32Functions.FindWindowA(null, proc.mainWindowTitle);
	} else {
		handle = functions.FMUser32Functions.FindWindowA(null, proc.processName);
	}
	if (handle != 0) {
		HANDLES[proc.pid] = handle;
	} else {
		handle = HANDLES[proc.pid];
	}
	return handle;
}


function startProcess() {
	return new Promise((resolve) => {
		processWindows.getProcesses(function (err, processes) {
			PROCESSES = processes;
			resolve(PROCESSES);
		});
	});
}