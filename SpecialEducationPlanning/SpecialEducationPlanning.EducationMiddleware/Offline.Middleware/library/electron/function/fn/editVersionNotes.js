const fs = require('fs');
const send = require('../../communication');
const frontConstants = require('../../../common/constants').FRONT_EVENTS;
const applicationConstants = require('../../../common/constants').APPLICATION_CONSTANTS;
const functions = require('../../../common/functions');

function editVersionNotes(event, model) {
    functions.log.info(`-----------------------------------------------------------------`);
    functions.log.info('Update Version Notes');

    const file = applicationConstants.FILE_PATH_JSON;

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

                // Find the index & element to edit
                var planIndex = planList.findIndex(p => p.id_offline === model.planId);
                var plan = planList.find(p => p.id_offline === model.planId);
                plan.updatedDate = new Date();

                var version = plan.versions.find(v => v.id_offline === model.versionId);
                version.versionNotes = model.versionNotes;
                version.quoteOrderNumber = model.quoteOrderNumber;

                // Update the element
                planList[planIndex] = plan;

                let buffer = new Buffer(JSON.stringify(planList, null, 4));

                fs.writeFile(
                    file,
                    buffer,
                    ['utf-8', '0o666', 'w+'],
                    function (err) {
                        if (err) {
                            functions.log.info('Error writing. Error: ' + err);
                            send(frontConstants.ERROR_EDIT_VERSION_NOTES, err);
                        }
                    }
                );

                functions.log.info('Success editing version notes');
                functions.log.info(`-----------------------------------------------------------------`);
                send(frontConstants.EDIT_VERSION_NOTES_SUCCESS, version);
            });
    } else {
        functions.log.info('File does not exist');
        functions.log.info(`-----------------------------------------------------------------`);
        send(frontConstants.NON_EXISTING_FILE, '');
        return;
    }
}

module.exports = editVersionNotes; 