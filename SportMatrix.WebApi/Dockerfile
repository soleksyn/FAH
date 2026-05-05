# Base image for .NET 8 runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image for .NET 8 SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy central build configuration
COPY ["Directory.Build.props", "Directory.Packages.props", "./"]

# Copy project files
COPY ["SportMatrix.WebApi/SportMatrix.WebApi.csproj", "SportMatrix.WebApi/"]
COPY ["SportMatrix.Application/SportMatrix.Application.csproj", "SportMatrix.Application/"]
COPY ["SportMatrix.Domain/SportMatrix.Domain.csproj", "SportMatrix.Domain/"]
COPY ["SportMatrix.Infrastructure/SportMatrix.Infrastructure.csproj", "SportMatrix.Infrastructure/"]

# Restore NuGet packages
RUN dotnet restore "SportMatrix.WebApi/SportMatrix.WebApi.csproj"

# Copy source code
COPY ["SportMatrix.WebApi/", "SportMatrix.WebApi/"]
COPY ["SportMatrix.Application/", "SportMatrix.Application/"]
COPY ["SportMatrix.Domain/", "SportMatrix.Domain/"]
COPY ["SportMatrix.Infrastructure/", "SportMatrix.Infrastructure/"]

# Build the application
WORKDIR "/src/SportMatrix.WebApi"
RUN dotnet build "SportMatrix.WebApi.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "SportMatrix.WebApi.csproj" -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Standard ASP.NET Core environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Use non-root user explicitly
USER 1001

ENTRYPOINT ["dotnet", "SportMatrix.WebApi.dll"]
