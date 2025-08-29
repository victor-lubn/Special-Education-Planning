//TODO maybe we should remove this file. 

const documentHasMap = require('./document');
const { lib, constants } = require('../../common');

/**
 * Closes all content of the document.
 * 
 * @param {int} pid 
 */
function closeAll(pid) {
	let doc = documentHasMap.get(pid);
	if (doc !== 'undefined') {
		if (doc.watcher) {

			doc.watcher.close();
		}
		if (doc.process) {
			doc.process.kill();
		}
		if (doc.path) {
			deleteFile(doc.path);
		}
	}
}

/**
 * Deletes file.
 * 
 * @param {String} path The file path.
 */
function deleteFile(path) {
	lib.fs.exists(path, function (exists) {
		if (exists) {
			lib.fs.unlink(path,()=>{
				console.log(arguments);
			});
		}
	});
}

/**
 * Creates a file from the blob and return the temporary path.
 * 
 * @param {Array} arrayBytes The array bytes.
 * @param {Int} fileId The file id.
 */
async function createFileAndReturnPath(arrayBytes, fileId) {

	let path = createTemporaryPath(fileId);
	return new Promise((resolve, reject) => {
		lib.fs.writeFile(path, arrayBytes,  constants.APPLICATION_CONSTANTS.FILE_FORMAT, function (error) {
			if (error)
				reject(error);
			else
				resolve(path);
		});
	});
}

/**
 * Creates a temporary file name.
 * 
 * @param {int} fileId The file id.
 */
function createTemporaryPath(fileId) {
	return lib.format('%s\\%s.Rom', constants.APPLICATION_CONSTANTS.PATH_DOCUMENT_BASE, fileId);
}

/**
 * Converts a file to array bytes.
 * 
 * @param {string} filePath The file path.
 */
function convertFileToArray(filePath) {
	return lib.fs.readFileSync(filePath, { encoding: constants.APPLICATION_CONSTANTS.FILE_FORMAT});
}

/**
 * Exports
 */
module.exports = {
	closeAll,
	createFileAndReturnPath,
	convertFileToArray
};