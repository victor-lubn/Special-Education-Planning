'use strict';

const user32Fn = require('./user32').user32;
const Constants = require('../constants');

function sendMessage(message) {
	let form = user32Fn.FindWindowA(null, Constants.APPLICATIONNAME);
	if (form) {
		user32Fn.PostMessageA(form, message, null, null);
	}
}

module.exports = sendMessage;