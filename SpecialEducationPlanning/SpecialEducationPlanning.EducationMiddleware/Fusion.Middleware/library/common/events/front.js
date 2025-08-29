
/**
 * Front events.
 */
const FRONT_EVENTS = {

	/**
     * Exit application event.
     */
	EXIT_APPLICATION: 'exit-application-event',
    
	/**
     * Close application event.
     */
	CLOSE_APPLICATION: 'close-application-event',
    
	/**
     * Save document event.
     */
	UPDATE_DOCUMENT: 'update-document-event',
   
	/**
     * Remove document event.
     */
	REMOVE_DOCUMENT: 'remove-document-event',
   
	/**
     * Data document event.
     */
	DATA_DOCUMENT: 'data-document-event',
   
	/**
     * Close document event.
     */
	CLOSE_DOCUMENT: 'close-document-event',
    
	/**
     * Error to open document event.
     */
	ERROR_OPEN_DOCUMENT: 'error-open-document-event',
   
	/**
     * Correct open document event.
     */
	OK_OPEN_DOCUMENT: 'ok-open-docuemnt-event',
   
	/**
     * Back door for generic events. 
     */
	GENERIC: 'generic-event',

     /**
      * 
      */
     TEMPORARY_CLEAN_FILES: 'temporary-folder-cleaned',

	/**
     * This event will be thrown when the file doesn't exists or whatever.
     */
     LICENCE_DOESNT_EXISTS: 'licence-doesnt-exists-event',
     
	/**
     * This event will be thrown when the licence has in a warn date.
     */
	LICENCE_WARN_DATE: 'licence-warn-date-event',

	/**
     * This event will be thrown when the licence has expired.
     */
	LICENCE_EXPIRED: 'licence-expired-event',

	/**
     * This event will be thrown when the licence is valid. 
     */
	LICENCE_CORRECT: 'licence-correct-event',
     
	/**
      * This event will be thrown when we try to open a non-existent catalog.
      */
     CATALOG_DOESNT_EXITS: 'catalog-doesnt-exists-event',
     
     /**
      * This event will be thrown when TDP hasn't got a fusion running.
      */
     NO_FUSION_EXEC:'no-fusion-event',

     /**
     * This event will be thrown when the version file doesn't exists or whatever.
     */
     VERSION_DOESNT_EXISTS: 'version-doesnt-exists-event',
    
    /**
     * This event will be thrown when the licence is valid. 
     */
    VERSION_CORRECT: 'version-correct-event',

     /**
     * This event will be thrown when the plan's rom doesn't exist in the tempo folder or access to it is not possible
     */
    UNABLE_TO_RETRIEVE_ROM: 'unable-to-retrieve-rom',

    /**
     * This event will be thrown when the plan's preview doesn't exist in the tempo folder or access to it is not possible
     */
    UNABLE_TO_RETRIEVE_PREVIEW: 'unable-to-retrieve-preview',
    
    /**
     * This event will be thrown when the the plan's rom and preview can be retrieved successfully 
     */
    SUCCESS_RETRIEVING_ROM_AND_PREVIEW: 'success-retrieving-rom-and-preview',

    /**
     * This event will be thrown when the the last plan's autosave can be retrieved successfully 
     */
    SUCCESS_RETRIEVING_AUTOSAVE: 'success-retrieving-autosave',

    /**
     ** This event will be thrown when the plan's autosave doesn't exist in thes folder or access to it is not possible 
     */
    UNABLE_TO_RETRIEVE_AUTOSAVE: 'unable-to-retrieve-autosave',
    
     /**
     ** This event will be thrown when the plan's directory can't be read
     */
    UNABLE_TO_READ_DIRECTORY: 'unable-to-read-directory'
};

module.exports = FRONT_EVENTS;