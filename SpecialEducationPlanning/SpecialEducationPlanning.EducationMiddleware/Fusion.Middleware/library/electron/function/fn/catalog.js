'use strict';
const { lib, constants, messages } = require('../../../common');
const communication = require('../../communication');
var Catalog = require('../../entities/catalog/catalog');
 


async function  checkCatalog(model) {
	let file = constants.APPLICATION_CONSTANTS.FILE_PATH_CATALOG;
	let catalog = Catalog.getInstance();
	if (typeof file !== 'undefined') {
		if (_checkCatalogFile(file)) {
			let contents = lib.fs.readFileSync(file, 'utf8');
			_extractNumbers(catalog, contents);
			
			if(!catalog.getValue().includes(model.catalogType)){
				communication.send(constants.FRONT_EVENTS.CATALOG_DOESNT_EXITS, messages.catalog.NO_CATALOG_IN_YOUR_LIST);
				throw new Error(messages.catalog.NO_CATALOG_IN_YOUR_LIST);
			}
		}
	}else{
		throw new Error(messages.catalog.NO_CATALOGS); 
	}
}

function _extractNumbers(catalog, contents){ 
	if(!catalog.checkValue()){
		catalog.setValue(_getCataloList(contents));
	}
}

function _checkCatalogFile(file) {
	let existsFile = lib.fs.existsSync(file);
	if (!existsFile) {
		//If the file doesn't exist, send the event with the file path like an argument.
		communication.send(constants.FRONT_EVENTS.LICENCE_DOESNT_EXISTS, file);
	}
	return existsFile;
}

function _getCataloList(content) {
	//let catLockRegex  = new RegExp(/(?:CatLock = )\s*(.*)/g);
	//let replaceText = /[" ]/gi;
	let text = new RegExp(/(?:CatLock = )\s*(.*)/g).exec(content)[1];
	let catalogList = text.split(','); 
	let list = [];
	catalogList.forEach((value)=>{
		list.push(value.replace(/[" ]/gi, ''));
	});
	return list;
}


module.exports = checkCatalog;