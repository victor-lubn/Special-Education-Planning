'use strict';

const fs = require('fs');
const send = require('../../communication')
const frontConstants = require('../../../common/constants').FRONT_EVENTS;
const applicationConstants = require('../../../common/constants').APPLICATION_CONSTANTS;
const functions = require('../../../common/functions');

async function getRom(event, romPath, sendCommunication = true) {
	fs.readFile(romPath, function (err, data) {
		if (err) {
			functions.log.info('Rom read failed.  Error: ' + err);
			if(sendCommunication) {
				send(frontConstants.ERROR_RETRIEVING_ROM, '');
			}			
		}
		
		functions.log.info('Rom read');
		send(frontConstants.RETRIEVING_ROM_SUCCESS, data);

		functions.log.info(`-----------------------------------------------------------------`);
	})
}

module.exports = getRom; 