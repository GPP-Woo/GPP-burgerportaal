# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN mkdir -p /app/data/images && chown -R app:app /app/data
USER app
WORKDIR /app
EXPOSE 8080

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS with-node
RUN apt-get update
RUN apt-get -y install curl
RUN curl -sL https://deb.nodesource.com/setup_20.x | bash
RUN apt-get -y install nodejs

FROM with-node AS restore
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["odbp.client/nuget.config", "odbp.client/"]
COPY ["ODBP.Server/ODBP.csproj", "ODBP.Server/"]
COPY ["odbp.client/odbp.client.esproj", "odbp.client/"]
RUN dotnet restore "./ODBP.Server/ODBP.csproj"
COPY ["odbp.client/package*.json", "odbp.client/"]
RUN npm install -C /src/odbp.client

# This stage is used to publish the service project to be copied to the final stage
FROM restore AS publish
ARG BUILD_CONFIGURATION=Release VERSION=0.0.0 REVISION=dev
COPY . .
WORKDIR "/src/ODBP.Server"
RUN dotnet publish "./ODBP.csproj" -c $BUILD_CONFIGURATION --no-restore -o /app/publish /p:UseAppHost=false /p:Version=$VERSION /p:SourceRevisionId=$REVISION

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ODBP.dll"]