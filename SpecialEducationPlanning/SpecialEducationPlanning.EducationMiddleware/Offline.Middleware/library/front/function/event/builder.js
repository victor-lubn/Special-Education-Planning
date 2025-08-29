'use strict';

const { constants } = require('../../common');

const eventsFunction = {};
let addEventsFunction = (event, fn) => { eventsFunction[event] = fn; };

function builder(object, ipcRender) {
	if (typeof object === 'undefined') {
		throw ('Object cannot be null.');
	}
	object.on = (event, fn) => { addEventsFunction(event, fn); return object; };

	let _constants = constants.FRONT_EVENTS;

	for (var propertyName in _constants) {
		let internalEvent = _constants[propertyName];
		if (ipcRender) {
			ipcRender.receive(internalEvent, (event, arg) => {
				let fn = eventsFunction[internalEvent];
				if (fn) {
					fn.call(this, event, arg);
				}
			});
		}
	}
}

module.exports = builder;