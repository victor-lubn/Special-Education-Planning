'use strict';

const fs = require('fs');
const path = require('path');
const applicationConstants = require('../../../common/constants').APPLICATION_CONSTANTS;
const eventContants = require('../../../common/constants').EVENT_CONSTANTS;
const send = require('../../communication');
/**
 * Reads all JSON files in a given directory, extracts their creation timestamps,
 * and returns the files' content in the order they were created.
 *
 * @returns {Array<Object>} - An array of file data objects sorted by creation time.
 */
function getAllFilesByCreationTime() {
    // try {
        const eventsFolder = applicationConstants.FOLDER_THREE_DC_EVENTS;

        // Check if the directory exists, throw error if not
        if (!fs.existsSync(eventsFolder)) {
            throw new Error(`Folder does not exist: ${eventsFolder}`);
        }

        // Read file names in the directory
        const fileNames = fs.readdirSync(eventsFolder);

        // Filter out non-JSON files (optional step in case the folder has other files)
        const jsonFiles = fileNames.filter(file => file.endsWith('.json'));

        // Map files to an array of their metadata (name, content, timestamp)
        const fileDataArray = jsonFiles.map(fileName => {
            const filePath = path.join(eventsFolder, fileName);

            // Get file stats (includes creation time)
            const stats = fs.statSync(filePath);

            // Parse file content
            const content = JSON.parse(fs.readFileSync(filePath, 'utf8'));

            // Return an object with the file's content, timestamp, and filename
            return {
                content,
                creationTime: stats.birthtime, // Creation time (timestamp)
                fileName
            };
        });

        // Sort files by creation time (ascending order)
        fileDataArray.sort((a, b) => a.creationTime - b.creationTime);

        // Return only the content of the sorted files (or modify as needed)
        console.log(fileDataArray);
        send(eventContants.GET_EVENTS_FROM_FILES, fileDataArray.map(file => file.content));
        return fileDataArray.map(file => file.content);
    // } catch (error) {
    //     console.error('Error reading files:', error.message);
    //     throw error; // Re-throw the error to notify the caller
    // }
}

module.exports = getAllFilesByCreationTime;