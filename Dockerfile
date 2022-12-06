FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR $RELEASE_BUILD_PATH

ENTRYPOINT ["dotnet", "MovieCatalogAPI.dll"]
