FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 54452

COPY . .
ENTRYPOINT ["dotnet", "FabricDemo.IdentityServer.dll"]
