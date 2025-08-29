'use strict';
const constants  = require('../../common').constants.APPLICATION_CONSTANTS.PROCESS.FUSION;
const showFusionFn = require('../../electron/function/fn/show');

module.exports = function showFusion() {
	showFusionFn(constants.SHOW);
};
