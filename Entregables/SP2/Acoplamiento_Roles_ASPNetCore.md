# Acoplamiento de Roles en ASP.NET Core · Proyecto Pura Sed

**Diseño de Aplicaciones y Servicios para Internet (DASI)**  
**Facultad de Ingeniería Eléctrica y Electrónica**  
**Documento de Arquitectura, Seguridad y Control de Acceso por Roles**

---

## 1. Introducción y Justificación

El presente documento detalla la implementación integral del control de acceso basado en roles (**Role-Based Access Control - RBAC**) en el backend de **Pura Sed** (*Cervecería / Picantería / Sereno Moreno*), acoplando las directrices de la guía académica **"2. -Roles en ASP.NET Core"** con los requerimientos funcionales y las historias de usuario de los documentos:
- `Definición_Alcance_Proyecto.md`
- `Historias_Usuarios_RF.md`

### 1.1 El Núcleo de la Guía "Roles en ASP.NET Core"
La guía de la cátedra establece dos pilares fundamentales:
1. **Dónde se asigna el rol:** En el proceso de autenticación (`AuthController.Login`), inyectando el rol como un claim dentro del token JWT:
   ```csharp
   new Claim(ClaimTypes.Role, rol)
   ```
   A través de ASP.NET Core Identity (`UserManager.GetRolesAsync(user)`), los roles se leen dinámicamente desde la base de datos SQL Server (`AspNetRoles` y `AspNetUserRoles`).
2. **Dónde se exige el rol:** En los controladores y endpoints mediante el atributo de autorización:
   ```csharp
   [Authorize] // Exige que el token sea válido (autenticado)
   [Authorize(Roles = "Admin")] // Exige rol específico
   [Authorize(Roles = "Admin,Mesero,Cliente")] // Permite acceso a cualquiera de los roles listados
   ```
3. **Señal de éxito y fallo en las pruebas:**
   - **`200 OK` / `201 Created`**: El usuario está autenticado y su rol tiene permiso para ejecutar la acción.
   - **`401 Unauthorized`**: El usuario no envió token, el token es inválido o expiró.
   - **`403 Forbidden`**: El token es válido y la identidad está confirmada, pero su rol **no cuenta con los privilegios requeridos**. En la guía se resalta expresamente: *«El 403 es la señal de que todo está bien: el token es válido, pero ese rol no tiene permiso para esa acción»*.

---

## 2. Matriz Integral de Roles del Negocio (Pura Sed)

A diferencia de un esquema simplista (solo Admin y Usuario), el modelo de negocio de **Pura Sed** contempla personal operativo, administrativo, técnico y comensales. Se definieron las siguientes identidades:

| Rol | Actor / Cargo | Responsabilidades y Permisos Clave | Historias de Usuario / RF |
|---|---|---|---|
| **`SuperAdmin`** | Equipo Técnico / TI | Mantenimiento del sistema, bitácora de auditoría inmutable, alta y asignación de roles al personal de forma remota. | HU-03, HU-04, RF-02 |
| **`Admin`** | Gerente General | Gestión de catálogo base (crear, editar, eliminar productos), promociones porcentuales, espectáculos, habilitar cobro de cuentas de mesas, arqueos y reportes globales. | HU-08, HU-09, HU-19, HU-20, HU-27 a HU-32 |
| **`Mesero`** | Operativo de Salón | **Toma de pedidos en mesas desde dispositivo móvil (HU-13)**, asignación y cambio de estado de mesas, seguimiento de pedidos, entrega a comensales y confirmación de cobro en efectivo. | HU-10, HU-13, HU-16, HU-23, HU-28 |
| **`AdminCocina`** | Jefe de Cocina / KDS | Visualización de pedidos de cocina (paila, comidas), cambio de estado (`Pendiente` → `EnPreparacion` → `Listo`), inventario de cocina y marcado de platos agotados. | HU-10, HU-11, HU-12, HU-15 |
| **`AdminBar`** | Bartender / KDD | Visualización de pedidos de barra (cervezas, fermentos, chichas, cócteles), cambio de estado de bebidas, inventario de licores y marcado de fermentos agotados. | HU-10, HU-11, HU-12, HU-15 |
| **`Asistente`** | Host / Recepción | Registro, modificación y cancelación de reservas (telefónicas y presenciales), visualización de agenda y asignación de mesas activas a clientes. | RF-01, HU-01, HU-18 |
| **`Cliente`** | Comensal en Mesa | Consulta del menú interactivo (solo disponibles), registro de pedidos de su mesa desde tablet/móvil, seguimiento en tiempo real, pago (tarjeta/transferencia) y encuesta de satisfacción. | HU-01, HU-05 a HU-07, HU-13, HU-16, HU-21, HU-25 |
| **`Usuario`** | Usuario Registrado | Rol estándar de comensal registrado en la plataforma. | HU-02 |

