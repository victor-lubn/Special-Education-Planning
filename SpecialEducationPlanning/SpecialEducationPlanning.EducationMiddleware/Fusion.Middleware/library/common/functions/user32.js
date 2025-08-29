'use strict';

const ffi = require('ffi-napi');

// Get proxy functions for native WINAPI FindWindow and SendMessage functions
// ffi requires the name of a dll to call methods from, along with an object
// describing the methods we want access to - their names and an array which
// contains their return type and an array of input types
let _user32 = null;
let _kernel = null;


/**
 * Get user32 functions
 */
function user32() {
	if (!_user32) {
		_user32 = ffi.Library('user32', {
			keybd_event: ['void', ['int32', 'int32', 'int32', 'int32']],
			SendMessageA: ['int32', ['long', 'int32', 'long', 'int32']],
			PostMessageA: ['int32', ['long', 'int32', 'long', 'int32']],
			FindWindowA: ['uint32', ['string', 'string']],
			GetKeyState: ['short', ['int']], // not really needed, since we track each key ourselves
			GetTopWindow: ['long', ['long']],
			SetActiveWindow: ['long', ['long']],
			SetForegroundWindow: ['bool', ['long']],
			BringWindowToTop: ['bool', ['long']],
			ShowWindow: ['bool', ['long', 'int']],
			SwitchToThisWindow: ['void', ['long', 'bool']],
			GetForegroundWindow: ['long', []],
			AttachThreadInput: ['bool', ['int', 'long', 'bool']],
			GetWindowThreadProcessId: ['int', ['long', 'int']],
			SetWindowPos: ['bool', ['long', 'long', 'int', 'int', 'int', 'int', 'uint']],
			SetFocus: ['long', ['long']]
		});
	}
	return _user32;
}

module.exports =  user32();
