'use strict';

const builder  = require('./builder');
const ThreeDcMiddlewareFront = {};

builder(ThreeDcMiddlewareFront);

ThreeDcMiddlewareFront.bootstrap = () => {
	ThreeDcMiddlewareFront.handShanking(); 
};

module.exports = ThreeDcMiddlewareFront;
