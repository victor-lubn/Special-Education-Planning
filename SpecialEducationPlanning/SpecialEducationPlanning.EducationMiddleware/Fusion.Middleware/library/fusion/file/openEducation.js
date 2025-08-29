'use strict';
const FileBase = require('./fileBase');

module.exports = class OpenEducation extends FileBase {
	constructor(filePath, fileName, catalogType, cadPlanNo, planCode, builderName, EducationerName, planName) {
		super(filePath, fileName);
		this.catalogType = catalogType ||'';
		this.cadPlanNo = cadPlanNo || planCode ||'';
		this.builderName = builderName ||'';
		this.EducationerName = EducationerName ||'';
		this.planName = planName || '';
	}

	toJSON() {
		let _json = Object.assign(JSON.parse(super.toJSON()), this.getObject());
		return JSON.stringify(_json);
	}

	getObject(){
		return {
			MainUniqueId : this.catalogType,
			CadPlanNo : this.cadPlanNo,
			SalesPerson : this.EducationerName,
			Customer : this.builderName,
			PlanName : this.planName
		};
	}
};

