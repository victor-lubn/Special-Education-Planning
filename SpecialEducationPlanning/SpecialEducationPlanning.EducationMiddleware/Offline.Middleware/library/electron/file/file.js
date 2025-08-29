'use strict';

const { lib, functions } = require('../../common');

function emptyFolder(path) {

	lib.fs.readdir(path, (err, files) => {
		if (err) {
			functions.log.error(`Failed to empty folder ${path} with this error ${err}`);
			throw err;
		}
		for (const file of files) {
			let fileToDelete = lib.path.join(path, file);
			deleteFile(fileToDelete);
		}
	});
}
/**
 * Exports
 */
module.exports = {
	emptyFolder
};