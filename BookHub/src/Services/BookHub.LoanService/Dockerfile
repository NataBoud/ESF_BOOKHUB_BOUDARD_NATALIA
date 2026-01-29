FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Services/BookHub.LoanService/BookHub.LoanService.csproj", "src/Services/BookHub.LoanService/"]
COPY ["src/Shared/BookHub.Shared/BookHub.Shared.csproj", "src/Shared/BookHub.Shared/"]

RUN dotnet restore "src/Services/BookHub.LoanService/BookHub.LoanService.csproj"

COPY . .

WORKDIR "/src/src/Services/BookHub.LoanService"
RUN dotnet build "BookHub.LoanService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookHub.LoanService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "BookHub.LoanService.dll"]
