'use strict';

const { lib, constants } = require('../common');
var EventHandShaking = null;

if (lib.ipcMain) {
	lib.ipcMain.on(constants.EVENT_CONSTANTS.HANDSHAKING_EVENT, (event) => {
		EventHandShaking = event.sender;
	});
}

/**
 * Sends a event to Front.
 * @param {String} msg The event name.
 * @param {Object} obj The payload.
 */
function send(msg, obj) {
	if (EventHandShaking) {
		EventHandShaking.send(msg, obj);
	}
}

module.exports = send ;