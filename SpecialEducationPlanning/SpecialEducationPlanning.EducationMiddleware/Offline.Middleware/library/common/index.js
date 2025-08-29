const lib = (typeof window == 'undefined') ? require('./requires') : {};
const constants = require('./constants');
const functions = (typeof window == 'undefined') ? require('./functions') : {};

module.exports = {
	lib,
	functions,
	constants
};