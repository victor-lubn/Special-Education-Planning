FROM crtdpdvwnonprduks001.azurecr.io/aiep/trade/dotnet/core/aspnet:8.0 as base

WORKDIR /app
EXPOSE 80

COPY bin/Release/net8.0/publish .
COPY dockerstart.sh .
RUN chmod -R +x dockerstart.sh
RUN chmod -R 777 dockerstart.sh
CMD [ "sh", "/app/dockerstart.sh", "" ]