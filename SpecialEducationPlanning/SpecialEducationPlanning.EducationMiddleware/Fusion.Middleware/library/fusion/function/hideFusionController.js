'use strict';
const  constants  = require('../../common').constants.APPLICATION_CONSTANTS.PROCESS.FUSION;
const  hideFusionController  = require('../../electron/function/fn/hide');

module.exports =  function showFusion() {
	hideFusionController(constants.HIDE.CONTROLLER);
}
