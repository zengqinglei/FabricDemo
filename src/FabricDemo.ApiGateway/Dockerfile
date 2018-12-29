FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 54479

COPY . .
ENTRYPOINT ["dotnet", "FabricDemo.ApiGateway.dll"]
