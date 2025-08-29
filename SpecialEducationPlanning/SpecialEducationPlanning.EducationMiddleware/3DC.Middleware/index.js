'use strict';

const ThreeDCElectron = require('./library/electron');
const ThreeDCFront = require('./library/front');
const ThreeDCConstants = require('./library/common').constants;

module.exports = {
	/**
	 * 3DC middleware electron functions.
	 */
	ThreeDCElectron,
	/**
	 * 3DC middleware Front functions.
	 */
	ThreeDCFront,
	/**
	 * 3DC middleware constants.
	 */
	ThreeDCConstants
};
