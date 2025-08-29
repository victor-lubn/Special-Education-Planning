'use strict';
const FileBase = require('./fileBase');

module.exports = class NewEducation extends FileBase {
	constructor(filePath, fileName, dLTOverrides, mainUniqueId, cadPlanNo, customer, salesPerson, planCode, planName) {
		super(filePath, fileName);
		this.dLTOverrides = dLTOverrides || '';
		this.mainUniqueId = mainUniqueId || '';
		this.cadPlanNo = cadPlanNo || planCode || '';
		this.salesPerson = salesPerson || '';
		this.customer = customer || '';
		this.planName = planName || '';
	}

	toJSON() {
		let _json = Object.assign(JSON.parse(super.toJSON()), this.getObject());
		return JSON.stringify(_json);
	}

	getObject(){
		return {
			DLTOverrides : this.dLTOverrides,
			MainUniqueId : this.mainUniqueId,
			CadPlanNo : this.cadPlanNo,
			SalesPerson : this.salesPerson,
			Customer : this.customer,
			PlanName : this.planName
		};
	}
};

