FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 54479
EXPOSE 44385

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/FabricDemo.ApiGateway/FabricDemo.ApiGateway.csproj src/FabricDemo.ApiGateway/
RUN dotnet restore src/FabricDemo.ApiGateway/FabricDemo.ApiGateway.csproj
COPY . .
WORKDIR /src/src/FabricDemo.ApiGateway
RUN dotnet build FabricDemo.ApiGateway.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish FabricDemo.ApiGateway.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FabricDemo.ApiGateway.dll"]
