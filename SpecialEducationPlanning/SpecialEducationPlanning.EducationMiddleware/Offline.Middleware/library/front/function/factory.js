'use strict';

const frontConstants = require('../../common/constants');

/**
 * Front Function.
 */
const functions = {
	/**
     * First event when the application is instanciated. 
     */
     handShanking: frontConstants.EVENT_CONSTANTS.HANDSHAKING_EVENT,
	/**
     * Open document event. 
     */
     createPlan: frontConstants.EVENT_CONSTANTS.CREATE_PLAN,

     readPlans: frontConstants.EVENT_CONSTANTS.READ_FILE,

     deleteFile: frontConstants.EVENT_CONSTANTS.DELETE_FILE,

     writePlans: frontConstants.EVENT_CONSTANTS.WRITE_PLANS,
     
     editPlan: frontConstants.EVENT_CONSTANTS.UPDATE_PLAN,

     editVersionNotes: frontConstants.EVENT_CONSTANTS.UPDATE_VERSION_NOTES,

     getRom: frontConstants.EVENT_CONSTANTS.GET_ROM,

     getPreview: frontConstants.EVENT_CONSTANTS.GET_PREVIEW,

     getPlan: frontConstants.EVENT_CONSTANTS.GET_PLAN,

     readCatalogues: frontConstants.EVENT_CONSTANTS.READ_CATALOGUES,

     createAction: frontConstants.EVENT_CONSTANTS.CREATE_ACTION,

     placeVersionFiles: frontConstants.EVENT_CONSTANTS.PLACE_VERSION_FILES
};

module.exports = functions;