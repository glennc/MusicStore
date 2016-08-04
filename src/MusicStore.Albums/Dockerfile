FROM troydai/aspnetcore-production:1.0.0
WORKDIR "/app"
ENTRYPOINT ["dotnet", "MusicStore.Albums.dll"]
ADD ./bin/release/netcoreapp1.0/publish/ /app