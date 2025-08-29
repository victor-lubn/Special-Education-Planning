const builder  = require('./builder');

/**
 * Offline Middleware front class.
 */
class OfflineMiddlewareFront{

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
module.exports = OfflineMiddlewareFront;