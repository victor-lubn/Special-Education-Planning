cd %~dp0
cd ../SpecialEducationPlanning
.Database

msbuild /t:Build /p:Configuration=Release
sqlPackage /Action:Publish /SourceFile:"bin/Release/SpecialEducationPlanning
.Database.dacpac" /TargetConnectionString:"Server=(LocalDb)\MSSQLLocalDB;Database=SpecialEducationPlanning
Database;Trusted_Connection=True;MultipleActiveResultSets=true;" /p:BlockOnPossibleDataLoss=false


cd %~dp0
cd ../SpecialEducationPlanning
.DatabaseCompare

dotnet publish -c Release
dotnet bin\Release\netcoreapp2.0\publish\SpecialEducationPlanning
.DatabaseCompare.dll


rem cd %~dp0

rem sqlpackage.exe /a:Script /sf:../SpecialEducationPlanning
.Database/bin/Release/SpecialEducationPlanning
.Database.dacpac /tf:../SpecialEducationPlanning
.DatabaseCompare/bin/Release/SpecialEducationPlanning
.Database.dacpac /tdn:aspTargetdb /op:AspDbUpdate.sql




pause 