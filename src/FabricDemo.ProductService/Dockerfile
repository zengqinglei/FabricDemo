FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 55121
EXPOSE 44361

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/FabricDemo.ProductService/FabricDemo.ProductService.csproj src/FabricDemo.ProductService/
RUN dotnet restore src/FabricDemo.ProductService/FabricDemo.ProductService.csproj
COPY . .
WORKDIR /src/src/FabricDemo.ProductService
RUN dotnet build FabricDemo.ProductService.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish FabricDemo.ProductService.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FabricDemo.ProductService.dll"]
