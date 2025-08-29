const fs = require('fs');
const send = require('../../communication')
const frontConstants = require('../../../common/constants').FRONT_EVENTS;
const applicationConstants = require('../../../common/constants').APPLICATION_CONSTANTS;
const functions = require('../../../common/functions');

async function getPreview(event, previewPath, sendCommunication = true) {
	let tempFolder = applicationConstants.FOLDER_PATH_TEMP;
	functions.log.info('Checks temp folder exists:', tempFolder);
	if (typeof tempFolder !== 'undefined') {
		fs.readFile(previewPath, function (err, data) {
			if (err) {
				functions.log.info('Preview read failed.  Error: ' + err);
				if(sendCommunication) {
					send(frontConstants.ERROR_RETRIEVING_PREVIEW, '');
					return;
				}			
			}
			
			functions.log.info('Preview read');
			send(frontConstants.RETRIEVING_PREVIEW_SUCCESS, data);
	
			functions.log.info(`-----------------------------------------------------------------`);
			return;
		})
	}
}

module.exports = getPreview; 