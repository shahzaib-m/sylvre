FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["Sylvre.WebAPI/Sylvre.WebAPI.csproj", "Sylvre.WebAPI/"]
COPY ["Sylvre.Core/Sylvre.Core.csproj", "Sylvre.Core/"]
RUN dotnet restore "Sylvre.WebAPI/Sylvre.WebAPI.csproj"
COPY . .
WORKDIR "/src/Sylvre.WebAPI"
RUN dotnet build "Sylvre.WebAPI.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Sylvre.WebAPI.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Sylvre.WebAPI.dll"]