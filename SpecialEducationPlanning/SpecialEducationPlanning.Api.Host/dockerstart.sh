#!/bin/sh
APPDYNAMICS_AGENT_NODE_NAME=$HOSTNAME
export APPDYNAMICS_AGENT_NODE_NAME

#Breaking Change with .NET8 Default port is 8080 however DV uses port 80 
#https://learn.microsoft.com/en-us/dotnet/core/compatibility/containers/8.0/aspnet-port
export ASPNETCORE_URLS=http://*:80


echo '$HOSTNAME='$HOSTNAME
echo '$APPDYNAMICS_AGENT_NODE_NAME='$APPDYNAMICS_AGENT_NODE_NAME
echo '$ASPNETCORE_URLS='$ASPNETCORE_URLS
cd /app
#dotnet "SpecialEducationPlanning
.Api.dll" /CORECLR_ENABLE_PROFILING=$CORECLR_ENABLE_PROFILING

dotnet "SpecialEducationPlanning
.Api.Host.dll" /CORECLR_ENABLE_PROFILING=$CORECLR_ENABLE_PROFILING

