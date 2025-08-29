'use strict';
const { lib, constants, functions } = require('../../../common');
const communication = require('../../communication');

function recoverPlanAutosave(event, planNumber) {
	let tempFolder = constants.APPLICATION_CONSTANTS.FOLDER_PATH_TEMP;
	let planFolder = tempFolder + '\\' + planNumber
	functions.log.info('Checks temp folder exists:', tempFolder);
	if (typeof tempFolder !== 'undefined') {
		lib.fs.readdir(planFolder, function(err, files) {
			if (err) {
				functions.log.info('Directory read failed.  Error: ' + err);
				communication.send(constants.FRONT_EVENTS.UNABLE_TO_READ_DIRECTORY, '');	
			} else {
				files = files.map(function (fileName) {
					return {
						name: fileName,
						time: lib.fs.statSync(planFolder + '\\' + fileName).mtime.getTime()
					};
					})
					.sort(function (a, b) {
					return b.time - a.time; })
					.map(function (v) {
					return v.name; });
		
					files = files.filter(filename => (filename.toLowerCase().endsWith('.rom')));
			
					if(files) {
					lib.fs.readFile(planFolder + '\\' + files[0], function (err, data) {
						if (err) {
							functions.log.info('Autosave read failed.  Error: ' + err);
							communication.send(constants.FRONT_EVENTS.UNABLE_TO_RETRIEVE_AUTOSAVE, '');	
							return;	
						}
						functions.log.info('Autosave read successfully: ' + files[0]);
						communication.send(constants.FRONT_EVENTS.SUCCESS_RETRIEVING_AUTOSAVE, data)
						return;
					});
				}
			}
		  });  
	}
}

module.exports = recoverPlanAutosave;
