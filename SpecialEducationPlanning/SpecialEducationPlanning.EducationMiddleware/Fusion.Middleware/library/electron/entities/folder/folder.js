'use strict';

const { lib, constants, functions } = require('../../../common');
const { FileNameConstants } = require('../file/filenames');
const documentUtils = require('../document/util');
const communication = require('../../communication');
const { deleteFile, readCloseFile, saveFileToTmp, savePreviewFileToTmp, readSaveFile } = require('../file');
const hideFusion = require('../../../fusion/function/hideFusion');
const showTdp = require('../../../tdp/function/showTdp');

let isSaving = false;
let isClosing = false;
let onlyOneWatch = undefined;

/**
 * Sets folder watcher. 
 */
function addFolderWatcher(model) {
	_closeEvents();
	onlyOneWatch = lib.watch(constants.APPLICATION_CONSTANTS.FOLDER_PATH_WATCHER, { recursive: true }, _execFolderBusiness.bind(model));
	return onlyOneWatch;
}

function _closeEvents() {
	if (onlyOneWatch !== undefined) {
		if (onlyOneWatch.close) {
			onlyOneWatch.close();
			onlyOneWatch = undefined;
		}
	}
}

function _execFolderBusiness(...arg) {
	//arg[0] = the event name.
	//arg[1] = the file path.
	if (arg[0] === 'remove') {
		functions.log.info(`The file ${arg[1]} has been deleted`);
	}
	else if (arg[1].indexOf('.jpeg') > -1) {
		functions.log.info(`The preview  ${arg[1]} has been created`);
	}
	else {
		let extension = lib.path.extname(arg[1]);
		let file = lib.path.basename(arg[1], extension);
		switch (file) {
			case FileNameConstants.NewEducation:
			case FileNameConstants.OpenEducation:
				//TODO we're waiting the new events specifications from Fusion company.
				break;
			case FileNameConstants.SaveEducation: {
				_setSemaphore(isSaving, this, arg[1], _saveEducation);
			}
				break;
			case FileNameConstants.CloseEducation: {
				_setSemaphore(isClosing, this, arg[1], _closeEducation);
			}
				break;
		}
	}
}

function _setSemaphore(semaphore, _this, file, fn) {
	if (!semaphore) {
		let model = _this;
		semaphore = true;
		setTimeout(fn.bind({ model, file }), constants.APPLICATION_CONSTANTS.DELAY_TIME);
	}
}

function _saveEducation() {
	let saveModel = readSaveFile();
	if (!saveModel.notExists) {
		try {
			saveFileToTmp(saveModel.fileName, this.model.planCode);
			deleteFile(this.file);
			communication.send(constants.FRONT_EVENTS.UPDATE_DOCUMENT, this.model.planCode);
		} catch (e) {
			console.log(e);
		}
	}
	isSaving = false;
}
function _closeEducation() {
	let newVariables = readCloseFile();
	communication.send(constants.FRONT_EVENTS.UPDATE_DOCUMENT, 'updateModel');
	if (!newVariables.notExists) {
		let updateModel = documentUtils.getFileCurrentStatus(this.model);
		updateModel.lineItems = newVariables.lineItems;
		updateModel.mainRange = newVariables.mainRange;
		updateModel.mainUniqueId = newVariables.mainUniqueId;
		updateModel.preview = getImage(updateModel, newVariables);
		communication.send(constants.FRONT_EVENTS.CLOSE_DOCUMENT, updateModel);
		savePreviewFileToTmp('CloseEducation.jpeg', this.model.planCode);
		deleteFile(this.file);
		deleteFile(newVariables.imagePath);
		_closeEvents();
		hideFusion();
		showTdp();
	}
	isClosing = false;
}

function getImage(model, closeFile) {
	return {
		fileName: `preview_${model.planCode}.jpeg`,
		previewByteArray: documentUtils.convertFileToArray(closeFile.imagePath),
		type: 'image/jpeg'
	} 
}
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
	addFolderWatcher,
	createFolders
};
