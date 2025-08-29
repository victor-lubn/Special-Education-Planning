
'use strict';

const _bootstrap = require('./init');
const _closeFusion = require('../fusion/function/closeFusion').closeFusion;
var appConstants = require('../common').constants;
/**
 * Electron class.
 */
class Electron {

	/**
     * Default constructor.
     */
	constructor(){
		this._bootstrap = new _bootstrap();
	}

	/**
     * Function to manually start up Electron Middleware application..
     */
	bootstrap(devMode, offline){
		appConstants.DEV_MODE = devMode;
		appConstants.OFFLINESTATE = offline;
		this._bootstrap.init();
	}

	closeFusion() {
		_closeFusion();
	}

}

module.exports = Electron;