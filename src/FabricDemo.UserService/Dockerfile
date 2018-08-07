FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 54045

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/FabricDemo.UserService/FabricDemo.UserService.csproj src/FabricDemo.UserService/
RUN dotnet restore src/FabricDemo.UserService/FabricDemo.UserService.csproj
COPY . .
WORKDIR /src/src/FabricDemo.UserService
RUN dotnet build FabricDemo.UserService.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish FabricDemo.UserService.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FabricDemo.UserService.dll"]
