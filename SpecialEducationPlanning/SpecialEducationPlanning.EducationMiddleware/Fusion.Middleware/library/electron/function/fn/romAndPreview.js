'use strict';
const { lib, constants, functions } = require('../../../common');
const communication = require('../../communication');


function getRomAndPreview(event, model, sendCommunication = true) {
	let tempFolder = constants.APPLICATION_CONSTANTS.FOLDER_PATH_TEMP;
	functions.log.info('Checks temp folder exists:', tempFolder);
	if (typeof tempFolder !== 'undefined') {
		let romFile;
		lib.fs.readFile(model.romFilePath, function (err, data) {
			if (err) {
				functions.log.info('Rom read failed.  Error: ' + err);
				if(sendCommunication) {
					communication.send(constants.FRONT_EVENTS.UNABLE_TO_RETRIEVE_ROM, '');
				}			
			}
			
			functions.log.info('Rom read');
			romFile = data;

			lib.fs.readFile(model.previewFilePath, function (err, data) {
				if (err) {
					functions.log.info('Preview read failed.  Error: ' + err);
					if(sendCommunication) {
						communication.send(constants.FRONT_EVENTS.UNABLE_TO_RETRIEVE_PREVIEW, '');
					}			
				}
				
				functions.log.info('Preview read');
				if(sendCommunication) {
					let files = {
						rom: romFile,
						preview: data,
						posPlan: model.posPlan,
						posVersion: model.posVersion
					};
					communication.send(constants.FRONT_EVENTS.SUCCESS_RETRIEVING_ROM_AND_PREVIEW, files);
				}
		
				functions.log.info(`-----------------------------------------------------------------`);
			})
	
			functions.log.info(`-----------------------------------------------------------------`);
		})
	}
}

module.exports = getRomAndPreview;
