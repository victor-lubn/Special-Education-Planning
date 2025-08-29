'use strict';

const { lib, constants, functions } = require('../../../common');
const { CloseEducation, NewEducation, OpenEducation, SaveEducation } = require('../../../fusion/file');
const { FileNameConstants } = require('./filenames');

function createFileBusiness(model) {
	if (model.isNewPlan) {
		createNewPlanFile(model);
	} else {
		createOpenPlanFile(model);
	}
}

function createNewPlanFile(model) {
	_createFile(model, (pathName, fileName) => {
		functions.log.info('New plan file has been created');
		return new NewEducation(pathName,
			fileName,
			createDLTOverrides(model),
			model.catalogType,
			model.cadPlanNo,
			model.builderName,
			model.EducationerName,
			model.planCode,
			model.planName);
	}, FileNameConstants.NewEducation);
}


function createOpenPlanFile(model) {
	_createFile(model, (pathName, fileName) => {
		functions.log.info('Open plan file has been created');
		return new OpenEducation(
			pathName, 
			fileName, 
			model.catalogType,
			model.cadPlanNo,
			model.planCode,
			model.builderName,
			model.EducationerName,
			model.planName);
	}, FileNameConstants.OpenEducation);
}

function _createFile(model, fn, jsonName) {
	let fileName = model.planCode + '.Rom';
	let pathName = constants.APPLICATION_CONSTANTS.FOLDER_PATH_TEMP + '\\' + model.planCode + '\\';
	let fileBase = fn(pathName, fileName);
	let data = fileBase.toJSON();
	lib.fs.writeFileSync(lib.format('%s\\%s.json', constants.APPLICATION_CONSTANTS.FOLDER_PATH_WATCHER, jsonName), data);
}

function saveFileToTmp(fileName, id) {
	let filePath = lib.format('%s\\%s\\%s', constants.APPLICATION_CONSTANTS.FOLDER_PATH_TEMP, id, fileName);
	if (lib.fs.existsSync(filePath)) {
		let newFileName = fileName.replace('.', lib.format('_%s.', _getCurrentTime()));
		let fileDestination = lib.format('%s\\%s\\%s', constants.APPLICATION_CONSTANTS.FOLDER_PATH_TEMP, id, newFileName);
		// The file will be created or overwritten by default.
		lib.fs.copyFile(filePath, fileDestination, (err) => {
			if (err) {
				functions.log.error(`Failed move from ${filePath} to ${fileDestination} with this error ${err}`);
				//throw err;
			} else {
				functions.log.info(`The file was move from ${filePath} to ${fileDestination}`);
			}
		});
	}
}

function savePreviewFileToTmp(fileName, id) {
	let filePath = lib.format('%s\\%s', constants.APPLICATION_CONSTANTS.FOLDER_PATH_WATCHER, fileName);
	if (lib.fs.existsSync(filePath)) {
		let newFileName = `preview_${id}.jpeg`
		let fileDestination = lib.format('%s\\%s\\%s', constants.APPLICATION_CONSTANTS.FOLDER_PATH_TEMP, id, newFileName);
		// The file will be created or overwritten by default.
		lib.fs.copyFile(filePath, fileDestination, (err) => {
			if (err) {
				functions.log.error(`Failed move from ${filePath} to ${fileDestination} with this error ${err}`);
				//throw err;
			} else {
				functions.log.info(`The file was move from ${filePath} to ${fileDestination}`);
			}
		});
	}
}

function deleteFile(path) {
	if (lib.fs.existsSync(path)) {
		lib.fs.unlink(path, (err) => {
			if (err) {
				functions.log.error(`Failed to delete the file ${path} with this error ${err}`);
				throw err;
			} else {
				functions.log.info(`The file ${path} will be delete`);
			}
		});
	}
}

function readSaveFile() {
	let obj = _readFile(FileNameConstants.SaveEducation);
	functions.log.info('readSaveFile called');
	return new SaveEducation(obj.FilePath, obj.FileName);
}

function readCloseFile() {
	let obj = _readFile(FileNameConstants.CloseEducation);
	functions.log.info('readCloseFile called');
	return new CloseEducation(obj.FilePath, obj.FileName, obj.LineItems, obj.ImagePath, obj.MainUniqueId, obj.MainRange);
}

function _readFile(jsonFile) {
	functions.log.info('_readFile called');
	let _path = lib.format('%s\\%s.json', constants.APPLICATION_CONSTANTS.FOLDER_PATH_WATCHER, jsonFile);
	let _default = { FilePath: '', FileName: '' };
	if (lib.fs.existsSync(_path)) {		
		try {
			let data = lib.fs.readFileSync(_path);	
			return _IsJsonString(data) ? JSON.parse(data) : _default;
		} catch (error) {
			functions.log.error(`readFileSync called failed -> Error ${error}`);
			functions.log.info(`readFileSync called failed -> FilePath:${_path}`);
			return _default;
		}
				
	} else {
		_default.notExists = true;
		return _default;
	}
}

function _IsJsonString(str) {
	try {
		JSON.parse(str);
	} catch (e) {
		functions.log.error(`_IsJsonString called failed -> Error ${e}`);
		return false;
	}
	return true;
}

function _getCurrentTime() {
	var date = new Date();
	return [date.getFullYear(), (date.getMonth() + 1), date.getDate(), date.getHours(), date.getMinutes(), date.getSeconds()].join('_');
}

function createDLTOverrides(model) {
	return [{
		'UniqueId': model.catalogType || 12345678,
		'DLTTypeId': '1',
		'DLTEntryId': '16',
		'ColourLinkId': '3',
		'ColourId': '7',
		'DLTBlockIndex': '1',
		'Comment': 'Open new plan'
	}];
}

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
	createFileBusiness,
	saveFileToTmp,
	savePreviewFileToTmp,
	readSaveFile,
	readCloseFile,
	deleteFile,
	emptyFolder
};
