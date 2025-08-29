'use strict';

const documentHasMap = require('../../entities/document/document');
const documentUtils = require('../../entities/document/util');
const { createFileBusiness } = require('../../entities/file');
const { addFolderWatcher } = require('../../entities/folder');
const { addWatcher } = require('../../entities/process');
const checkCatalog = require('./catalog');
const { constants, functions } = require('../../../common');
const communication = require('../../communication');
const showFusion = require('../../../fusion/function/showFusion');
const hideTdp = require('../../../tdp/function/hideTdp');
const {checksFusion} = require('../../entities/fusion');
/**
 * Opens .Rom and subscribed all events.
 * 
 * @param {Object} event The event.
 * @param {Object} model The model object where contains all meta information to open the application.
 */
async function open(event, model) {
	try {
		if (checksFusion()) {
			functions.log.info(`-----------------------------------------------------------------`);
			functions.log.info(`[${model.isNewPlan?'NEW':'OPEN'} PLAN] :: ID  ${model.planCode}`);
			functions.log.info('Checks Catalog id:');
			await checkCatalog(model);
			functions.log.info('Create a temporary path for plan Id:', model.planCode);
			let destinationFolder = documentUtils.createTemporaryPath(model.planCode);
			let previewDestinationPath = documentUtils.createTemporaryPreviewPath(model.planCode);
			functions.log.info('Create a file and return path for id:', model.planCode);
			let path = await documentUtils.createFileAndReturnPath(
				model.romFileInfo ? model.romFileInfo.romByteArray : new Uint8Array(),
				destinationFolder
			);
			let previewPath = await documentUtils.createFileAndReturnPath(
				new Uint8Array(),
				previewDestinationPath
			);
			functions.log.info('Current create path:', path);
			createFileBusiness(model);
			let watcher = addWatcher(path);
			functions.log.info('Add folder watchers');
			let folderWatcher = addFolderWatcher(model);
			functions.log.info('Create process');
			let noteProcess = {}; // createProccess(path, model);
			functions.log.info('Store the model');
			documentHasMap.add(model.planCode, model, noteProcess, watcher, folderWatcher, path);
			functions.log.info('Show fusion');
			showFusion();
			functions.log.info('Hide Tdp');
			hideTdp();
			communication.send(constants.FRONT_EVENTS.OK_OPEN_DOCUMENT);
		}

	} catch (exception) {
		communication.send(constants.FRONT_EVENTS.ERROR_OPEN_DOCUMENT, exception);
		functions.log.error(`The open action throws ${JSON.stringify(exception)} for the plan ${JSON.stringify(model)}`);
	}
}

module.exports = open;