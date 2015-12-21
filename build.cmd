pushd src

..\tools\nuget restore

msbuild.exe IISManager\IISManager.csproj /t:pipelinePreDeployCopyAllFilesToOneFolder /p:_PackageTempDir="..\..\build";AutoParameterizationWebConfigConnectionStrings=false;Configuration=Release;SolutionDir="."

popd

.\tools\nuget pack .\IISManager.nuspec -BasePath .\build -OutputDirectory .\artifacts