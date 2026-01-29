# Dockerfile pour UserService
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Services/BookHub.UserService/BookHub.UserService.csproj", "src/Services/BookHub.UserService/"]
COPY ["src/Shared/BookHub.Shared/BookHub.Shared.csproj", "src/Shared/BookHub.Shared/"]

# Restore dependencies
RUN dotnet restore "src/Services/BookHub.UserService/BookHub.UserService.csproj"

# Copy source code
COPY . .

# Build
WORKDIR "/src/src/Services/BookHub.UserService"
RUN dotnet build "BookHub.UserService.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "BookHub.UserService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "BookHub.UserService.dll"]
