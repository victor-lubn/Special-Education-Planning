'use strict';
const constants = require('../../common').constants.APPLICATION_CONSTANTS.PROCESS.FUSION;
const appConstants = require('../../common').constants;
const hideFusion = require('../../electron/function/fn/hide');

module.exports = function showFusion() {
	if(!appConstants.OFFLINESTATE)
		return hideFusion(constants.HIDE.PROGRAM);

	return new Promise(resolve=>true);
};