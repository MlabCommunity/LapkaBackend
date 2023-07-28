#Build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "./LapkaBackend.API/LapkaBackend.API.csproj" --disable-parallel
RUN dotnet publish "./LapkaBackend.API/LapkaBackend.API.csproj" -c release -o /app --no-restore

#Serve
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./

EXPOSE 5072
ENTRYPOINT ["dotnet", "LapkaBackend.API.dll"]