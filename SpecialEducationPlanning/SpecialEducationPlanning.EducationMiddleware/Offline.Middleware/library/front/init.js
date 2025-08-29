'use strict';

const builder  = require('./builder');
const OfflineMiddlewareFront = {};

builder(OfflineMiddlewareFront);

OfflineMiddlewareFront.bootstrap = () => {
	OfflineMiddlewareFront.handShanking(); 
};

module.exports = OfflineMiddlewareFront;
