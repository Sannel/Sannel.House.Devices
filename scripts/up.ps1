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
elseif($Users)
{
	$target = "users";
}

$version = GetVersion

return RunDockerCompose "up" $version $target