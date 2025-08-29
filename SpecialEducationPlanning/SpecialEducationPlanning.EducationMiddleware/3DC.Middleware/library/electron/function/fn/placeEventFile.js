'use strict';

const fs = require('fs');
const path = require('path');
const applicationConstants = require('../../../common/constants').APPLICATION_CONSTANTS;
const functions = require('../../../common/functions');

/**
 * Writes data into a JSON file in a defined folder with error handling.
 * @param {string} filename - The name of the file (without extension).
 * @param {object} data - The data to be written to the file, serialized as JSON.
 */
function placeEventFile(filename, data) {
    try {
        const eventsFolder = applicationConstants.FOLDER_THREE_DC_EVENTS;
        if (!fs.existsSync(eventsFolder)) {
            fs.mkdirSync(eventsFolder, {recursive: true});
        }
        const filePath = path.join(eventsFolder, `${filename}.json`);
        const jsonData = JSON.stringify(data, null, 2); // Beautify JSON with indentation
        fs.writeFileSync(filePath, jsonData, 'utf8'); // Sync write for consistent state operations
    } catch (error) {
        functions.log(`Error writing file ${filename}.json: ${error.message}`);
        throw error;
    }
}

module.exports = placeEventFile;