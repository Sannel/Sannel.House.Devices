#!/usr/local/bin/pwsh
param(
	[switch]$DevicesOnly,
	[switch]$MainOnly
)

. "$PSScriptRoot/_common.ps1"

SetBuildType

$target = "";
if($DevicesOnly -or $MainOnly)
{
	$target = "devices";
}

# Pull latest images 
docker pull microsoft/dotnet:2.2-aspnetcore-runtime
docker pull microsoft/dotnet:2.2-sdk

CleanDevFiles

$version = GetVersion

return RunDockerCompose "push" $version $target


