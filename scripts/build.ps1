#!/usr/local/bin/pwsh
param(
	[switch]$DevicesOnly
)

. "$PSScriptRoot/_common.ps1"

SetBuildType

$target = "";
if($DevicesOnly)
{
	$target = "devices";
}

# Pull latest images 
docker pull microsoft/dotnet:2.2-aspnetcore-runtime
docker pull microsoft/dotnet:2.2-sdk

CleanDevFiles

$version = GetVersion

return RunDockerCompose "build" $version $target


