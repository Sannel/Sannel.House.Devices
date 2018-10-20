
# base
FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 443

#build the software
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Directory.Build.props", "./"]
COPY ["src/Sannel.House.Devices/Sannel.House.Devices.csproj", "src/Sannel.House.Devices/"]
COPY ["src/Sannel.House.Devices.Tests/Sannel.House.Devices.Tests.csproj", "src/Sannel.House.Devices.Tests/"]
COPY ["src/Sannel.House.Devices.Interfaces/Sannel.House.Devices.Interfaces.csproj", "src/Sannel.House.Devices.Interfaces/"]
COPY ["src/Sannel.House.Devices.Models/Sannel.House.Devices.Models.csproj", "src/Sannel.House.Devices.Models/"]
COPY ["src/Migrations/Sannel.House.Devices.Data.Migrations.MySql/Sannel.House.Devices.Data.Migrations.MySql.csproj", "src/Migrations/Sannel.House.Devices.Data.Migrations.MySql/"]
COPY ["src/Sannel.House.Devices.Data/Sannel.House.Devices.Data.csproj", "src/Sannel.House.Devices.Data/"]
COPY ["src/Migrations/Sannel.House.Devices.Data.Migrations.Sqlite/Sannel.House.Devices.Data.Migrations.Sqlite.csproj", "src/Migrations/Sannel.House.Devices.Data.Migrations.Sqlite/"]
COPY ["src/Sannel.House.Devices.Repositories/Sannel.House.Devices.Repositories.csproj", "src/Sannel.House.Devices.Repositories/"]
COPY ["src/Migrations/Sannel.House.Devices.Data.Migrations.SqlServer/Sannel.House.Devices.Data.Migrations.SqlServer.csproj", "src/Migrations/Sannel.House.Devices.Data.Migrations.SqlServer/"]
RUN dotnet restore "src/Sannel.House.Devices/Sannel.House.Devices.csproj"
COPY . .
WORKDIR "/src/src/Sannel.House.Devices"
RUN dotnet build "Sannel.House.Devices.csproj" -c Release

#test the software
FROM build as Test
WORKDIR "/src/src/Sannel.House.Devices.Tests"
RUN dotnet build "Sannel.House.Devices.Tests.csproj" -c Debug
RUN dotnet test "Sannel.House.Devices.Tests.csproj"

#publish the software
FROM build AS publish
WORKDIR "/src/src/Sannel.House.Devices"
RUN dotnet publish "Sannel.House.Devices.csproj" -c Release -o /app

#prepare the image
FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Sannel.House.Devices.dll"]