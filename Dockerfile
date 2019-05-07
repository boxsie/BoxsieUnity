FROM microsoft/dotnet:2.0-sdk
# copy and build everything else
COPY . ./app
WORKDIR /app/Boxsie.Server.Console
RUN dotnet restore
RUN dotnet publish -c Release -o out
EXPOSE 8558
ENTRYPOINT ["dotnet", "out/Boxsie.Server.Console.dll"]