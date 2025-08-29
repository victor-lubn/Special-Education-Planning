'use strict';

const builder  = require('./builder');
const FrontMiddlewareFront = {};

builder(FrontMiddlewareFront);

FrontMiddlewareFront.bootstrap = () => {
	FrontMiddlewareFront.handShanking(); 
};

module.exports = FrontMiddlewareFront;
