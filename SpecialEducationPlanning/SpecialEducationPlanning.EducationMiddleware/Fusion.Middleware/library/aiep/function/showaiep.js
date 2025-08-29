'use strict';
const tdpConstants = require('../../common').constants.APPLICATION_CONSTANTS.PROCESS.TDP;
const maximizeTdpFn = require('../../electron/function/fn/maximize');
const  appConstants = require('../../common').constants;

module.exports = function showTdp() {
	return maximizeTdpFn(appConstants.DEV_MODE ? tdpConstants.DEV : tdpConstants.PROD);
};
