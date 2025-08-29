const fs = require('fs');


const send = require('../../communication')
const frontConstants = require('../../../common/constants').FRONT_EVENTS;
const applicationConstants = require('../../../common/constants').APPLICATION_CONSTANTS;
const functions = require('../../../common/functions');

function deleteFile() {
    functions.log.info(`-----------------------------------------------------------------`);
    functions.log.info('Delete Plans');

    let file = applicationConstants.FILE_PATH_JSON;
    functions.log.info('Delting path:', file);
    
    fs.unlink(file, function (err) {
        if (err) {
            functions.log.info('File delete failed.  Error: ' + err);
            send(frontConstants.ERROR_DELETING_FILE, '');
        }
        functions.log.info('File deleted');
        send(frontConstants.DELETING_FILE_SUCCESS, '' );

        functions.log.info(`-----------------------------------------------------------------`);
    })
}

module.exports = deleteFile; 