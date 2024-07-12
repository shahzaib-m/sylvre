# Build Core/WebAPI project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS core-webapi-build
ARG BUILD_CONFIGURATION=Release
WORKDIR /core-and-webapi
COPY ["core-and-webapi/Sylvre.WebAPI/Sylvre.WebAPI.csproj", "Sylvre.WebAPI/"]
COPY ["core-and-webapi/Sylvre.Core/Sylvre.Core.csproj", "Sylvre.Core/"]
RUN dotnet restore "Sylvre.WebAPI/Sylvre.WebAPI.csproj"
COPY core-and-webapi/. .
WORKDIR "/core-and-webapi/Sylvre.WebAPI"
RUN dotnet build "Sylvre.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "Sylvre.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# build webapp client project
FROM node:lts-alpine AS webapp-client-build
WORKDIR /webapp-client
COPY webapp-client/package*.json ./
RUN npm ci
COPY webapp-client/. .
RUN npm run build

# combine both builds, static webapp client files to be served by aspnet
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=core-webapi-build /app/publish .
COPY --from=webapp-client-build /webapp-client/dist /app/wwwroot
EXPOSE 8080
ENTRYPOINT ["dotnet", "Sylvre.WebAPI.dll"]
