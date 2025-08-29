'use strict';

const { constants, lib } = require('../../common');
const { FileNameConstants } = require('../../electron/entities/file/filenames');

function closeFusion() {
	let data = {};
	lib.fs.writeFileSync(lib.format('%s\\%s.json', constants.APPLICATION_CONSTANTS.FOLDER_PATH_WATCHER, FileNameConstants.Quit), JSON.stringify(data));
}

module.exports = { closeFusion };