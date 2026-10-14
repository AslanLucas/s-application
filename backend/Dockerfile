FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY Notes.Api/Notes.Api.csproj Notes.Api/
RUN dotnet restore Notes.Api/Notes.Api.csproj
COPY Notes.Api/ Notes.Api/
RUN dotnet publish Notes.Api/Notes.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "Notes.Api.dll"]
