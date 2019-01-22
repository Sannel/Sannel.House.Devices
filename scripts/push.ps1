#!/usr/local/bin/pwsh
param(
)

$target = "devices"
. "$PSScriptRoot/_common.ps1"

SetBuildType
CleanDevFiles

$version = GetVersion
TryLogin

return RunDockerCompose "push" $version $target


