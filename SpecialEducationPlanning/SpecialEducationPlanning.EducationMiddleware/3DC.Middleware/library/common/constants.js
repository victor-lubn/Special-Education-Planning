/**
 * Application constants.
 */
const APPLICATION_CONSTANTS = {
	/**
	 * The log folder.
	 */
	FOLDER_LOG: 'C:\\TDP\\Middleware\\Log',

	FOLDER_THREE_DC_EVENTS: 'C:\\TDP\\3DC_Events'
};

// TODO put the constants to only one place
const EVENT_CONSTANTS = {
	HANDSHAKING_EVENT: 'async-message-handShanking-threedc',
	GET_EVENTS_FROM_FILES: 'async-message-getEventFiles',
	DELETE_EVENT_FILES: 'async-message-deleteEventFiles'
};

const FRONT_EVENTS = {
	PLAN_CREATED: 'plan-created',
	PLAN_SAVED: 'plan-saved',
	PLAN_OPENED: 'plan-opened',
	PLAN_CHANGED: 'plan-changed',
	SESSION_INITIALIZED: 'session-initialized',
	SESSION_ERROR: 'session-error',
	SESSION_CLOSED: 'session-closed',
	CHILD_WINDOW_UNRESPONSIVE: 'child-window-unresponsive',
	CHILD_WINDOW_RESPONSIVE: 'child-window-responsive',
	PLANNER_WINDOW_ALREADY_OPENED: 'planner-window-already-opened',
	PLANNER_WINDOW_INITIALIZATION_STARTED: 'planner-window-initialization-started',
	GET_EVENTS_FROM_FILES: 'async-message-getEventFiles',
	DELETE_EVENT_FILES: 'async-message-deleteEventFiles'
};


module.exports = {
	APPLICATION_CONSTANTS,
	FRONT_EVENTS,
	EVENT_CONSTANTS
};
