'use strict';

const builderFunctions = require('./function/builder');
const builderEvents = require('./function/event/builder');


/**
 * Functions and events builder.
 */
class FunctionsBuilder {

	constructor (object, ipcRender) {
		return this.addEvents(object, ipcRender)
			.addFunction(object, ipcRender);
	}

	/**
     * Add the api functions for the use of the front. 
     * 
     * @param {FMFront} object
	 * @param {ipcRender} ipcRender The ipcrender object.
     */
	addFunction(object, ipcRender) {
		builderFunctions(object, ipcRender);
		return this;
	}

	/**
     * Add events for the use of the front.
     * 
     * @param {FMFront} object
	 * @param {ipcRender} ipcRender The ipcrender object.
     */
	addEvents(object, ipcRender) {
		builderEvents(object, ipcRender);
		return this;
	}
}

module.exports = (object, ipcRender) => {
	return new FunctionsBuilder(object, ipcRender );
};