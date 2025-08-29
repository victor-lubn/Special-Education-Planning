module.exports = {
	ffi : require('ffi-napi'),
	fs : require('fs'),
	con : (...arg)=>{console.log(arg);},
	watch : require('node-watch'),
	ipcMain :  require('electron').ipcMain,
    ipcRenderer:  require('electron').ipcRenderer,
	BrowserWindow:  require('electron').BrowserWindow,
	App:  require('electron').App,
	exec : require('child_process').exec,
	execFile: require('child_process').execFile,
	spawn: require('child_process').spawn,
	format : require('util').format,
	path: require('path')
};