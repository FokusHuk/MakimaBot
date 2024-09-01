FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /src

COPY ./MakimaBot/MakimaBot.csproj ./MakimaBot/
COPY ./MakimaBot.Tests/MakimaBot.Tests.csproj ./MakimaBot.Tests/

RUN dotnet restore "./MakimaBot/MakimaBot.csproj"
RUN dotnet restore "./MakimaBot.Tests/MakimaBot.Tests.csproj"

COPY ./MakimaBot ./MakimaBot/
COPY ./MakimaBot.Tests ./MakimaBot.Tests/

# Stage for building the app
FROM base AS build-app
WORKDIR /src/MakimaBot
RUN dotnet build -c Release -o /app/build

FROM build-app AS publish
WORKDIR /src/MakimaBot
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS application
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MakimaBot.dll"]

# Stage for building tests
FROM base AS build-tests
WORKDIR /src/MakimaBot.Tests
RUN dotnet build -c Release -o /app/build

FROM build-tests AS tests
WORKDIR /src/MakimaBot.Tests
ENTRYPOINT ["dotnet", "test", "--no-build", "--logger:console;verbosity=normal"]
