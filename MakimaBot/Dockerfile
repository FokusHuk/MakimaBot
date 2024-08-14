FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Use the official .NET Core SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the .csproj and restore as distinct layers
COPY ./MakimaBot.csproj ./
RUN dotnet restore "MakimaBot.csproj"

# Copy the remaining source code and build the application
COPY . .
WORKDIR "/src"
RUN dotnet build "MakimaBot.csproj" -c Release -o /app/build

# Build the runtime image
FROM build AS publish
RUN dotnet publish "MakimaBot.csproj" -c Release -o /app/publish

# Entry point when the container starts
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MakimaBot.dll"]
