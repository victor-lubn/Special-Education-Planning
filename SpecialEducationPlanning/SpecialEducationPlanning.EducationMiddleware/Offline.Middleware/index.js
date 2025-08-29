
"use strict";

const OMElectron = require('./library/electron');
const OMFront = require('./library/front');
const OMConstants = require('./library/common').constants;

module.exports = {
    /**
     * Offline middleware electron functions.
     */
    OMElectron,

    /**
     * Offline middleware front functions.
     */
    OMFront,

    /**
     * Offline middleware constants.
     */
    OMConstants
}