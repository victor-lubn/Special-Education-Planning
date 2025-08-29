/**
 * Events names.
 */
const EVENTS_NAMES = {

	SEND_EVENT_FRONT: 'async-message-front',

	RECEIVE_EVENT_FRONT: 'async-message-front',

	SEND_EVENT_MIDDELWARE: 'async-message-middelware',

	RECEIVE_EVENT_MIDDELWARE: 'async-message-middelware',

	HANDSHAKING_EVENT: 'async-message-handShanking',

	BUILDER_SAVE_EVENT: 'async-message-save',

	BUILDER_QUIT_EVENT: 'async-message-quit',

	BUILDER_NEW_AND_SAVE_EVENT: 'async-message-newAndSave',

	BUILDER_OPEN: 'async-message-open',

	CHECK_LICENCE : 'sync-check-licence',

	FOCUS_PROGRAM: 'sync-focus-program',

	CLOSE_PROGRAM:'sync-close-program',

	CHECK_FUSION: 'check-fusion',

	SHOW_FUSION: 'show-fusion-program',

	GET_FUSION_VERSION: 'get-fusion-version',

	GET_ROM_AND_PREVIEW: 'get-rom-and-preview',

	GET_PLAN_AUTOSAVE: 'get-plan-autosave'
};
module.exports = EVENTS_NAMES;