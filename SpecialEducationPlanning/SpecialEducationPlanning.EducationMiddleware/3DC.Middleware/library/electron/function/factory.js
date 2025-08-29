'use strict';

const constants = require('../../common').constants;
const getEventsFromFiles = require('./fn/readEventFiles');
const deleteEventFiles = require('./fn/deleteEventFiles');
/**
 * Constants list <event, function>
 */
const functions = {
    getEventsFromFiles: {
        event: constants.EVENT_CONSTANTS.GET_EVENTS_FROM_FILES,
        fn: getEventsFromFiles
    },
    deleteEventFiles: {
        event: constants.EVENT_CONSTANTS.DELETE_EVENT_FILES,
        fn: deleteEventFiles
    }
};

module.exports = functions;