const builder  = require('./builder');

/**
 * Front Middleware class.
 */
class FrontMiddlewareFront{

	/**
     * Default constructor.
     * 
     * @param {ipcRender} ipcRender The ipcrender object.
     */
	constructor(ipcRender){
		builder(this, ipcRender);
	}

	/**
     * Function to manually start up Front Middleware application.
     */
	bootstrap(){
		this.handShanking(); 
	}
}
module.exports = FrontMiddlewareFront;