FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Sylvre.WebAPI/Sylvre.WebAPI.csproj", "Sylvre.WebAPI/"]
COPY ["Sylvre.Core/Sylvre.Core.csproj", "Sylvre.Core/"]
RUN dotnet restore "./Sylvre.WebAPI/Sylvre.WebAPI.csproj"
COPY . .
WORKDIR "/src/Sylvre.WebAPI"
RUN dotnet build "./Sylvre.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Sylvre.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sylvre.WebAPI.dll"]