'use strict';

const { constants } = require('../../../common');


module.exports = function offline(offline) {
    constants.OFFLINE = offline;
}