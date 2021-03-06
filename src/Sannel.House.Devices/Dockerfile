
# base
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 8080

#build the software
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY [".nuget/nuget.config", ".nuget/nuget.config"]
COPY ["Directory.Build.props", "./"]
COPY ["src/Sannel.House.Devices/Sannel.House.Devices.csproj", "src/Sannel.House.Devices/"]
COPY ["src/Sannel.House.Devices.Tests/Sannel.House.Devices.Tests.csproj", "src/Sannel.House.Devices.Tests/"]
COPY ["src/Sannel.House.Devices.Interfaces/Sannel.House.Devices.Interfaces.csproj", "src/Sannel.House.Devices.Interfaces/"]
COPY ["src/Sannel.House.Devices.Models/Sannel.House.Devices.Models.csproj", "src/Sannel.House.Devices.Models/"]
#COPY ["src/Migrations/Sannel.House.Devices.Data.Migrations.MySql/Sannel.House.Devices.Data.Migrations.MySql.csproj", "src/Migrations/Sannel.House.Devices.Data.Migrations.MySql/"]
COPY ["src/Sannel.House.Devices.Data/Sannel.House.Devices.Data.csproj", "src/Sannel.House.Devices.Data/"]
COPY ["src/Migrations/Sannel.House.Devices.Data.Migrations.Sqlite/Sannel.House.Devices.Data.Migrations.Sqlite.csproj", "src/Migrations/Sannel.House.Devices.Data.Migrations.Sqlite/"]
COPY ["src/Sannel.House.Devices.Repositories/Sannel.House.Devices.Repositories.csproj", "src/Sannel.House.Devices.Repositories/"]
COPY ["src/Sannel.House.Devices.Services/Sannel.House.Devices.Services.csproj", "src/Sannel.House.Devices.Services/"]
COPY ["src/Migrations/Sannel.House.Devices.Data.Migrations.SqlServer/Sannel.House.Devices.Data.Migrations.SqlServer.csproj", "src/Migrations/Sannel.House.Devices.Data.Migrations.SqlServer/"]
RUN dotnet restore --configfile .nuget/nuget.config "src/Sannel.House.Devices/Sannel.House.Devices.csproj"
COPY . .
WORKDIR "/src/src/Sannel.House.Devices"
RUN dotnet build "Sannel.House.Devices.csproj" -c Release


#publish the software
FROM build AS publish
WORKDIR "/src/src/Sannel.House.Devices"
RUN dotnet publish "Sannel.House.Devices.csproj" -c Release -o /app

#prepare the image
FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=build /src/src/Sannel.House.Devices/app_config /app/app_config
COPY --from=build /src/src/Sannel.House.Devices/app_data /app/app_data
ENTRYPOINT ["dotnet", "Sannel.House.Devices.dll"]