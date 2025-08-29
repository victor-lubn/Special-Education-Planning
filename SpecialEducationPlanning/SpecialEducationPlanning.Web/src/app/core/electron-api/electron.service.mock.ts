/* Mock Electron Service for components unit testing */
export class ElectronServiceMock {
  get ipcRenderer() {
    return {
      on: (event, callback) => {}
    };
  }
}
