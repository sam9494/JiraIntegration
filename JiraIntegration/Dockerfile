FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["JiraIntegration/JiraIntegration.csproj", "JiraIntegration/"]
RUN dotnet restore "JiraIntegration/JiraIntegration.csproj"
COPY . .
WORKDIR "/src/JiraIntegration"
RUN dotnet build "JiraIntegration.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JiraIntegration.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JiraIntegration.dll"]
