export class TdpLog {

  private _time: string;
  private _message: string;
  private _level: string;
  private _controller: string;
  private _caller: string;
  private _userName: string;

  constructor(
    message: string,
    level: string,
    controller?: string,
    caller?: string,
    userName?: string
  ) {
    this._time = new Date().toLocaleString();
    this._message = message;
    this._level = level;
    this._controller = controller || null;
    this._caller = caller || null;
    this._userName = userName || null;
  }

  public toString(): string {
    let stringLog = `[${this._time}] ${this._level} ${this._message.toLowerCase()}, Context:`;
    if (this._caller) {
      stringLog += ` at ${this._caller}`;
    }
    if (this._controller) {
      stringLog += ` in ${this._controller}`;
    }
    stringLog += this._userName ? ` AUTHORIZED USER: ${this._userName}` : ` NO USER`;
    return stringLog;
  }
}
