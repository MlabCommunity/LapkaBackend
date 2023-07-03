FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["LapkaBackend.API/LapkaBackend.API.csproj", "LapkaBackend.API/"]
RUN dotnet restore "LapkaBackend.API/LapkaBackend.API.csproj"
COPY . .
WORKDIR "/src/LapkaBackend.API"
RUN dotnet build "LapkaBackend.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LapkaBackend.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LapkaBackend.API.dll"]
