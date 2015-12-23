call "C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\VsDevCmd.bat"

pushd src

..\tools\nuget restore

msbuild.exe IISManager\IISManager.csproj /t:pipelinePreDeployCopyAllFilesToOneFolder /p:_PackageTempDir="..\..\build";AutoParameterizationWebConfigConnectionStrings=false;Configuration=Release;SolutionDir="."

popd

.\tools\nuget pack .\IISManager.nuspec -BasePath .\build -OutputDirectory .\artifacts