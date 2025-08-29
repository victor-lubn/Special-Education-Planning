'use strict';

const constants = require('../../common').constants;
const createPlan = require('./fn/createPlan');
const readPlans = require('./fn/readPlans');
const deleteFile = require('./fn/deleteFile');
const writePlans = require('./fn/writePlans');
const editPlan = require('./fn/editPlan');
const editVersionNotes = require('./fn/editVersionNotes');
const getRom = require('./fn/getRom');
const getPreview = require('./fn/getPreview');
const getPlan = require('./fn/getPlan');
const placeVersionFiles = require('./fn/placeVersionFiles');
const readCatalogues = require('./fn/readCatalogues');
const createAction = require('./fn/createAction');

/**
 * Constants list <event, function>
 */
const functions = {
	open:
        { event: constants.EVENT_CONSTANTS.CREATE_PLAN, fn: createPlan },
        read:
        { event: constants.EVENT_CONSTANTS.READ_FILE, fn: readPlans },
        delete:
        { event: constants.EVENT_CONSTANTS.DELETE_FILE, fn: deleteFile },
        writePlans:
        { event: constants.EVENT_CONSTANTS.WRITE_PLANS, fn: writePlans },
        edit:
        { event: constants.EVENT_CONSTANTS.UPDATE_PLAN, fn: editPlan },
        editVersionNotes:
        { event: constants.EVENT_CONSTANTS.UPDATE_VERSION_NOTES, fn: editVersionNotes },
        getRom:
        { event: constants.EVENT_CONSTANTS.GET_ROM, fn: getRom },
        getPreview:
        { event: constants.EVENT_CONSTANTS.GET_PREVIEW, fn: getPreview },
        getPlan: 
        { event: constants.EVENT_CONSTANTS.GET_PLAN, fn: getPlan},
        placeVersionFiles:
        { event: constants.EVENT_CONSTANTS.PLACE_VERSION_FILES, fn: placeVersionFiles },
        readCatalogues: 
        { event: constants.EVENT_CONSTANTS.READ_CATALOGUES, fn: readCatalogues},
        createAction: 
        { event: constants.EVENT_CONSTANTS.CREATE_ACTION, fn: createAction}
};

module.exports = functions;