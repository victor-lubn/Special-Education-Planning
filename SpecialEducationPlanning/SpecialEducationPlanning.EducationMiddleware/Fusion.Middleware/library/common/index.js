const lib = (typeof window == 'undefined')?require('./requires'):{};
const functions = (typeof window == 'undefined')?require('./functions'):{};
const constants = require('./constants');
const messages = require('./messages/index');
const processWindows = (typeof window == 'undefined')?require('node-process-windows'):{};

module.exports = {
	lib,
	functions,
	constants,
	messages,
	processWindows
};
