var assert = require('assert');
var {  constants } = require('../library/common');
var  documentFunctions = require('./library/electron/document/util');

describe('Documents', function () {
    let documentText = new Buffer("hello word");
    let fileId = "Test";
    let path;

    describe('Create document and convert to ' + constants.APPLICATION_CONSTANTS.FILE_FORMAT, function () {

        it('should create the file in the path:' + constants.APPLICATION_CONSTANTS.PATH_DOCUMENT_BASE, function () {
            path = documentFunctins.createFileAndReturnPath(documentText, fileId);
            assert.notEqual(documentText, null);
        });

        it('should read the file in the current path and get the same array of bytes', function(){
            let array = documentFunctions.convertFileToArray(path);
            assert.equal(documentText, array);
        });

    });

});

