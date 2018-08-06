FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 54452
EXPOSE 44324

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/FabricDemo.IdentityServer/FabricDemo.IdentityServer.csproj src/FabricDemo.IdentityServer/
RUN dotnet restore src/FabricDemo.IdentityServer/FabricDemo.IdentityServer.csproj
COPY . .
WORKDIR /src/src/FabricDemo.IdentityServer
RUN dotnet build FabricDemo.IdentityServer.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish FabricDemo.IdentityServer.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FabricDemo.IdentityServer.dll"]
