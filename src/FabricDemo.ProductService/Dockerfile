FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 55121

COPY . .
ENTRYPOINT ["dotnet", "FabricDemo.ProductService.dll"]
