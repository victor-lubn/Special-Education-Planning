'use strict';

const sendMessage = require('./sendMessage');
const Messages = require('../messages');

module.exports = ()=>sendMessage(Messages.HANDSHAKE);