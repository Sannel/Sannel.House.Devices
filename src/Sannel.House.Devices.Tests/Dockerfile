# base
FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 443

#build the software
FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY . .
WORKDIR "/src/src/Sannel.House.Devices.Tests"
RUN dotnet build "Sannel.House.Devices.Tests.csproj" -c Debug
RUN dotnet test /p:AltCover=true /p:AltCoverAssemblyExcludeFilter="Tests`|Migrations" /p:AltCoverTypeFilter="Startup`|Program" "Sannel.House.Devices.Tests.csproj"
RUN dotnet tool install --global dotnet-reportgenerator-globaltool
RUN reportgenerator "-reports:coverage.xml" "-targetdir:/coveragereport"

#prepare the image
FROM sannel/staticfileserver AS final
COPY --from=build /coveragereport /app/wwwroot
