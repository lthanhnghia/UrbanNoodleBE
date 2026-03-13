# build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["UrbanNoodle.csproj", "./"]
RUN dotnet restore "./UrbanNoodle.csproj"

COPY . .
RUN dotnet publish "UrbanNoodle.csproj" -c Release -o /app/publish

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "UrbanNoodle.dll"]