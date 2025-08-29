'use strict';

const fs = require('fs');
const send = require('../../communication')
const frontConstants = require('../../../common/constants').FRONT_EVENTS;
const applicationConstants = require('../../../common/constants').APPLICATION_CONSTANTS;
const functions = require('../../../common/functions');

// #region Variables
let planList = [];
let planOfflineModel;

const file = applicationConstants.FILE_PATH_JSON;
//#endregion

// #region Functions
// #region function generatePlanCode
/*
 * Generate the Plan Code, this code will be given to the Folder aswell.  The PlanCode is 
 * based on the actual day month and year, following this structure 'yyyymmdd', for example, 
 * 20190912 for the 12th of September, 2019.
 */
function generatePlanCode() {
    var now = new Date();
    var timestamp = now.getFullYear().toString();
    timestamp += ((now.getMonth() + 1) < 10 ? '0' : '') + (now.getMonth() + 1).toString();
    timestamp += (now.getDate() < 10 ? '0' : '') + now.getDate().toString();
    timestamp += now.getHours().toString() + now.getMinutes().toString() + now.getSeconds().toString();

    return timestamp;
}
// #endregion

// #region function generatePlanOfflineModel
/*
 * Create and return the PlanModel needed to write in the file
 */
function generatePlanOfflineModel(plan) {
    var calculatedIdOffline = planList.length > 0 ? (planList[planList.length - 1].id_offline + 1) : 1;
    return {
        id_offline: calculatedIdOffline,
        planNumber: 'Offline_' + generatePlanCode() + '_' + calculatedIdOffline,
        planName: plan.planName,
        EducationerName: plan.EducationerName,
        survey: plan.survey,
        createdDate: new Date(),
        updatedDate: new Date(),
        lastOpen: new Date(),
        versions: [],
        catalogueCode: plan.catalogueCode,
        catalogueId: plan.catalogueId
    };
}
// #endregion

// #region function writePlan
/*
 * Receives the data to write as a Buffer element and writes it in the file.  In case the file 
 * doesn't exist it will create it.
 */
function writePlan(buffer) {
    fs.writeFile(
        file,
        buffer,
        ['utf-8', '0o666', 'w+'],
        function (err) {
            if (err) {
                functions.log.info('Error writing. Error: ' + err);
                send(frontConstants.CREATE_PLAN_ERROR, err);
                return;
            }
        }
    );
}
// #endregion
// #endregion

async function createPlan(event, plan) {
    functions.log.info(`-----------------------------------------------------------------`);
    functions.log.info('Create New Plan');

    // Open the file with reading & writing permissions; if not it will be created
    fs.access(file, fs.constants.F_OK | fs.constants.W_OK | fs.constants.R_OK, (err) => {
        if (err && err.code == 'ENOENT') {
            // As the file doesn't exist we create the file with the element received as the first element
            functions.log.info('File does not exist - Created');

            // Map the plan & version to our needed format
            planOfflineModel = generatePlanOfflineModel(plan);

            let buffer = new Buffer('[ ' + JSON.stringify(planOfflineModel, null, 4) + ' ]');

            // Write into the file
            writePlan(buffer);

            // continua si da error?
            send(frontConstants.CREATE_PLAN_SUCCESS, planOfflineModel);
            functions.log.info('Success writing');

            functions.log.info(`-----------------------------------------------------------------`);
        } else {
            // File already exists.  Add the new element
            functions.log.info('File exists.  Proceed to read and write.');
            fs.readFile(
                file,
                'utf-8',
                function (err, data) {
                    if (err) {
                        functions.log.info('File read failed.  Error: ' + err);
                        send(frontConstants.ERROR_READING_FILE, '');
                        return;
                    }

                    // Convert the data from the file to a manipulable object
                    if (data != '' || data != undefined) { planList = JSON.parse(data); }

                    // Map the plan & version to our needed format
                    planOfflineModel = generatePlanOfflineModel(plan);

                    // Add to the actual list the new element
                    planList.push(planOfflineModel);

                    let buffer = new Buffer(JSON.stringify(planList, null, 4));

                    // Write into the file
                    writePlan(buffer);

                    send(frontConstants.CREATE_PLAN_SUCCESS, planOfflineModel);
                    functions.log.info('Success writing');
                });


            functions.log.info(`-----------------------------------------------------------------`);
        }
    });
}

module.exports = createPlan; 
