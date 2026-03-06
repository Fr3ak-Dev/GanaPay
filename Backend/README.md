# GanaPay Backend

Sistema de pagos y transferencias digitales desarrolado en .NET 8, SQL Server y MongoDB.

## 🚀 Quick Start

### 1. Clonar el repositorio
```bash
git clone https://github.com/Fr3ak-Dev/GanaPay
cd Backend
```

### 2. Configurar variables de entorno
```bash

# Copiar el template
cp .env.example .env

# Editar .env con tus valores reales
nano .env  # o tu editor favorito
```

### 3. Ejecutar con Docker
```bash
docker-compose up -d
```

### 4. Acceder a Swagger
```
http://localhost:5057/swagger
```

## 🔐 Variables de Entorno

Edita el archivo `.env` con tus credenciales:

- `SQL_CONNECTION_STRING`: Connection string de SQL Server
- `MONGO_CONNECTION_STRING`: Connection string de MongoDB Atlas
- `JWT_SECRET_KEY`: Clave secreta para JWT (mínimo 32 caracteres)

Ver `.env.example` para más detalles.

## 📊 Arquitectura
```
┌─────────────────────────────────────┐
│  API (.NET 8)                       │
├─────────────────────────────────────┤
│  - Clean Architecture               │
│  - Repository Pattern               │
│  - Unit of Work                     │
│  - JWT Authentication               │
└──────────┬──────────────┬───────────┘
           │              │
    ┌──────▼──────┐  ┌────▼───────┐
    │ SQL Server  │  │  MongoDB   │
    │(Transac.)   │  │(Auditoría) │
    └─────────────┘  └────────────┘
```

## 🛠️ Tecnologías

- .NET 8
- EF Core 8
- SQL Server
- MongoDB
- JWT Authentication
- AutoMapper
- FluentValidation
- Swagger/OpenAPI

## 📝 Endpoints

Ver documentación completa en `/swagger`

**Autenticación:**
- POST `/api/auth/register` - Registrar usuario
- POST `/api/auth/login` - Login

**Transacciones:**
- POST `/api/transacciones/transferir` - Realizar transferencia
- GET `/api/transacciones/historial/{cuentaId}` - Ver historial

**Auditoría (Admin):**
- GET `/api/audit/recientes` - Logs recientes
- GET `/api/audit/usuario/{id}` - Logs por usuario

## 🐳 Docker

### Comandos útiles
```bash
# Iniciar
docker-compose up -d

# Ver logs
docker logs -f ganapay-api

# Detener
docker-compose down

# Reconstruir
docker-compose build
```