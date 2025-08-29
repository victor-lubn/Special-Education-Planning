
module.exports =  (function () {
	var instance;
 
	function createInstance() {
		var object = new Object('Catalogs');
		object.catalogs = [];
		object.check = false;
		object.getValue = ()=>{
			return object.catalogs;
		};
		object.setValue = (list)=>{
			object.check = true;
			return object.catalogs = list;
		};
		object.checkValue =()=>{
			return object.check;
		};
		return object;
	}
 
	return {
		getInstance: function () {
			if (!instance) {
				instance = createInstance();
			}
			return instance;    
		}
	};
})();