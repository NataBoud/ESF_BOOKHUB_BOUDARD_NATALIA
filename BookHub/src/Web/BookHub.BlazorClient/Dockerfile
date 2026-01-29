# Dockerfile pour Blazor WebAssembly Client
# TODO: À COMPLÉTER PAR LE CANDIDAT
#
# Ce Dockerfile doit :
# 1. Builder l'application Blazor WASM
# 2. Servir les fichiers statiques via Nginx
#
# Indices :
# - Utiliser dotnet publish pour générer les fichiers WASM
# - Les fichiers publiés seront dans /app/publish/wwwroot
# - Utiliser nginx:alpine comme image de runtime

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/Web/BookHub.BlazorClient/BookHub.BlazorClient.csproj", "src/Web/BookHub.BlazorClient/"]
COPY ["src/Shared/BookHub.Shared/BookHub.Shared.csproj", "src/Shared/BookHub.Shared/"]

# Restore dependencies
RUN dotnet restore "src/Web/BookHub.BlazorClient/BookHub.BlazorClient.csproj"

# Copy source code
COPY . .

# Build and publish
WORKDIR "/src/src/Web/BookHub.BlazorClient"
RUN dotnet publish "BookHub.BlazorClient.csproj" -c Release -o /app/publish

# Runtime stage with Nginx
FROM nginx:alpine AS final

# Copy published Blazor WASM files
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

# Copy nginx configuration
COPY src/Web/BookHub.BlazorClient/nginx.conf /etc/nginx/nginx.conf

EXPOSE 80
