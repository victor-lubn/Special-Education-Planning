'use strict';

const fs = require('fs');
const send = require('../../communication');
const frontConstants = require('../../../common/constants').FRONT_EVENTS;
const applicationConstants = require('../../../common/constants').APPLICATION_CONSTANTS;
const functions = require('../../../common/functions');


// #region Variables
let planList = [];

const file = applicationConstants.FILE_PATH_JSON;
//#endregion

// #region Functions
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
                send(frontConstants.EDIT_PLAN_ERROR, err);
            }
        }
    );
}
// #endregion

// #region function generatePath
/*
 *  Create the preview and the rom paths with each preview and rom path to the corresponding 
 *  folder for each version
 */
function generatePath(plan) {
    let planUpdated = plan;
    let tmpFolder = applicationConstants.FOLDER_PATH_TEMP;
    plan.versions.forEach(element => {
        let planFolder = tmpFolder + '\\' + plan.planNumber + '\\' + element.id_offline;
        functions.log.info('Checks temp folder exists:', tmpFolder);
        if (typeof tmpFolder !== 'undefined') {
            planUpdated.versions[element.id_offline - 1].romPath = planFolder + '\\' + plan.planNumber + '.Rom';
            planUpdated.versions[element.id_offline - 1].previewPath = planFolder + '\\preview_' + plan.planNumber + '.jpeg';
        }
    });

    return planUpdated;
}
// #endregion
// #endregion

function editPlan(event, plan) {
    functions.log.info(`-----------------------------------------------------------------`);
    functions.log.info('Update Plan');

    if (fs.existsSync(file)) {
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

                // Find the element to edit
                var planIndex = planList.findIndex(p => p.id_offline === plan.id_offline);

                let updatedPlan = generatePath(plan);

                updatedPlan.lastOpen = new Date();
                updatedPlan.updatedDate = new Date();

                // Update the plan element
                planList[planIndex] = updatedPlan;

                let buffer = new Buffer(JSON.stringify(planList, null, 4));

                // Write the file
                writePlan(buffer);

                functions.log.info('Success editing');
                functions.log.info(`-----------------------------------------------------------------`);
                send(frontConstants.EDIT_PLAN_SUCCESS, updatedPlan);
            });
    } else {
        functions.log.info('File does not exist');
        functions.log.info(`-----------------------------------------------------------------`);
        send(frontConstants.NON_EXISTING_FILE, '');
        return;
    }
}

module.exports = editPlan; 