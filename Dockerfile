# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "./BlogWebApiDotNet.csproj" --disable-parallel
RUN dotnet publish "./BlogWebApiDotNet.csproj" -c release -o /app --no-restore

# serve
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "BlogWebApiDotNet.dll"]
