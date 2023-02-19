FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TfGM-API-Wrapper/TfGM-API-Wrapper.csproj", "TfGM-API-Wrapper/"]
RUN dotnet restore "TfGM-API-Wrapper/TfGM-API-Wrapper.csproj"
COPY . .
WORKDIR "/src/TfGM-API-Wrapper"
RUN dotnet build "TfGM-API-Wrapper.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TfGM-API-Wrapper.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TfGM-API-Wrapper.dll"]
