const documentHasMap = require('./document');
const { lib, constants, functions } = require('../../../common');

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
 * @param {String} destinationFolder The folder file.
 */
async function createFileAndReturnPath(arrayBytes, destinationFolder) {

	//let path = createTemporaryPath(fileId);
	return new Promise((resolve, reject) => {
		lib.fs.writeFile(destinationFolder, arrayBytes,  constants.APPLICATION_CONSTANTS.FILE_FORMAT, function (error) {
			if (error){
				functions.log.error(`TDP middleware cannot create the file ${destinationFolder} because`, error);
				reject(error);
			}
			else{
				functions.log.info(`TDP middleware create the file ${destinationFolder}`);
				resolve(destinationFolder);
			}
		});
	});
}

/**
 * Creates a temporary file name.
 * 
 * @param {int} fileId The file id.
 */
function createTemporaryPath(fileId) {
	let path = lib.format('%s\\%s', constants.APPLICATION_CONSTANTS.FOLDER_PATH_TEMP, fileId);
	_createFolders(path);
	return lib.format('%s\\%s.Rom', path, fileId);
}

/**
 * Creates a temporary preview file name.
 * 
 * @param {int} fileId The file id.
 */
function createTemporaryPreviewPath(fileId) {
	let path = lib.format('%s\\%s', constants.APPLICATION_CONSTANTS.FOLDER_PATH_TEMP, fileId);
	_createFolders(path);
	return lib.format('%s\\preview_%s.jpeg', path, fileId);
}

/**
 * 
 * @param {String} dir The current path to created
 */
function _createFolders(dir) {
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
 * Converts a file to array bytes.
 * 
 * @param {string} filePath The file path.
 */
function convertFileToArray(filePath) {
	return lib.fs.readFileSync(filePath);
}

/**
 * Reads a file path.
 * 
 * @param {string} filePath The file path.
 */
function readFile(filePath){
	return lib.fs.readFileSync(filePath);
}

/**
 * Gets the current array of bytes.
 * 
 * @param {Object} model The model object has been passed by the front-end.
 */
function getFileCurrentStatus(model) {
	let document = documentHasMap.get(model.planCode);
	if (document) {
		// FIXME Closing Fusion without saving causes error
		let _filePath = _changeFileName(model.planCode, document.path);
		if (model.isNewPlan) {
			functions.log.info(`Save the new plan ${model.planCode}`);
			model.romFileInfo = {
				fileName: lib.format('%s.Rom', model.planCode),
				romByteArray: convertFileToArray(_filePath),
				type: 'application/octet-stream'
			}
		} else {
			functions.log.info(`Save the plan ${model.planCode}`);
			model.romFileInfo.romByteArray = convertFileToArray(_filePath);
		}
	}
	return model;
}

/**
 * This function changes the current name for the save name. This is necessary because Fusion blocks the main file after 
 * the closeEducation message has been created. Therefore, the middleware cannot copy the main file an exception throws, and yes,
 * Fusion can not change the file creation flow.
 * 
 * @param {Int} id The rom id.
 * @param {String} path The rom path.
 */
function _changeFileName(id, path){
	let _currentFile = lib.format('%s.Rom', id);
	let _changeFile = lib.format('%s_SAVE.Rom', id);
	functions.log.info(`TDP middleware changes the file ${path} to ${_changeFile}`);
	return path.replace(_currentFile, _changeFile);
}

/**
 * Exports
 */
module.exports = {
	closeAll,
	createFileAndReturnPath,
	createTemporaryPreviewPath,
	convertFileToArray,
	getFileCurrentStatus,
	readFile,
	createTemporaryPath
};
