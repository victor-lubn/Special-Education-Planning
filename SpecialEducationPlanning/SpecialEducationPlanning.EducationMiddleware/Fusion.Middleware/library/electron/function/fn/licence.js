'use strict';
const { lib, constants, functions } = require('../../../common');
const communication = require('../../communication');

/**
 * Looks the current licence version and: the expiration time and warn time. In any case, the method throws the follows events:
 * FRONT_EVENTS.LICENCE_CORRECT
 * FRONT_EVENTS.LICENCE_DOESNT_EXISTS
 * FRONT_EVENTS.LICENCE_WARN_DATE
 * FRONT_EVENTS.LICENCE_EXPIRED
 * 
 * Please, the definitions are in  FRONT_EVENTS constants.
 * 
 * For the initial configuration uses: 
 * APPLICATION_CONSTANTS.FILE_LICENCE_PATH
 * 
 * @param {event} event the event.
 * @param {Function} callback the callback function.
 * @param {boolean} sendCommunication sets if it is necessary send events. 
 */
function lookVersion(event, callback, sendCommunication = true) {
	communication.send(constants.FRONT_EVENTS.LICENCE_CORRECT, '');
	// let file = constants.APPLICATION_CONSTANTS.FILE_LICENCE_PATH;
	// functions.log.info('Check licence into the path:', constants.APPLICATION_CONSTANTS.FILE_LICENCE_PATH);
	// if (typeof file !== 'undefined') {
	// 	if (_existsLicence(file, sendCommunication)) {
	// 		lib.fs.readFile(file, 'utf8', function (err, contents) {
	// 			let _valid = _isValid(contents, sendCommunication)
	// 			if (_valid) {
	// 				functions.log.info('Licence correct');
	// 				if (sendCommunication) {
	// 					communication.send(constants.FRONT_EVENTS.LICENCE_CORRECT, '');
	// 				}
	// 			} else {
	// 				functions.log.error('The licence is not correct, please contact with the administrator.');
	// 			}
	// 			if (callback) {
	// 				callback(_valid);
	// 			}
	// 		});
	// 	}
	// }
}

/**
 * Checks if the licence exists or not. If the file doesn't exist, send a event "LICENCE_DOESNT_EXISTS".
 * 
 * @param {String} file The path file.
 * @param {boolean} sendCommunication sets if it is necessary send events. 
 * @returns (Boolean) Does the file exist?
 */
function _existsLicence(file, sendCommunication) {
	let existsFile = lib.fs.existsSync(file);
	if (!existsFile) {
		functions.log.error('The licence does not exist, please contact with the administrator.');
		//If the file doesn't exist, send the event with the file path like an argument.
		if (sendCommunication) {
			communication.send(constants.FRONT_EVENTS.LICENCE_DOESNT_EXISTS, file);
			communication.send(constants.FRONT_EVENTS.NO_FUSION_EXEC, 'Not licence, please contact with the administrator.');
		}
	}
	return existsFile;
}

/**
 * Extract the date through the regex pattern.
 *
 * @param {String} content The content file.
 * @param {Object} regex The regex.
 * @returns The date correctly convert it. 
 */
function _convertStringToDate(content, regex) {
	let stringDate = regex.exec(content)[1];
	let dateSplit = stringDate.split(',');
	let replaceText = /[" ]/gi;
	return new Date(parseInt(dateSplit[2].replace(replaceText, '')),
		(parseInt(dateSplit[1].replace(replaceText, '')) - 1),
		parseInt(dateSplit[0].replace(replaceText, '')));
}
/**
 * Checks if the licence has a valid date to executed. 
 * 
 * @param {String} content The content file.
 * @param {boolean} sendCommunication sets if it is necessary send events. 
 * @returns (Boolean) Does the licence valid?
 */
function _isValid(content, sendCommunication) {
	let endDateRegex = new RegExp(/(?:EndDate = )\s*(.*)/g);
	let warnDateRegex = new RegExp(/(?:WarnDate = )\s*(.*)/g);
	let endDate = _convertStringToDate(content, endDateRegex);
	let warnDate = _convertStringToDate(content, warnDateRegex);
	let todayDate = new Date();

	if (endDate <= todayDate) {
		if (sendCommunication) {
			communication.send(constants.FRONT_EVENTS.LICENCE_EXPIRED, endDate);
		}
		return false;
	}
	else if (warnDate <= todayDate) {
		if (sendCommunication) {
			communication.send(constants.FRONT_EVENTS.LICENCE_WARN_DATE, warnDate);
		}
		return false;
	}
	return true;
}

module.exports = lookVersion;