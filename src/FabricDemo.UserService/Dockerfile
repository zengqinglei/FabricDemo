FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 54045

COPY . .
ENTRYPOINT ["dotnet", "FabricDemo.UserService.dll"]
