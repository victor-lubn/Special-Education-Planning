'use strict';

const { constants, processWindows } = require('../../common');
const { createProccess } = require('../../electron/entities/process');
const { exec } = require('child_process');

function check() {
	return new Promise((resolve)=>{
		const command = 'taskkill /F /IM \"' + constants.APPLICATION_CONSTANTS.FUSION_NAME + '.exe\" ' +
										'& taskkill /F /IM ' + constants.APPLICATION_CONSTANTS.FUSION_NAME_APP;
		exec(command, (err, stdout, stderr) => {
			processWindows.getProcesses(function (err, processes) {
				if(!processes){
					return;
				}
				var process = processes.filter(p => p.mainWindowTitle.indexOf(constants.APPLICATION_CONSTANTS.FUSION_NAME) >= 0);
				if (process.length == 0) {
					createProccess(constants.APPLICATION_CONSTANTS.FUSION_PATH, constants.APPLICATION_CONSTANTS.FUSION_NAME_APP);
				}
				resolve(true);
			});
		});
	});
}

module.exports = { check }