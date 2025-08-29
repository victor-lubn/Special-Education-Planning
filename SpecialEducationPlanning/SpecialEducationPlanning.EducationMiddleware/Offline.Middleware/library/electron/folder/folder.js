const { lib } = require('../../common');

/**
 * 
 * @param {String} dir The current path to created
 */
function createFolders(dir) {
	let splitPath = dir.split('\\');
	splitPath.reduce((path, subPath) => {
		let currentPath;
		if (subPath != '.' && subPath.indexOf(':') === -1) {
			currentPath = path + '\\' + subPath;
			if (!lib.fs.existsSync(currentPath)) {
				lib.fs.mkdirSync(currentPath);
			}
		}
		else {
			currentPath = subPath;
		}
		return currentPath;
	}, '');
}

/**
 * Exports
 */
module.exports = {
	// addFolderWatcher,
	createFolders
};