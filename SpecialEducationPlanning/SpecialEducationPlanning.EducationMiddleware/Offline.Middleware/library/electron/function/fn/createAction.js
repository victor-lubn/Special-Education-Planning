'use strict';

const fs = require('fs');
const send = require('../../communication')
const frontConstants = require('../../../common/constants').FRONT_EVENTS;
const applicationConstants = require('../../../common/constants').APPLICATION_CONSTANTS;
const functions = require('../../../common/functions');

// #region Variables
let actionList = [];
let actionOfflineModel;

const file = applicationConstants.FILE_PATH_ACTION_LOGS;
const root = applicationConstants.FOLDER_PATH_TEMP;

//#endregion

// #region Functions

// #region function generateActionOfflineModel
/*
 * Return the ActionOfflineModel completed to write
 */
function generateActionOfflineModel(action) {
    return {
        ...action,
        id_offline: actionList.length > 0 ? (actionList[actionList.length - 1].id_offline + 1) : 1,
        date: new Date(),
        path: action.isPlan ? root + '\\' + action.planNumber : root + '\\' + action.planNumber + '\\' + action.entityId 
    }
}
// #endregion


// #region function writeAction
/*
 * Receives the data to write as a Buffer element and appends it at the end of the file.  In case the file 
 * doesn't exist it will create it.
 */
function writeAction(buffer) {
    fs.writeFile(
        file,
        buffer,
        ['utf-8', '0o666', 'w+'],
        function (err) {
            if (err) {
                functions.log.info('Error writing. Error: ' + err);
                send(frontConstants.ERROR_WRITING_ACTION_LOGS, err);
                return;
            }
        }
    );
}
// #endregion
// #endregion

/**
 * Appending to the Action Log file a new file with the information
 */
async function createAction(event, action) {
    functions.log.info(`-----------------------------------------------------------------`);
    functions.log.info('Action Written');

    if (fs.existsSync(file)) {

        fs.readFile(
            file,
            'utf-8',
            function (err, data) {
                if (err) {
                    functions.log.info('File read failed.  Error: ' + err);
                    send(frontConstants.ERROR_READING_ACTION_LOGS, '');
                    return;
                }

                // Convert the data from the file to a manipulable object
                if (data != '' || data != undefined) { actionList = JSON.parse(data); }

                // Map to our needed format
                actionOfflineModel = generateActionOfflineModel(action);

                // Add to the actual list the new element
                actionList.push(actionOfflineModel);

                let buffer = new Buffer(JSON.stringify(actionList, null, 4));

                // Write into the file
                writeAction(buffer);

                send(frontConstants.SUCCESS_WRITING_ACTION_LOGS, '');
                functions.log.info('Success writing');
                functions.log.info(`-----------------------------------------------------------------`);
                return;
            });

    } else {
        functions.log.info('File does not exist - Created');

        // Map to our needed format
        actionOfflineModel = generateActionOfflineModel(action);

        let buffer = new Buffer('[ ' + JSON.stringify(actionOfflineModel, null, 4) + ' ]');

        // Write into the file
        writeAction(buffer);

        send(frontConstants.SUCCESS_WRITING_ACTION_LOGS, '');
        functions.log.info('Success writing');
        functions.log.info(`-----------------------------------------------------------------`);
        return;
    }
}


module.exports = createAction; 