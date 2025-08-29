'use strict';

const fnFactory = require('./factory');

function builder(object, ipcRender) {

	if (typeof object === 'undefined') {
		throw ('object cannot be null.');
	}

	for (var propertyName in fnFactory) {
		let message = fnFactory[propertyName];
		object[propertyName] = (msg) => {
			ipcRender.send(message, msg);
		};
	}
}

module.exports = builder;