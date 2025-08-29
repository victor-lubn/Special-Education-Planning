'use strict';
const { lib, constants, functions } = require('../../../common');
const communication = require('../../communication');
const {file} = require('../../entities/file/file');


function getVersion(callback, sendCommunication = true) {
	let file = constants.APPLICATION_CONSTANTS.VERSIONPATH;
	functions.log.info('Checks version into the path:', file);
	if (typeof file !== 'undefined') {
		if (_existsFile(file, sendCommunication)) {
				let content = require(constants.APPLICATION_CONSTANTS.VERSIONPATH);
				if (content) {
					functions.log.info('Version correct', content);
					communication.send(constants.FRONT_EVENTS.VERSION_CORRECT, content.version);
				} else {
					//functions.log.error('The version is not correct, please contact with the administrator.');
					communication.send(constants.FRONT_EVENTS.VERSION_DOESNT_EXISTS, "");
				}
		}
		else 
		{
			communication.send(constants.FRONT_EVENTS.VERSION_DOESNT_EXISTS, "");
		}
	}
}

/**
 * Checks if the licence exists or not. If the file doesn't exist, send a event "LICENCE_DOESNT_EXISTS".
 * 
 * @param {String} file The path file.
 * @param {boolean} sendCommunication sets if it is necessary send events. 
 * @returns (Boolean) Does the file exist?
 */
function _existsFile(file, sendCommunication) {
	let existsFile = lib.fs.existsSync(file);
	if (!existsFile) {
		functions.log.error('The Version does not exist, please contact with the administrator.');
		//If the file doesn't exist, send the event with the file path like an argument.
		if (sendCommunication) {
			communication.send(constants.FRONT_EVENTS.VERSION_DOESNT_EXISTS, file);
			communication.send(constants.FRONT_EVENTS.NO_FUSION_EXEC, 'Not Version, please contact with the administrator.');
		}
	}
	return existsFile;
}

module.exports = getVersion;