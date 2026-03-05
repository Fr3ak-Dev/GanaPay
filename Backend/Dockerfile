# ==================== STAGE 1: BUILD ====================
# Imagen base con SDK para compilar el proyecto y dependencias
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos .csproj y restaurar dependencias, primero esto para aprovechar el cache de Docker
COPY ["GanaPay.API/GanaPay.API.csproj", "GanaPay.API/"]
COPY ["GanaPay.Application/GanaPay.Application.csproj", "GanaPay.Application/"]
COPY ["GanaPay.Core/GanaPay.Core.csproj", "GanaPay.Core/"]
COPY ["GanaPay.Infrastructure/GanaPay.Infrastructure.csproj", "GanaPay.Infrastructure/"]

RUN dotnet restore "GanaPay.API/GanaPay.API.csproj"

# Copiar todo el código y compilar optimizado para desarrollo
COPY . .
WORKDIR "/src/GanaPay.API"
RUN dotnet build "GanaPay.API.csproj" -c Release -o /app/build

# ==================== STAGE 2: PUBLISH ====================
# Optimizar los archivos compilados para producción
FROM build AS publish
RUN dotnet publish "GanaPay.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==================== STAGE 3: FINAL ====================
# Imagen base solo con runtime para ejecutar
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Exponer puertos
EXPOSE 80
EXPOSE 443

# Copiar archivos publicados del stage anterior
COPY --from=publish /app/publish .

# Comando para ejecutar la aplicación
ENTRYPOINT ["dotnet", "GanaPay.API.dll"]