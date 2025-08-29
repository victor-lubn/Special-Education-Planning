'use strict';

const FileBase = require('./fileBase');

class CloseEducation extends FileBase {
	
	constructor(filePath, fileName, lineItems, imagePath, mainUniqueId, mainRange) {
		super(filePath, fileName);
		this.lineItems = lineItems || [];
		this.imagePath = imagePath;
		this.mainUniqueId = mainUniqueId;
		this.mainRange = mainRange;
	}
}

module.exports = CloseEducation;
