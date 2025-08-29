const { contextBridge, ipcRenderer, shell } = require('electron');

const allowedChannels = {
  TDP_LOG : 'new-tdp-log',
  GET_HOSTNAME: 'get-hostname',
}

/**
 * Due to security features like context isolation and sandbox we need to expose only small subset of features that we use in
 * renderer process and ideally we should add all allowed channels that we are using on frontend so that we can add additional check 
 * for send and receive methods
 */
contextBridge.exposeInMainWorld('electronAPI', {
  ipc: {
    //from renderer to main process
    send: (channel, args) => ipcRenderer.send(channel, args),

    //from main process to renderer
    receive: (channel, listener) => ipcRenderer.on(channel, (event, ...args) => listener(event, ...args)),

    //retreive hostname from main process
    getHostname: () => ipcRenderer.sendSync(allowedChannels.GET_HOSTNAME),

    //send log from renderer to main process
    tdpLog: (arg) => ipcRenderer.send(allowedChannels.TDP_LOG, arg),

    //open links in default browser
    openLinkExternally: (url) => ipcRenderer.invoke('open-link-externally', url),
  }
});