---

## 3. Principios SOLID y DRY en la Implementación

Para cumplir estrictamente con los lineamientos de arquitectura de software y la rúbrica de la práctica:

1. **Principio DRY (Don't Repeat Yourself):**
   - Se erradicaron las cadenas mágicas (*magic strings*) como `"Admin"`, `"Mesero"` dispersas en controladores.
   - Se creó la clase centralizada `PuraSed.WebApi.Constants.Roles`, que almacena los roles individuales y las combinaciones de políticas en constantes reutilizables.
2. **Principio de Responsabilidad Única (SRP):**
   - La autenticación y emisión de tokens reside en `AuthController`.
   - La gestión de cuentas de personal reside en `UsuariosController`.
   - La operatividad de comensales y salón reside en `PedidosController` y `MesasController`.
   - La gestión del menú reside en `ProductosController`.
   - La liquidación de cuentas reside en `PagosController`.
   - La analítica gerencial reside en `ReportesController`.
3. **Principio Abierto/Cerrado (OCP):**
   - Agregar un nuevo rol (por ejemplo, `Cajero` o `Sommelier`) únicamente requiere añadir la constante en `Roles.cs` y sembrarlo en `IdentitySeeder.cs`, sin necesidad de reescribir la lógica de validación del token.

---

## 4. Arquitectura de Código Implementada

### 4.1 Catálogo Centralizado (`PuraSed.WebApi/Constants/Roles.cs`)
```csharp
namespace PuraSed.WebApi.Constants;

public static class Roles
{
    // Roles individuales
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string Mesero = "Mesero";
    public const string AdminCocina = "AdminCocina";
    public const string AdminBar = "AdminBar";
    public const string Asistente = "Asistente";
    public const string Cliente = "Cliente";
    public const string Usuario = "Usuario";

    // Agrupaciones para [Authorize(Roles = ...)]
    public const string TodosPersonal = $"{SuperAdmin},{Admin},{Mesero},{AdminCocina},{AdminBar},{Asistente}";
    public const string Administracion = $"{SuperAdmin},{Admin}";
    public const string GestionMenu = $"{SuperAdmin},{Admin}";
    public const string TomaPedidos = $"{SuperAdmin},{Admin},{Mesero},{Cliente}"; // HU-13
    public const string OperativoMesas = $"{SuperAdmin},{Admin},{Mesero},{Asistente}";
    public const string CocinaYBar = $"{SuperAdmin},{Admin},{AdminCocina},{AdminBar}";
    public const string GestionInventario = $"{SuperAdmin},{Admin},{AdminCocina},{AdminBar}"; // HU-10
    public const string GestionCobro = $"{SuperAdmin},{Admin},{Mesero}"; // HU-23
}
```

### 4.2 Inyección de Claims en `AuthController.cs`
En cumplimiento del Paso 1 de la Guía 2:
```csharp
var roles = await _userManager.GetRolesAsync(user);

var claims = new List<Claim>
{
    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
    new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
};

// Inyección de un ClaimTypes.Role por cada rol asignado en la BD
foreach (var rol in roles)
{
    claims.Add(new Claim(ClaimTypes.Role, rol));
}
```

### 4.3 Toma de Pedidos por el Mesero y Clientes (`PedidosController.cs`)
Atendiendo al requerimiento explícito del proyecto y a la **HU-13**:
- El mesero puede ingresar pedidos directamente para cualquier mesa activa.
- El sistema asocia automáticamente el `MeseroId` y `MeseroNombre` para generar el índice de rendimiento de meseros (**HU-28**).
- **HU-14 (Distribución automática):** Cada producto se clasifica automáticamente según su categoría culinaria:
  - Cervezas, licores, fermentos y chichas → `Area = "Bar"`.
  - Platos típicos, paila, comida caliente → `Area = "Cocina"`.
- **HU-15 (Control de preparación):** Cocina solo puede cambiar el estado de platos de cocina; Bar solo de bebidas de barra. Si un usuario de Cocina intenta modificar un ítem de Bar, la API responde `403 Forbidden`.
- **Entrega a Mesa:** El Mesero marca el pedido como `Entregado`.

### 4.4 Disponibilidad de Productos y Agotados (`ProductosController.cs`)
En cumplimiento de la **HU-10**:
- Comensales y público no autenticado únicamente visualizan productos con `Activo = true`.
- Personal de Cocina, Bar y Administración pueden consultar el catálogo completo (incluyendo los agotados sombreados) y disponen del endpoint `PATCH /api/productos/{id}/disponibilidad?activo=false` con `[Authorize(Roles = Roles.GestionInventario)]`.

### 4.5 Gestión de Cobros y Efectivo (`PagosController.cs`)
- **HU-20:** Únicamente `Admin` o `SuperAdmin` pueden habilitar el cobro de la mesa (`POST /api/pagos/habilitar/{numeroMesa}`).
- **HU-23:** Tanto el `Mesero` como el `Admin` pueden recibir el efectivo en la mesa o caja y confirmar el pago (`POST /api/pagos/confirmar-efectivo`), liberando la mesa automáticamente y calculando el cambio.
- **HU-21:** Los clientes pueden pagar con tarjeta (`POST /api/pagos/tarjeta/{numeroMesa}`) una vez habilitado el cobro.

### 4.6 Gestión de Usuarios del Personal (`UsuariosController.cs`)
En cumplimiento de la **HU-03**:
- Endpoint protegido con `[Authorize(Roles = Roles.SuperAdmin)]`.
- Permite consultar todo el personal y crear usuarios asignando directamente roles autorizados (`Admin`, `Mesero`, `AdminCocina`, `AdminBar`, `Asistente`).

---

## 5. Usuarios Sembrados para Pruebas (Credenciales)

Al arrancar la API, `IdentitySeeder` y `MesaSeeder` garantizan que la base de datos `PuraSedDb` cuente con los siguientes usuarios listos para ser testeados en Swagger o Postman:

| Usuario | Contraseña | Roles Asignados | Propósito de Prueba en Swagger |
|---|---|---|---|
| `admin` | `Admin123` | `Admin`, `SuperAdmin` | Gestión completa: menú, reportes, habilitar cobro, bitácora y usuarios. |
| `mesero` | `Mesero123` | `Mesero` | **Hacer pedidos en mesas (HU-13)**, ver mapa de mesas, entregar pedidos y confirmar efectivo. |
| `cocina` | `Cocina123` | `AdminCocina` | Ver pedidos de cocina (KDS), marcar platos como `EnPreparacion` o `Listo`. |
| `bar` | `Bar12345` | `AdminBar` | Ver pedidos de bar (KDD), marcar cervezas/chichas como `Listo`. |
| `asistente` | `Asistente123` | `Asistente` | Gestión de reservas y estado de mesas. |
| `cliente` | `Cliente123` | `Cliente`, `Usuario` | Consulta de catálogo y pedidos desde su mesa. |

---

## 6. Guía de Pruebas en Swagger / HTTP

Para validar el acoplamiento de roles según la sección 3 de la guía de la cátedra:

### Paso 1: Autenticación
1. En Swagger, abrir `POST /api/Auth/login`.
2. Enviar el cuerpo con las credenciales del usuario deseado, por ejemplo el Mesero:
   ```json
   {
     "usuario": "mesero",
     "password": "Mesero123"
   }
   ```
3. Copiar el valor del `token` devuelto.

### Paso 2: Inyección del Token en Swagger
1. Pulsar el botón verde **`Authorize`** en la parte superior derecha de Swagger.
2. Pegar el token en el campo `Value` (sin la palabra Bearer) y pulsar **Authorize**.

### Paso 3: Casos de Prueba Esperados (200 OK vs 403 Forbidden)

#### Escenario 1: Mesero registra pedido en Mesa 1 (HU-13)
- **Token:** `mesero`
- **Petición:** `POST /api/pedidos`
- **Cuerpo:**
  ```json
  {
    "numeroMesa": 1,
    "observaciones": "Comanda tomada por Mesero en mesa 1",
    "items": [
      { "productoId": 1, "cantidad": 2, "observaciones": "Bien servido" }
    ]
  }
  ```
- **Resultado esperado:** `201 Created`. Se devuelve el pedido con ID generado, total calculado, `meseroNombre` asociado y el ítem distribuido a su área respectiva.

#### Escenario 2: Mesero intenta crear un nuevo producto en el Menú Base
- **Token:** `mesero`
- **Petición:** `POST /api/productos`
- **Resultado esperado:** **`403 Forbidden`**. Demuestra que el mesero no tiene permiso para alterar el catálogo gerencial (reservado para `Admin`).

#### Escenario 3: Cocina intenta modificar un ítem de Bar
- **Token:** `cocina`
- **Petición:** `PATCH /api/pedidos/1/items/1/estado` con `{"estadoItem": "Listo"}` (donde el ítem pertenece a Bar).
- **Resultado esperado:** **`403 Forbidden`**. El sistema aísla la preparación culinaria de las bebidas.

#### Escenario 4: Bar actualiza su propio ítem a Listo
- **Token:** `bar`
- **Petición:** `PATCH /api/pedidos/1/items/1/estado` con `{"estadoItem": "Listo"}`.
- **Resultado esperado:** **`200 OK`**. Se actualiza el ítem y se recalcula el estado general del pedido.

#### Escenario 5: Admin habilita cobro y Mesero confirma pago en efectivo (HU-20 / HU-23)
1. **Admin habilita cobro:** `POST /api/pagos/habilitar/1` con token de `admin` → **`200 OK`**.
2. **Mesero confirma pago en mesa:** `POST /api/pagos/confirmar-efectivo` con token de `mesero`:
   ```json
   {
     "numeroMesa": 1,
     "montoRecibido": 10.0,
     "observaciones": "Cobro en efectivo mesa 1"
   }
   ```
- **Resultado esperado:** **`200 OK`**. La cuenta pasa a pagada, se calcula el cambio devuelto y la mesa se libera automáticamente (`mesaLiberada = true`).

#### Escenario 6: Admin consulta reporte de ventas y rendimiento de meseros (HU-27 / HU-28)
- **Token:** `admin`
- **Petición:** `GET /api/reportes/rendimiento-meseros`
- **Resultado esperado:** **`200 OK`**. Muestra a cada mesero con el número de pedidos atendidos, montos totales y efectividad.

---

## 7. Verificación en Base de Datos (SQL Server LocalDB)

Para auditar la persistencia de usuarios y roles mediante consola o SQL Server Management Studio:
```sql
-- Consultar roles registrados
SELECT Id, Name FROM AspNetRoles;

-- Consultar usuarios y su asignación de roles
SELECT u.UserName, u.Email, u.NombreCompleto, r.Name AS Rol
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON ur.UserId = u.Id
JOIN AspNetRoles r ON r.Id = ur.RoleId
ORDER BY r.Name, u.UserName;

-- Consultar pedidos registrados con el mesero a cargo
SELECT p.Id, p.NumeroMesa, p.FechaHora, p.Estado, p.Total, p.MeseroNombre, p.Pagado
FROM Pedidos p;
```

---

## 8. Conclusión

Con esta implementación, la sección **"2. -Roles en ASP.NET Core"** queda acoplada al 100% en la solución **Pura Sed**:
1. Los roles provienen de la base de datos vía ASP.NET Core Identity.
2. Se inyectan de forma estandarizada en los tokens JWT (`ClaimTypes.Role`).
3. Se protegen endpoints de forma granular y robusta con `[Authorize(Roles = ...)]`.
4. El rol **Mesero** dispone de todas las facultades requeridas para la operativa en mesas (**HU-13**, **HU-16**, **HU-23**).
5. Se respetan los principios de diseño de software **SOLID** y **DRY**, garantizando un código limpio, modular y completamente escalable.
