/**
 * Application constants.
 */
const APPLICATION_CONSTANTS = {

	/**
   * Gets the file where the plans are.
   */
  FILE_PATH_JSON: 'C:\\TDP\\offline\\data.json',

	/**
   * Gets the file where the catalogs are.
   */
  FILE_PATH_CATALOGUES: 'C:\\TDP\\offline\\catalogues.json',

	/**
   * Gets the file where the logs are.
   */
  FILE_PATH_ACTION_LOGS: 'C:\\TDP\\offline\\action_logs.json',

  /**
   * The log folder.  
   */
  FOLDER_LOG: 'C:\\TDP\\Middleware\\Log',

	/**
   * The rom file path folder temp.
   */
  FOLDER_PATH_TEMP: 'C:\\TDP\\tmp'
};

const EVENT_CONSTANTS = {

  HANDSHAKING_EVENT: 'async-message-handShanking-offline',

  SEND_EVENT_FRONT: 'async-message-front',

  RECEIVE_EVENT_FRONT: 'async-message-front',

  CREATE_PLAN: 'create-plan',

  UPDATE_PLAN: 'update-plan',

  UPDATE_VERSION_NOTES: 'update-version-notes',

  CREATE_FILE: 'create-file',

  DELETE_FILE: 'delete-file',

  READ_FILE: 'read-plan',
  
  WRITE_PLANS: 'write-plans',

  READ_CATALOGUES: 'read-catalogues',

  GET_ROM: 'get-rom',

  GET_PREVIEW: 'get-preview',

  GET_PLAN: 'get-plan',

  CREATE_ACTION: 'create-actions',

  READ_ACTION: 'read-actions',
  
  PLACE_VERSION_FILES: 'place-version-files'
};

const FRONT_EVENTS = {
  /**
   * Success creating the plan
   */
  CREATE_PLAN_SUCCESS: 'plan-created-correctly',

  /**
   * Error creating the plan
   */
  CREATE_PLAN_ERROR: 'error-creating-plan',

  /**
   * Success editing the plan
   */
  EDIT_PLAN_SUCCESS: 'plan-edited-correctly',

  /**
   * Error editing the plan
   */
  EDIT_PLAN_ERROR: 'error-edited-plan',

  /**
   * Success reading the JSON File
   */
  READING_FILE_SUCCESS: 'file-read-correctly',

  /**
   * This will be throw when an error is produced reading the JSON File
   */
  ERROR_READING_FILE: 'error-reading-file',

  /**
   * When the file does not exist
   */
  NON_EXISTING_FILE: 'non-existing-file',

  /**
   * Success deleting the JSON File
   */
  DELETING_FILE_SUCCESS: 'file-delete-correctly',

  /**
   * This will be throw when an error is produced deleting the JSON File
   */
  ERROR_DELETING_FILE: 'error-deleting-file',

  /**
   * Success writing plans
   */
  WRITING_PLANS_SUCCESS: 'plans-written-correctly',

  /**
   * This will be throw when an error is produced writing multiple plans at once
   */
  ERROR_WRITING_PLANS: 'error-writing-plans',

  /**
   * Success reading the Catalogues
   */
  READING_CATALOGUES_SUCCESS: 'catalogues-read-correctly',

  /**
   * This will be throw when an error is produced reading the catalogues File
   */
  ERROR_READING_CATALOGUES: 'error-reading-catalogues',

  /**
   * Success reading the Action Logs
   */
  READING_ACTION_LOGS_SUCCESS: 'action-logs-read-correctly',

  /**
   * This will be throw when an error is produced reading the Action Logs File
   */
  ERROR_READING_ACTION_LOGS: 'error-reading-action-logs',

  /**
   * This will be throw when an error is produced writing the Action File
   */
  ERROR_WRITING_ACTION_LOGS: 'error-writing-action-logs',

  /**
   * Success creating the Action
   */
  SUCCESS_WRITING_ACTION_LOGS: 'success-writing-action-logs',

  /**
   * Success reading the JSON File
   */
  EDIT_VERSION_NOTES_SUCCESS: 'version-notes-edited-correctly',

  /**
   * This will be throw when an error is produced reading the JSON File
   */
  ERROR_EDIT_VERSION_NOTES: 'error-edited-version-notes',

  /**
  * Success retriving the rom File
  */
  RETRIEVING_ROM_SUCCESS: 'rom-retrieved-correctly',

  /**
   * This will be throw when an error is produced retrieving the rom
   */
  ERROR_RETRIEVING_ROM: 'error-retrieving-rom',

  /**
  * Success retriving the preview File
  */
  RETRIEVING_PREVIEW_SUCCESS: 'preview-retrieved-correctly',

  /**
   * This will be throw when an error is produced retrieving the preview
   */
  ERROR_RETRIEVING_PREVIEW: 'error-retrieving-preview',

  /**
  * Success retriving the a plan
  */
  RETRIEVING_PLAN_SUCCESS: 'plan-retrieved-correctly',

  /**
   * This will be throw when an error is produced retrieving the preview
   */
  ERROR_RETRIEVING_PLAN: 'error-retrieving-plan',

  /**
   * This will be throw when an error is produced listing the directories/files in a plan's folder
   */
  ERROR_LISTING_DIRECTORY: 'error-listing-directory',

   /**
   * This will be throw when an error is produced when no version's action is recognized
   */
  ERROR_NO_ACTION: 'error-no-action',

  /**
   * This will be throw when an error is produced when trying to get the item's stats from a folder
   */
  ERROR_READING_STATS: 'error-reading-stats',

  /**
   * This will be throw when an error is produced when trying to copy a file from one folder to another
   */
  ERROR_COPYING_FILE: 'error-copying-file',

  /**
   * This will be throw when an error is produced when trying to remove a file from a plan's/version's folder
   */
  ERROR_REMOVING_VERSION_FILE: 'error-removing-version-file',
  
  /**
    * Success creating the version
    */
  VERSION_FILES_MOVED_SUCCESS: 'version_files_moved_success',

   /**
    * Success discarding the version
    */
   DISCARDING_VERSION_SUCCESS: 'discard-version-success',

};

const PLACE_VERSION_ACTIONS = {
  CREATE: 0,
  OVERWRITE: 1,
  DISCARD: 2
}

module.exports = {
  APPLICATION_CONSTANTS,
  EVENT_CONSTANTS,
  FRONT_EVENTS,
  PLACE_VERSION_ACTIONS
}