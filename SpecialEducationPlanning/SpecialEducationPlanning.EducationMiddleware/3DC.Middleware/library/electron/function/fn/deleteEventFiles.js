'use strict';

const fs = require('fs');
const path = require('path');
const applicationConstants = require('../../../common/constants').APPLICATION_CONSTANTS;

/**
 * Deletes specified files from the events folder.
 *
 * @param {Array<string>} filenames - An array of filenames (without folder path) to be deleted.
 */
function deleteEventFiles(filenames) {
    try {
        const eventsFolder = applicationConstants.FOLDER_THREE_DC_EVENTS;

        // Check if the directory exists before proceeding
        if (!fs.existsSync(eventsFolder)) {
            throw new Error(`Folder does not exist: ${eventsFolder}`);
        }

        filenames.forEach(filename => {
            const filePath = path.join(eventsFolder, filename);

            // Check whether the file exists
            if (fs.existsSync(filePath)) {
                fs.unlinkSync(filePath); // Remove the file synchronously
                console.log(`File successfully removed: ${filename}`);
            } else {
                console.warn(`File does not exist, skipping: ${filename}`);
            }
        });
    } catch (error) {
        console.error('Error removing event files:', error.message);
        throw error; // Throw the error to notify the caller
    }
}

module.exports = deleteEventFiles;