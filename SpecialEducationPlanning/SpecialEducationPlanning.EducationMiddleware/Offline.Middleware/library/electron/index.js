'use strict';

const _bootstrap = require('./init');
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
	bootstrap(){ 
		this._bootstrap.init();
	}
}

module.exports = Electron;