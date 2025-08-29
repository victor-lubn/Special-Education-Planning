'use strict';

module.exports = class FileBase {
	constructor(filePath, fileName) {
		this.filePath = filePath;
		this.fileName = fileName;
	}

	toJSON() {
		let { filePath, fileName } = this;
		return JSON.stringify({ FilePath : filePath, FileName: fileName});
	}
};
