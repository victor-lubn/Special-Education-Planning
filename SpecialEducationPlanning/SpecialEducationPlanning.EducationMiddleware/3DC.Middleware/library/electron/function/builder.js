
'use strict';

const { lib } = require('../../common');
const fnFactory = require('./factory');

/**
 * Adds all messages.
 * 
 */
function builder() {
	if (typeof lib.ipcMain !== 'undefined') {
		for (var propertyName in fnFactory) {
			let message = fnFactory[propertyName];
			lib.ipcMain.on(message.event, message.fn);
		}
	}
}

module.exports = builder;