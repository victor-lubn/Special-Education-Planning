'use strict';

const fs = require('fs');
const send = require('../../communication')
const frontConstants = require('../../../common/constants').FRONT_EVENTS;
const applicationConstants = require('../../../common/constants').APPLICATION_CONSTANTS;
const functions = require('../../../common/functions');

async function getPlan(event, planIdOffline) {
    functions.log.info(`-----------------------------------------------------------------`);
    functions.log.info('Read Plans');

    let file = applicationConstants.FILE_PATH_JSON;
    functions.log.info('Reading path:', file);

    if (fs.existsSync(file)) {
        fs.readFile(file, 'utf8', function (err, data) {
            if (err) {
                functions.log.info('File read failed.  Error: ' + err);
                send(frontConstants.ERROR_READING_FILE, '');
                return;
            } else {
                functions.log.info('File read');
                if (data) {
                    functions.log.info('Get plan: ');
                    const planList = JSON.parse(data);
                    const plan = planList.find(plan => plan.id_offline === planIdOffline);

                    if (plan) {
                        functions.log.info('Plan retrieve');
                        send(frontConstants.RETRIEVING_PLAN_SUCCESS, plan);
                        return;
                    } else {
                        send(frontConstants.ERROR_RETRIEVING_PLAN, '');
                        return;
                    }
                } else {
                    send(frontConstants.ERROR_RETRIEVING_PLAN, '');
                    return;
                }
            }
        });
    } else {
        functions.log.info('File does not exist');
        send(frontConstants.NON_EXISTING_FILE, '');
        return;
    }

}

module.exports = getPlan; 