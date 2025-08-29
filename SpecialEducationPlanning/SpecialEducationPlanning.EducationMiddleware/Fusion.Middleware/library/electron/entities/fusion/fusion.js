'use strict';
const { lib, constants, functions } = require('../../../common');
const communication = require('../../communication');
const fusionPath = [constants.APPLICATION_CONSTANTS.FUSION_PATH, constants.APPLICATION_CONSTANTS.FUSION_NAME_APP].join('\\');

function checksFusion() {
	// let _check = constants.APPLICATION_CONSTANTS.LAUNCH_FUSION && lib.fs.existsSync(fusionPath);
	// if (!_check) {
	// 	functions.log.error('No Fusion installed or disabled');
	// 	communication.send(constants.FRONT_EVENTS.NO_FUSION_EXEC, 'Fusion is disabled by the middleware');
	// }
	// return _check;
	return true;
}

module.exports = { checksFusion };