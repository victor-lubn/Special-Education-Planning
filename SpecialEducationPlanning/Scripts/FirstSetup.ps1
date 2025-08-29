#Requires -RunAsAdministrator

function CleanupAndInstallDependencies {
  Remove-Item package-lock.json -ErrorAction Ignore
  Remove-Item  ./node_modules -Force -Recurse -ErrorAction Ignore
  npm install
}

function InstallAllDependencies {
  cd ../SpecialEducationPlanning
.EducationMiddleware/Fusion.Middleware
  CleanupAndInstallDependencies
  npm run build

  cd ../Offline.Middleware
  CleanupAndInstallDependencies
  npm run build

  cd ../../SpecialEducationPlanning
.Web
  CleanupAndInstallDependencies

  echo "Dependencies installed"
  echo "The project is ready to start coding and packaging! :)"
}

$nodeVersion = Read-Host 'Type a custom node version (optional)'
if (-Not $nodeVersion) {
  $nodeVersion = "14.15.1"
}
nvm install $nodeVersion
nvm use $nodeVersion
Start-Sleep -s 1
$pathNode = (Get-Command node).Path
Start-Sleep -s 1
$currentNodeVersion = & $pathNode '-v' | Out-String
Start-Sleep -s 1
$nodeVersion = ("v" + $nodeVersion) | Out-String
Start-Sleep -s 1
if ($nodeVersion -eq $currentNodeVersion) {
  if (npm list -g windows-build-tools | Select-String '(empty)') {
    npm install --global --production windows-build-tools
  }

  if (npm list -g node-gyp | Select-String '(empty)') {
    npm install -g node-gyp
  }
  InstallAllDependencies
} else {
  echo "Version cannot be switched properly"
  echo "- Make sure that nodejs is not installed (nvm and nodejs cannot work together)"
  echo "- Make sure that you have admin rights"
}

