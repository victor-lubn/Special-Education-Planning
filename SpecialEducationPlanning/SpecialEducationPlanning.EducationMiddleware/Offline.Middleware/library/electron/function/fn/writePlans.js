const fs = require('fs');


const send = require('../../communication')
const frontConstants = require('../../../common/constants').FRONT_EVENTS;
const applicationConstants = require('../../../common/constants').APPLICATION_CONSTANTS;
const functions = require('../../../common/functions');

function writePlans(event, plans) {
    functions.log.info(`-----------------------------------------------------------------`);
    functions.log.info('Write Plans');

    let file = applicationConstants.FILE_PATH_JSON;
    functions.log.info('Writing path:', file);

    let buffer = new Buffer(JSON.stringify(plans, null, 4));

    fs.open(file, 'w', function (err, fd) {
        if (err) {
            functions.log.info('File read failed.  Error: ' + err);
            send(frontConstants.ERROR_READING_FILE, '');
        }

        functions.log.info('File read');

        fs.write(fd, buffer, 0, buffer.length, null, function (err) {
            if (err) {
                functions.log.info('File write failed.  Error: ' + err);
                send(frontConstants.ERROR_WRITING_PLANS, '');
            }

            fs.close(fd, function () {
                functions.log.info('Plans written');
                send(frontConstants.WRITING_PLANS_SUCCESS, '');
            });
        });
    });
}

module.exports = writePlans; 