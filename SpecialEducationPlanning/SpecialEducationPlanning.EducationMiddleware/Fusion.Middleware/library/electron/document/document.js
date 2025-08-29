/**
 * Document has map.
 */
class DocumentsHasMap {

	/**
     * Default constructor. 
     */
	constructor() {
		this.map = {};
	}

	/**
     * Set item.
     * Note: if you'll add the same key, it will be override. 
     * 
     * @param {int} model The object key.
     * @param {object} obj The model.
     * @param {int} process The process pid.
     * @param {object} watcher  The file watcher.
     * @param {string} path  The file path.
     * */
	add(key, model, process, watcher, path) {
		this.map[key] = { model, process, watcher, path };
	}

	/**
     * Get item.
     * 
     * @param {Int} key 
     */
	get(key) {
		return this.map[key];
	}
  
	/**
     * Delete item.
     * 
     * @param {Int} key 
     */
	delete(key) {
		delete this.map[key];
	}

}

const documents = new DocumentsHasMap();

module.exports = documents;

