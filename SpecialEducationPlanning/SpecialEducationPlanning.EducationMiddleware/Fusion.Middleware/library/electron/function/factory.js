'use strict';

const constants = require('../../common').constants;
const open = require('./fn/open');
const checkLicence = require('./fn/licence');
const focus = require('./fn/focus').focusProgram;
const closeFusion = require('../../fusion/function/closeFusion').closeFusion;
const checkFusion = require('./fn/fusion').checksFusion;
const showFusion = require('../../fusion/function/showFusion');
const getVersion = require('./fn/version');
const getRomAndPreview = require('./fn/romAndPreview');
const offline = require('./fn/offline');
const recoverPlanAutosave = require('./fn/recoverPlanAutosave');

/**
 * Constants list <event, function>
 */
const functions = {
	open:
        { event: constants.EVENTS_NAMES.BUILDER_OPEN, fn: open },
	checkLicence:
        { event: constants.EVENTS_NAMES.CHECK_LICENCE, fn: checkLicence },
	focusProgram:
        { event: constants.EVENTS_NAMES.FOCUS_PROGRAM, fn: focus },
	closeFusion:
        { event: constants.EVENTS_NAMES.CLOSE_PROGRAM, fn: closeFusion },
        checkFusion:
        { event: constants.EVENTS_NAMES.CHECK_FUSION, fn: checkFusion },
        showFusion:
        { event: constants.EVENTS_NAMES.SHOW_FUSION, fn: showFusion },
        getVersion:
        { event: constants.EVENTS_NAMES.GET_FUSION_VERSION, fn: getVersion },
        getRomAndPreview:
        { event: constants.EVENTS_NAMES.GET_ROM_AND_PREVIEW, fn: getRomAndPreview },
        offline:
        { event: constants.OFFLINE, fn: offline},
        recoverPlanAutosave:
        { event: constants.EVENTS_NAMES.GET_PLAN_AUTOSAVE, fn: recoverPlanAutosave }
};

module.exports = functions;