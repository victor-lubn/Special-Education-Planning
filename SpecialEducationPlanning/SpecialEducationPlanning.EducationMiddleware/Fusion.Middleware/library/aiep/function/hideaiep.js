'use strict';
const  tdpConstants  = require('../../common').constants.APPLICATION_CONSTANTS.PROCESS.TDP;
const  appConstants = require('../../common').constants;
const  minimizeTdpFn  = require('../../electron/function/fn/minimize');

module.exports =  function hideTdp() {
	minimizeTdpFn(appConstants.DEV_MODE ? tdpConstants.DEV : tdpConstants.PROD);
};
