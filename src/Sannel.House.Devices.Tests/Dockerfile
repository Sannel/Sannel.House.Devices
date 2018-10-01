#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see http://aka.ms/containercompat 

FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/Sannel.House.Devices.Tests/Sannel.House.Devices.Tests.csproj src/Sannel.House.Devices.Tests/
COPY src/Sannel.House.Devices/Sannel.House.Devices.csproj src/Sannel.House.Devices/
COPY src/Sannel.House.Devices.Interfaces/Sannel.House.Devices.Interfaces.csproj src/Sannel.House.Devices.Interfaces/
COPY src/Sannel.House.Devices.Models/Sannel.House.Devices.Models.csproj src/Sannel.House.Devices.Models/
COPY src/Migrations/Sannel.House.Devices.Data.Migrations.MySql/Sannel.House.Devices.Data.Migrations.MySql.csproj src/Migrations/Sannel.House.Devices.Data.Migrations.MySql/
COPY src/Sannel.House.Devices.Data/Sannel.House.Devices.Data.csproj src/Sannel.House.Devices.Data/
COPY src/Migrations/Sannel.House.Devices.Data.Migrations.Sqlite/Sannel.House.Devices.Data.Migrations.Sqlite.csproj src/Migrations/Sannel.House.Devices.Data.Migrations.Sqlite/
COPY src/Sannel.House.Devices.Repositories/Sannel.House.Devices.Repositories.csproj src/Sannel.House.Devices.Repositories/
COPY src/Migrations/Sannel.House.Devices.Data.Migrations.SqlServer/Sannel.House.Devices.Data.Migrations.SqlServer.csproj src/Migrations/Sannel.House.Devices.Data.Migrations.SqlServer/
RUN dotnet restore src/Sannel.House.Devices.Tests/Sannel.House.Devices.Tests.csproj
COPY . .
WORKDIR /src/src/Sannel.House.Devices.Tests
RUN dotnet build Sannel.House.Devices.Tests.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Sannel.House.Devices.Tests.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet","test", "Sannel.House.Devices.Tests.dll"]
