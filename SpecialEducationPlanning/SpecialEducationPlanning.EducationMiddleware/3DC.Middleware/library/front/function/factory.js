'use strict';

const frontConstants = require('../../common/constants');

const functions = {
	handShanking: frontConstants.EVENT_CONSTANTS.HANDSHAKING_EVENT,
	getEventsFromFiles: frontConstants.EVENT_CONSTANTS.GET_EVENTS_FROM_FILES,
	deleteEventFiles: frontConstants.EVENT_CONSTANTS.DELETE_EVENT_FILES
};

module.exports = functions;