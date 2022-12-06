
FROM buildpack-deps:aspnet:6.0

WORKDIR $RELEASE_BUILD_PATH

ENTRYPOINT ["dotnet", "DotNet.Docker.dll"]
