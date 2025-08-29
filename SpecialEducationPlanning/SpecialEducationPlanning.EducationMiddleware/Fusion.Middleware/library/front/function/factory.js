'use strict';

const constants = require('../../common').constants.EVENTS_NAMES;

/**
 * Front Function.
 */
const functions = {
	/**
     * First event when the application is instanciated. 
     */
	handShanking: constants.HANDSHAKING_EVENT,
	/**
     * Open document event. 
     */
	open: constants.BUILDER_OPEN,
	/**
     * Save documente event.
     */
	save: constants.BUILDER_SAVE_EVENT,
	/**
     * Quit document event.
     */
	quit: constants.BUILDER_QUITE_EVENT,
	/**
     * Close fusion event.
     */
	closeFusion: constants.CLOSE_PROGRAM,

	/**
	 * Shows Fusion.
	 */
	showFusion: constants.SHOW_FUSION,
	
	/**
     * Check the licence event.
     */
	checkLicence: constants.CHECK_LICENCE,

	/**
	 * Sends new and save event.
	 */
	newAndSave: constants.BUILDER_NEW_AND_SAVE_EVENT,
	
	/**
	 * Sends event to Middleware.
	 */
	send: constants.SEND_EVENT_MIDDELWARE,
	
	/**
	 * Checks Fusion.
	 */
	checkFusion: constants.CHECK_FUSION,
	
	/**
	 * Gets the fusion version.
	 */
	getFusionVersion: constants.GET_FUSION_VERSION,

	/**
	 * Gets the rom and preview from a plan's version
	 */
	getRomAndPreview: constants.GET_ROM_AND_PREVIEW,

	/**
	 * Gets the last plan's autosave 
	 */
	recoverPlanAutosave: constants.GET_PLAN_AUTOSAVE
};

module.exports = functions;