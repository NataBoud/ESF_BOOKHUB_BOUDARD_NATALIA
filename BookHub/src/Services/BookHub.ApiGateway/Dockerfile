# -----------------------------
# Build stage
# -----------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier les fichiers projets
COPY ["src/Services/BookHub.ApiGateway/BookHub.ApiGateway.csproj", "src/Services/BookHub.ApiGateway/"]
COPY ["src/Shared/BookHub.Shared/BookHub.Shared.csproj", "src/Shared/BookHub.Shared/"]

# Restaurer les dépendances
RUN dotnet restore "src/Services/BookHub.ApiGateway/BookHub.ApiGateway.csproj"

# Copier tout le code source
COPY . .

# Construire le projet
WORKDIR "/src/src/Services/BookHub.ApiGateway"
RUN dotnet build "BookHub.ApiGateway.csproj" -c Release -o /app/build

# -----------------------------
# Publish stage
# -----------------------------
FROM build AS publish
RUN dotnet publish "BookHub.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -----------------------------
# Runtime stage
# -----------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Créer un utilisateur non-root
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Copier les fichiers publiés
COPY --from=publish /app/publish .

# Copier le fichier ocelot.json dans le même répertoire que BookHub.ApiGateway.dll
COPY src/Services/BookHub.ApiGateway/ocelot.json /app/ocelot.json

# Exposer le port
EXPOSE 5167

# Point d'entrée
ENTRYPOINT ["dotnet", "BookHub.ApiGateway.dll"]
