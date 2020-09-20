#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY [".nuget/nuget.config", ".nuget/nuget.config"]
COPY ["Directory.Build.props", "./"]
COPY ["src/Sannel.House.Devices.Listener/Sannel.House.Devices.Listener.csproj", "src/Sannel.House.Devices.Listener/"]
COPY ["src/Sannel.House.Devices.Services/Sannel.House.Devices.Services.csproj", "src/Sannel.House.Devices.Services/"]
COPY ["src/Sannel.House.Devices.Interfaces/Sannel.House.Devices.Interfaces.csproj", "src/Sannel.House.Devices.Interfaces/"]
COPY ["src/Sannel.House.Devices.Models/Sannel.House.Devices.Models.csproj", "src/Sannel.House.Devices.Models/"]
COPY ["src/Migrations/Sannel.House.Devices.Data.Migrations.Sqlite/Sannel.House.Devices.Data.Migrations.Sqlite.csproj", "src/Migrations/Sannel.House.Devices.Data.Migrations.Sqlite/"]
COPY ["src/Sannel.House.Devices.Data/Sannel.House.Devices.Data.csproj", "src/Sannel.House.Devices.Data/"]
COPY ["src/Migrations/Sannel.House.Devices.Data.Migrations.PostgreSQL/Sannel.House.Devices.Data.Migrations.PostgreSQL.csproj", "src/Migrations/Sannel.House.Devices.Data.Migrations.PostgreSQL/"]
COPY ["src/Sannel.House.Devices.Repositories/Sannel.House.Devices.Repositories.csproj", "src/Sannel.House.Devices.Repositories/"]
COPY ["src/Migrations/Sannel.House.Devices.Data.Migrations.SqlServer/Sannel.House.Devices.Data.Migrations.SqlServer.csproj", "src/Migrations/Sannel.House.Devices.Data.Migrations.SqlServer/"]
RUN dotnet restore  --configfile .nuget/nuget.config "src/Sannel.House.Devices.Listener/Sannel.House.Devices.Listener.csproj"
COPY . .
WORKDIR "/src/src/Sannel.House.Devices.Listener"
RUN dotnet build "Sannel.House.Devices.Listener.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sannel.House.Devices.Listener.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sannel.House.Devices.Listener.dll"]