# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy central build configuration
COPY ["Directory.Build.props", "Directory.Packages.props", "./"]

# Copy project files
COPY ["SportMatrix.AIAssistant/SportMatrix.AIAssistant.csproj", "SportMatrix.AIAssistant/"]

# Restore NuGet packages
RUN dotnet restore "SportMatrix.AIAssistant/SportMatrix.AIAssistant.csproj"

# Copy source code
COPY ["SportMatrix.AIAssistant/", "SportMatrix.AIAssistant/"]

# Build the application
WORKDIR "/src/SportMatrix.AIAssistant"
RUN dotnet build "SportMatrix.AIAssistant.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "SportMatrix.AIAssistant.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Standard ASP.NET Core environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Use non-root user explicitly
USER 1001

ENTRYPOINT ["dotnet", "SportMatrix.AIAssistant.dll"]
