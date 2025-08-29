'use strict';

const ffi = require('ffi-napi');

// Get proxy functions for native WINAPI FindWindow and SendMessage functions
// ffi requires the name of a dll to call methods from, along with an object
// describing the methods we want access to - their names and an array which
// contains their return type and an array of input types
let _kernel = null;

function kernel32(){
	if (!_kernel) {
		_kernel = ffi.Library('Kernel32.dll', {
			GetCurrentThreadId: ['int', []]
		});
	}
	return _kernel;
}


module.exports = kernel32();
