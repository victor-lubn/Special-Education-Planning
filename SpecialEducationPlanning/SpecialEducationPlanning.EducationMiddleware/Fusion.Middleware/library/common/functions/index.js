'use strict';

const FMUser32Functions = require('./user32');
const FMKernel32Functions = require('./kernel32');
const FMHandShakingFunction = require('./handshaking');
const log = require('./log');

module.exports = {
	FMUser32Functions,
	FMKernel32Functions,
	FMHandShakingFunction,
	log
};