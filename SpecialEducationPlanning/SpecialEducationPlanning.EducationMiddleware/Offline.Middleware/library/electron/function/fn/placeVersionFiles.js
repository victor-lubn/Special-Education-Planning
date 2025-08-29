'use strict';

const fs = require('fs');
const send = require('../../communication')
const frontConstants = require('../../../common/constants').FRONT_EVENTS;
const placeVerionActionsConstants = require('../../../common/constants').PLACE_VERSION_ACTIONS;
const applicationConstants = require('../../../common/constants').APPLICATION_CONSTANTS;
const functions = require('../../../common/functions');

function placeVersionFiles(event, data) {
    let tempFolder = applicationConstants.FOLDER_PATH_TEMP;
    let planFolder = tempFolder + '\\' + data.planNumber;
    let versionFolder = planFolder + '\\' + data.versionNumber;

    switch(data.action) {
        case placeVerionActionsConstants.CREATE:
            functions.log.info('Creating version ' + data.versionNumber + ' directory');
            fs.mkdirSync(versionFolder);
            readFilesFromDirectory(planFolder, versionFolder)
            break;
        case placeVerionActionsConstants.OVERWRITE:
            functions.log.info('Overwritting version ' + data.versionNumber);
            readFilesFromDirectoryOverwrite(planFolder, versionFolder);
            break;
        case placeVerionActionsConstants.DISCARD:
            functions.log.info('Discarding version ' + data.versionNumber + ' changes');
            readFilesFromDirectoryDiscard(planFolder);
            break;
        default:
            functions.log.info('Action not recognized');
            send(frontConstants.ERROR_NO_ACTION, false);
            break;
    }
}

// Create
function readFilesFromDirectory(planDirectory, versionDirectory) {
    functions.log.info('Reading directory');
    fs.readdir(planDirectory, (err, files) => {
        if (err) {
            functions.log.info('Error reading directory: ' + err);
            send(frontConstants.ERROR_LISTING_DIRECTORY, false);
            return;
        } else {
            moveFiles(planDirectory, versionDirectory, files, 0);
        }
    });
}

function moveFiles(planDirectory, versionDirectory, files, pos) {
    if(pos < files.length) {
        let filePath = planDirectory + '\\' + files[pos];
        fs.stat(filePath, function (err, data) {
            if (err) {
                send(frontConstants.ERROR_READING_STATS, false);
                return;
            }

            if(!data.isDirectory()) {
                fs.rename(filePath, versionDirectory + '\\' + files[pos], (err)=>{
                    if(err) { 
                        functions.log.info('Error moving file: ' + err);
                        send(frontConstants.ERROR_COPYING_FILE, false);
                        return; 
                    } else  {
                        moveFiles(planDirectory, versionDirectory, files, pos + 1);
                    }
                });
            } else {
                moveFiles(planDirectory, versionDirectory, files, pos + 1);
            }
        });    
    } else {
        send(frontConstants.VERSION_FILES_MOVED_SUCCESS, true);
        return;
    }
}

// Overwrite
function readFilesFromDirectoryOverwrite(planDirectory, versionDirectory) {
    functions.log.info('Reading directory');
    fs.readdir(versionDirectory, (err, files) => {
        if (err) { 
            functions.log.info('Error reading directory: ' + err);
            send(frontConstants.ERROR_LISTING_DIRECTORY, false);
            return;
        } else {
            cleanFolderToOverwrite(planDirectory, versionDirectory, files, 0);
        }
    });
}

function cleanFolderToOverwrite(planDirectory, versionDirectory, files, pos) {
    if (pos < files.length) {
        let filePath = versionDirectory + '\\' + files[pos];
        fs.stat(filePath, function (err, data) {
            if (err) {
                send(frontConstants.ERROR_READING_STATS, false);
                return;
            }

            if(!data.isDirectory()) {
                fs.unlink(filePath, err => {
                    if (err) { 
                        functions.log.info('Error moving file: ' + err);
                        send(frontConstants.ERROR_REMOVING_VERSION_FILE, false);
                        return;
                    } else {
                        cleanFolderToOverwrite(planDirectory, versionDirectory, files, pos + 1);
                    }
                });
            } else {
                cleanFolderToOverwrite(planDirectory, versionDirectory, files, pos + 1);
            }
        });
    } else {
        readFilesFromDirectory(planDirectory, versionDirectory);
    }
}

// Discard
function readFilesFromDirectoryDiscard(planDirectory) {
    functions.log.info('Reading directory');
    fs.readdir(planDirectory, (err, files) => {
        if (err) { 
            functions.log.info('Error reading directory: ' + err);
            send(frontConstants.ERROR_LISTING_DIRECTORY, false);
            return;
        } else {
            discardChanges(planDirectory, files, 0);
        }
    });
}

function discardChanges(planDirectory, files, pos) {
    if (pos < files.length) {
        let filePath = planDirectory + '\\' + files[pos];
        fs.stat(filePath, function (err, data) {
            if (err) {
                send(frontConstants.ERROR_READING_STATS, false);
                return;
            }

            if(!data.isDirectory()) {
                fs.unlink(filePath, err => {
                    if (err) { 
                        functions.log.info('Error deleting file: ' + err);
                        send(frontConstants.ERROR_REMOVING_VERSION_FILE, false);
                        return;
                    } else {
                        discardChanges(planDirectory, files, pos + 1);
                    }
                });
            } else {
                discardChanges(planDirectory, files, pos + 1);
            }
        });
    } else {
        send(frontConstants.DISCARDING_VERSION_SUCCESS, true);
        return;
    }
}

module.exports = placeVersionFiles; 