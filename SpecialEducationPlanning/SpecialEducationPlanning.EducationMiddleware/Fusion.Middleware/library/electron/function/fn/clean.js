'use strict';
const { lib, constants, functions } = require('../../../common');
const communication = require('../../communication');
const { deleteFile } = require('../../entities/file/index');

/**
 * Delete temporary files from the folder.
 * 
 * @param {String} startPath the path.
 * @param {String} filter The filter
 * @param {Booelan} main if is main to send a message to Front.
 */
function cleanTemporaryFiles(startPath = constants.APPLICATION_CONSTANTS.FOLDER_PATH_TEMP, filter = '.rom', main=true) {

    functions.log.info(`[cleanTemporaryFiles] :: Start to clean the directory ${startPath}`);

    if (!lib.fs.existsSync(startPath)) {
        functions.log.info(`[cleanTemporaryFiles] :: No directory ${startPath}`);
        return;
    }
    var files = lib.fs.readdirSync(startPath);

    for (var i = 0; i < files.length; i++) {
        var filename = lib.path.join(startPath, files[i]).toLowerCase();
        var stat = lib.fs.lstatSync(filename);
        if (stat.isDirectory()) {
            cleanTemporaryFiles(filename, filter, false); //recurse
        }
        else if (filename.indexOf(filter) >= 0) {
            _deleteFile(filename);
        };
    };

    if(main){
        communication.send(constants.FRONT_EVENTS.TEMPORARY_CLEAN_FILES, files);
    }
};

function _deleteFile(path) {
    let _file = lib.fs.lstatSync(path);
    let _lastModify = new Date(_file.mtime);
    let _today = new Date();
    if (parseInt(((_today - _lastModify) / 1000 / 60 / 60 / 24)) > constants.APPLICATION_CONSTANTS.CLEAN_TIME_TEMP_FILES) {
        try {
            deleteFile(path);
        } catch (e) {
            functions.log.info(`[cleanTemporaryFiles] :: Error ${path}`);
        }
    }
}

module.exports = cleanTemporaryFiles;