param()

. "$PSScriptRoot/_common.ps1"

$version = GetVersion

$buildType = SetBuildType
$imageName = GetImageName

$combinedTag="${env:DOCKER_REGISTRY}$imageName:$buildType-$version"
$arm="${env:DOCKER_REGISTRY}$imageName:$buildType-$version-linux-arm"
$x64="${env:DOCKER_REGISTRY}$imageName:$buildType-$version-linux-x64"
$win="${env:DOCKER_REGISTRY}$imageName:$buildType-$version-win"

docker manifest create $combinedTag $arm $x64 $win
docker manifest push $combinedTag

$combinedTag="${env:DOCKER_REGISTRY}$imageName:$buildType"
docker manifest create $combinedTag $arm $x64 $win
docker manifest push $combinedTag

if($buildType -eq "release")
{
	$combinedTag="${env:DOCKER_REGISTRY}$imageName:latest"
	docker manifest create $combinedTag $arm $x64 $win
	docker manifest push $combinedTag
}