# Education View

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 11.0.0 and use [Electron](https://electronjs.org/docs) in 10.1.5 version.

Provides an environment for Angular-Cli + Electron desktop apps development full configured with development server (live realod) for web and desktop, run app in production mode and executable files generation for Windows, Linux and MacOS.

## Getting Started
### Prerequisites
This must be installed globally in your computer:
- [NodeJS](https://nodejs.org/en/download/) version 14.16.0
- Angular/cli version 11.0.0
- Typescript version 4.0.5 

#### **Dependency installation**

1. Clone this repository locally.
2. In command line navigate to repository.
3. Run `npm install --global --production windows-build-tools@4.0.0`
4. Install C++ build tools in Visual Studio (Go to VS installer, press modify, go to Desktop development with C++ - isntall the build tools: e.g.MSVC v142 вЂ“ VS 2019 C++). 



5. Run three commands `npm install`, `npm audit fix` and `npm run start` for each of the folders:
    1. SpecialEducationPlanning
.EducationMiddleware/Fusion.Middleware
    2. TradeEducationPlatfrom.EducationMiddleware/Offline.Middleware
    3. SpecialEducationPlanning
.Web

    *Note: If you are getting C++ build tools errors when running `npm install`, try running `npm config set msvs_version 2017`*

#### **VS Code setup for running the app in 'Run and Debug' mode**
1. Create folder called .vscode in web project
2. In .vscode create file called launch.json
3. Paste below code:
```
{
    // Use IntelliSense to learn about possible attributes.
    // Hover to view descriptions of existing attributes.
    // For more information, visit: https://go.microsoft.com/fwlink/?linkid=830387
    "version": "0.2.0",
    "configurations": [
      {
        "type": "node",
        "request": "launch",
        "name": "Electron Main",
        "runtimeExecutable": "${workspaceFolder}/node_modules/.bin/electron",
        "program": "${workspaceFolder}/wwwroot/electron.js",
        "args": [
          ".",
          "--dev"
        ]
      }
    ]
  }
  ```
  4. You should be able to see the Electron Main debug option in the 'Debug and Run'. You can also press F5 to start debugging.



## Development

Run `npm start` to run a development webpack server with the output of the build. This server don't open automatically an instance of electron, after it we have to execute one more command:
- `npm run electron`: For development, with live realoading and source maps.

### Other Commands

|Command|Description|
|--|--|
|`npm run start:prod`| Build your app in production mode running on an instance of electron |
|`npm run start:web`| Run your app in local development server and open it on the browser |
|`npm run package:win`| Generates executable files for Windows 32/64 bits systems |
|`npm run package:linux`| Generates executable files for Linux 32/64 bits systems |
|`npm run package:osx`| Generates executable files for MacOS 32/64 bits systems |
|`npm run package:all`| Generates executable files for all platforms |

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via [Protractor](http://www.protractortest.org/).

