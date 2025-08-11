# Audimedic – Guía rápida para clonar y ejecutar

## 1) Requisitos
- **Git**
- **.NET SDK 8.x** (VS 2022 actualizado también sirve)
- **SQL Server** (LocalDB, Developer o instancia remota)
- **SQL Server Management Studio (SSMS)** (opcional, pero útil)

> Opcional (para probar autenticación Google más adelante): una cuenta en Google Cloud para obtener *Client ID*.

---

## 2) Clonar el proyecto
```bash
git clone https://github.com/jjballestas/audimedic.git
cd audimedic/Audimedic_Backend
```

---

## 3) Configuración de appsettings
Copia el archivo de ejemplo y ajusta la conexión a tu SQL Server y las claves JWT.

**Crear `appsettings.Development.json`:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AudimedicDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "ClaveSuperSecreta_ColocaUnaMuyLarga",
    "Issuer": "Audimedic",
    "Audience": "AudimedicUsers"
  },
  "Logging": {
    "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" }
  },
  "AllowedHosts": "*"
}
```

> Cambia `Server=localhost` por tu instancia (ej. `Server=.\SQLEXPRESS` o `Server=MIHOST,1433;User Id=...;Password=...`).

---

## 4) Restaurar paquetes y compilar
```bash
dotnet restore
dotnet build
```

---

## 5) Base de datos: crear/esquemas/tablas
Entra al proyecto y aplica migraciones.

### Opción A – CLI
```bash
cd Audimedic_Backend
dotnet ef database update
```

### Opción B – Visual Studio
- Abrir solución → **Consola del Administrador de Paquetes**  
- Proyecto predeterminado: `Audimedic_Backend`  
- Ejecutar: `Update-Database`

> Esto crea los **esquemas** (`security`, `users`, `contratos`, `catalogo`) y todas las tablas.

---

## 6) (Opcional) Datos semilla mínimos
**Roles + usuario de prueba (BCrypt para `123456`):**
```sql
-- Rol
IF NOT EXISTS (SELECT 1 FROM security.Roles WHERE NombreRol='medico')
INSERT INTO security.Roles (NombreRol) VALUES ('medico');

-- Usuario
DECLARE @rolId INT = (SELECT TOP 1 Id FROM security.Roles WHERE NombreRol='medico');

IF NOT EXISTS (SELECT 1 FROM security.Usuarios WHERE Email='juan@example.com')
INSERT INTO security.Usuarios (Nombre, Email, ContrasenaHash, RolId, Activo, FechaCreacion)
VALUES (
  'Juan Pérez',
  'juan@example.com',
  '$2a$11$yDrq1tq6DAzv91v6mqlf1.1I.HjztakQvUwGh1MuKQ7FE.zPYeBr2', -- 123456
  @rolId, 1, GETUTCDATE()
);
```

---

## 7) Ejecutar la API
```bash
dotnet run
```
Por defecto se levanta en `http://localhost:5xxx` / `https://localhost:7xxx`.  
Abre **Swagger** (el proyecto ya lo incluye) y prueba los endpoints.

---

## 8) Probar login (local)
**POST** `/api/auth/login`
```json
{
  "email": "juan@example.com",
  "password": "123456"
}
```
Copia el **token JWT** de la respuesta.  
Para endpoints protegidos, agrega en los headers:
```
Authorization: Bearer <tu_token>
```

---

## 9) Autenticación Google (cuando se habilite)
1. Crea un **OAuth Client ID** (tipo Web) en Google Cloud.  
2. Agrega estas claves en `appsettings.Development.json`:
```json
"GoogleAuth": {
  "ClientId": "tu-client-id.apps.googleusercontent.com",
  "Audience": "tu-client-id.apps.googleusercontent.com"
}
```
3. Usa el endpoint **POST** `/api/auth/login-google` enviando el **IdToken** del cliente web.

*(Si el equipo lo necesita, luego compartimos el `GoogleTokenValidator` y la config del front.)*

---

## 10) Convenciones del proyecto
- **Esquemas SQL**:  
  - `security` (usuarios/roles/proveedores externos)  
  - `users` (historias, médicos, entidades, procedimientos por historia)  
  - `contratos` (vínculos médico–entidad, contratos, tarifas contrato)  
  - `catalogo` (procedimientos, SOAT, UVB)
- **Enums** están en `Enums/` y se persisten como **texto** (no números).
- **Precisión decimal** configurada en el `DbContext` (evita truncamientos).
- **Regla de oro**: si vamos a usar valores “quemados”, crear **enum**.

---

## 11) Problemas comunes

**A. Error de conexión SQL**
- Revisa cadena en `appsettings.Development.json`
- Si es SQL Auth: agrega `User Id=...;Password=...`
- Si es remoto: abre el puerto 1433 y habilita TCP/IP

**B. “Unable to create a ‘DbContext’ at design time”**
- Asegura `AudimedicDbContextFactory` presente (lee `appsettings` y crea el contexto).

**C. “PendingModelChanges” o advertencias DECIMAL**
- Ejecuta:  
  ```bash
  dotnet ef migrations add FixDecimalPrecision
  dotnet ef database update
  ```

**D. Git: archivos bloqueados `.vs`**
- `.gitignore` incluye `.vs/ bin/ obj/`  
- Si ya se subieron:  
  ```bash
  git rm -r --cached .vs bin obj
  git add . && git commit -m "chore(git): limpia temporales VS"
  ```

---

## 12) Flujo de trabajo Git
```bash
git checkout -b feature/nombre-tarea
git add .
git commit -m "feat: describir cambio"
git push origin feature/nombre-tarea
# Abrir Pull Request
```
