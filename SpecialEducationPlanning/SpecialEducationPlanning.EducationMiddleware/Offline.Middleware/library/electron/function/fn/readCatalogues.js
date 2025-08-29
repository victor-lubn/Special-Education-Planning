'use strict';

const fs = require('fs');
const send = require('../../communication')
const frontConstants = require('../../../common/constants').FRONT_EVENTS;
const applicationConstants = require('../../../common/constants').APPLICATION_CONSTANTS;
const functions = require('../../../common/functions');

async function readCatalogues() {
    functions.log.info(`-----------------------------------------------------------------`);
    functions.log.info('Read Catalogues');

    let file = applicationConstants.FILE_PATH_CATALOGUES;
    functions.log.info('Reading path: ', file);

    if (fs.existsSync(file)) {
        fs.readFile(file, 'utf8', function (err, data) {
            if (err) {
                functions.log.info('File read failed.  Error: ' + err);
                functions.log.info(`-----------------------------------------------------------------`);
                send(frontConstants.ERROR_READING_CATALOGUES, err);
                return;
            } else {
                functions.log.info('File read');
                functions.log.info(`-----------------------------------------------------------------`);
                send(frontConstants.READING_CATALOGUES_SUCCESS, JSON.parse(data));
                return;
            }

        });
    } else {
        functions.log.info('File does not exist');
        functions.log.info(`-----------------------------------------------------------------`);
        send(frontConstants.NON_EXISTING_FILE, err);
        return;
    }

}

module.exports = readCatalogues; 