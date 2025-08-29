const { lib, constants, functions } = require('../../../common');

/**
 * Method to set the correct configuration file (PPPrefs.ini) depending if the app is online or offline to allow or avoid users
 * to create plans directly from Fusion or not
 */
module.exports = function setFusionConfig() {
    let filePath;
    if (constants.OFFLINESTATE) {
        filePath = constants.APPLICATION_CONSTANTS.FUSION_OFFLINE_CONFIG_PATH
    }
    else {
        filePath = constants.APPLICATION_CONSTANTS.FUSION_ONLINE_CONFIG_PATH
    }

    lib.fs.copyFile(filePath, constants.APPLICATION_CONSTANTS.FUSION_CONFIG_PATH, (err) => {
        if (err) {
            functions.log.error(`Failed move from ${filePath} to ${constants.APPLICATION_CONSTANTS.FUSION_CONFIG_PATH} with this error ${err}`);
            //throw err;
        } else {
            functions.log.info(`The file was move from ${filePath} to ${constants.APPLICATION_CONSTANTS.FUSION_CONFIG_PATH}`);
        }
    });
}
