
/**
 * Application constants.
 */
const APPLICATION_CONSTANTS = {

	/**
	 * Launch Fusion?
	 */
	LAUNCH_FUSION : true,
	
	/**
	 * Time to clean files in days. 	
	 */
	CLEAN_TIME_TEMP_FILES: 10,

	/**
      * Delay time. 
      */
	DELAY_TIME: 750,

	/**
     * The path document base where we store the files temporary. 
     */
	PATH_DOCUMENT_BASE: 'C:\\tmp',

	/**
     * Sets the file format which provides the front-end. 
     */
	FILE_FORMAT: 'base64',

	/**
     * Gets the file licence path where we look the time of expiration.
     */
	FILE_LICENCE_PATH: 'C:\\Planit\\V9\\ppsecure.lic',

	/**
      * Gets the file where the catalogs are.
      */
	FILE_PATH_CATALOG: 'C:\\Planit\\V9\\ppsecure.lic',

	/**
     * Gets the current path file work.
     */
	FILE_PATH: 'C:\\TDP\\MSG',

	/**
     * Gets the current path work.
     */
	FOLDER_PATH_WATCHER: 'C:\\TDP\\MSG',

	/**
     * The rom file path folder temp.
     */
	FOLDER_PATH_TEMP: 'C:\\TDP\\tmp',

	/**
      * The log folder.  
      */
	FOLDER_LOG: 'C:\\TDP\\Middleware\\Log',

	/**
      * The path to start Fusion
      */
	 FUSION_PATH: 'C:\\Planit\\V9',
	 
	 /**
      * Fusion configuration path
      */
     FUSION_CONFIG_PATH: 'C:\\Planit\\V9\\PPPrefs.ini',
	 /**
      * Fusion offline configuration path
      */
     FUSION_OFFLINE_CONFIG_PATH: 'C:\\Planit\\V9\\PPPrefsOffline.ini',
	 /**
      * Fusion online configuration path
      */
     FUSION_ONLINE_CONFIG_PATH: 'C:\\Planit\\V9\\PPPrefsOnline.ini',

	/**
       * The name of fusion app executable.
       */
	FUSION_NAME_APP: 'FusionController.exe',

	/**
      * The task manager Fusion name for get the process and put the window on the top on the screen. 
      */
	FUSION_NAME: '2020 Fusion',

	/**
      * The task manager TDP name for get the process and put the window on the top on the screen. 
      */
	TDP_NAME: 'TDP',

	/**
	* Variables for the process execution
	*/
	PROCESS: {
		FUSION: {
			SHOW: '2020 Fusion',
			HIDE: {
				CONTROLLER: 'FusionController',
				PROGRAM: '2020 Fusion'
			}
		},
		TDP: {
			DEV: 'electron',
			PROD: 'Education-view'	
		},
	},

	/**
	 * Gets the Fusion's path where the version file is.
	 */
	VERSIONPATH: "C:\\Planit\\V9\\Version\\version.json"
};

module.exports = APPLICATION_CONSTANTS; 
