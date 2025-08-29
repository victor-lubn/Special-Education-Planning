'use strict';

const { processWindows } = require('../../../common');

function focusProgram(programName) {
	//https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/tasklist
	//We can not use FindWindowA in user32 library because the title name of fusion changes when a .Rom is loaded..
	processWindows.getProcesses(function(err, processes) {
		var process = processes.filter(p => p.mainWindowTitle.indexOf(programName) >= 0);
		if(process.length > 0) {
			processWindows.focusWindow(process[0]);
		}
	});
}

module.exports = {
	focusProgram
};