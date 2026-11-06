<p align="center">
  <img src="./assets/banner.svg" width="100%" alt="PuraSed — Bar y Restaurante System" />
</p>

<p align="center">
  <b>Sistema Transaccional en Tiempo Real para Gestión Integral de Bares Nocturnos y Restaurantes</b><br/>
  <i>Pedidos digitales por mesa, enrutamiento desacoplado a cocina y barra, control de inventario y facturación por jornada.</i>
</p>

<p align="center">
  <img src="./assets/tags.svg" width="100%" alt="Ecosistema Tecnológico: .NET 10, Blazor WebAssembly, ASP.NET Core Web API, SQL Server y Jira Software" />
</p>

<p align="center">
  <b>Escuela Politécnica Nacional (EPN)</b> · Facultad de Ingeniería Eléctrica y Electrónica (FIEE)<br/>
  <sub>ITID803 · Diseño de Aplicaciones y Servicios para Internet (DASI) · Periodo Académico 2026</sub>
</p>

---

## <img src="./assets/icons/equipo.svg" width="22" height="22" align="absmiddle" /> Equipo Scrum y Roles

El desarrollo se gestiona bajo el marco de trabajo **Scrum**, con responsabilidades distribuidas entre el docente asesor y los desarrolladores del equipo:

<table>
  <tr>
    <td width="50%" align="center" valign="top">
      <img src="./assets/team/jose-naranjo.svg" width="100%" alt="Jose Naranjo — Product Owner" />
    </td>
    <td width="50%" align="center" valign="top">
      <a href="https://github.com/JannysG1">
        <img src="./assets/team/jannys-garrido.svg" width="100%" alt="Jannys Sherley Garrido Pazmiño — Scrum Master" />
      </a>
    </td>
  </tr>
  <tr>
    <td width="50%" align="center" valign="top">
      <a href="https://github.com/angelch1595-glitch">
        <img src="./assets/team/angel-chicaiza.svg?v=3" width="100%" alt="Angel Chicaiza — Equipo de Desarrollo" />
      </a>
    </td>
    <td width="50%" align="center" valign="top">
      <a href="https://github.com/Anghel01">
        <img src="./assets/team/angelo-conteron.svg" width="100%" alt="Angelo Conterón — Equipo de Desarrollo" />
      </a>
    </td>
  </tr>
</table>

---

## <img src="./assets/icons/contenido.svg" width="22" height="22" align="absmiddle" /> Tabla de Contenidos

| N° | Sección del Proyecto | Alcance y Contenido Técnico |
| :---: | :--- | :--- |
| `01` | [**Sobre el Proyecto y Objetivo del Sistema**](#sobre-el-proyecto-y-objetivo-del-sistema) | Problemática de bares nocturnos, propuesta de valor y alcances funcionales |
| `02` | [**Arquitectura del Sistema**](#arquitectura-del-sistema) | Arquitectura en capas desacoplada, WebSockets y ciclo de vida de comandas |
| `03` | [**Casos de Uso**](#casos-de-uso) | Diagrama UML formal interactivo, actores operativos y matriz funcional |
| `04` | [**Modelo de Base de Datos – Diagrama Entidad-Relación**](#modelo-de-base-de-datos--diagrama-entidad-relación) | Modelo relacional, 23 entidades, claves PK/FK y cardinalidades |
| `05` | [**Estructura de la Solución .NET**](#estructura-de-la-solución-net) | Árbol estructurado de carpetas, responsabilidades de WebApi y Blazor Client |
| `06` | [**Guía de Ejecución Local**](#guía-de-ejecución-local) | Requisitos previos (.NET 10, SQL Server), clonación, migraciones y arranque |
| `07` | [**Estrategia de Ramas y Flujo en Git**](#estrategia-de-ramas-y-flujo-en-git) | Modelo GitFlow, convención de ramas `develop`/`feature` y commits semánticos |
| `08` | [**Backlog, Planificación y Sprints**](#backlog-planificación-y-sprints) | Metodología Scrum, historias de usuario, estimación y tablero Jira Cloud |
| `09` | [**Licencia y Uso Académico**](#licencia-y-uso-académico) | Régimen institucional Escuela Politécnica Nacional (EPN - FIEE) |

---

## <img src="./assets/icons/proyecto.svg" width="22" height="22" align="absmiddle" /> Sobre el Proyecto y Objetivo del Sistema

**PuraSed** es una plataforma web reactiva que moderniza el ciclo operativo completo de un bar nocturno y restaurante, eliminando las comandas físicas en papel, las demoras en el enrutamiento a cocina y barra, y los errores de cuadre al cierre de jornada.

| Dimensión Operativa | Gestión Tradicional (Manual) | Solución Digital con PuraSed |
| :--- | :--- | :--- |
| **Atención en Mesa** | Comandas físicas en papel; desplazamientos constantes y confusiones de mesa. | **Acceso sin fricción**: el cliente pide y consulta el estado de su orden desde su dispositivo disponible en mesa sin registro previo. |
| **Catálogo y Menú** | Cartas físicas o imágenes estáticas en Canva que no avisan de productos agotados. | **Disponibilidad **: bloqueo automático de insumos agotados y promociones por franja horaria. |
| **Despacho a Cocina y Bar** | Comandas divididas manualmente entre coctelería y cocina con extravíos frecuentes. | **Enrutamiento automático con SignalR**: derivación simultánea de bebidas a la barra y comidas a la cocina. |
| **Cobro y Facturación** | Cuentas divididas a mano, validación tardía de transferencias y cuadres manuales. | **Pasarela integrada y arqueo**: cobro con tarjeta, verificación de transferencias y emisión secuencial de facturas. |

### Objetivos del Sistema

Diseñar e implementar una solución web desacoplada y de alta disponibilidad que permita:

- **Catálogo Dinámico**: Administrar bebidas, alimentos, combos por horario y espectáculos con variantes e insumos configurables.
- **Enrutamiento Reactivo**: Transmitir pedidos en tiempo real con actualización continua de estados (*Pendiente* $\rightarrow$ *En preparación* $\rightarrow$ *Listo* $\rightarrow$ *Entregado*).
- **Inventario por Área**: Descontar existencias automáticamente por receta con alertas de stock mínimo para cocina y bar.
- **Gestión de Reservas y Pagos**: Agendar mesas en línea, verificar comprobantes de pago digitales y consolidar la caja por jornada.
- **Analítica de Negocio**: Evaluar la satisfacción del cliente y generar reportes financieros al cierre de turno.

---

## <img src="./assets/icons/arquitectura.svg" width="22" height="22" align="absmiddle" /> Arquitectura del Sistema

PuraSed adopta una **arquitectura monolítica modular en capas**. Todo el sistema se despliega como una unidad, pero sus responsabilidades se mantienen separadas para facilitar las pruebas, el mantenimiento y una futura extracción de módulos. La base de datos y la pasarela de pago son servicios externos al monolito.

<p align="center">
  <img src="./assets/Arquitectura_Pura_Sed.png" width="100%" alt="Diagrama de Arquitectura de PuraSed" />
</p>

### Capas y responsabilidades

| Capa | Responsabilidad |
| :--- | :--- |
| **Presentación** | Blazor WebAssembly, páginas y componentes responsivos por rol. Consume la API mediante `HttpClient` y mantiene el estado de autenticación. |
| **Web API** | Controladores REST, DTOs, validación de entradas, autenticación JWT, autorización por rol y hub de SignalR. No contiene reglas de negocio. |
| **Service Layer** | Orquesta los casos de uso de pedidos, menú, inventario, reservas, pagos, facturación, encuestas, reportes, usuarios y bitácora. |
| **Domain Layer** | Define entidades, enumeraciones, reglas de negocio e interfaces de repositorio sin depender de infraestructura. |
| **Acceso a datos** | Entity Framework Core, `DbContext`, repositorios y migraciones para persistir la información en SQL Server. |
| **Infrastructure Layer** | Servicios transversales: hashing de contraseñas, logging, bitácora, generación de facturas PDF, exportación a Excel, almacenamiento de comprobantes e integración con pagos. |

### Comunicación, seguridad y tiempo real

- El cliente no accede directamente a SQL Server; todos los datos pasan por la Web API.
- Las comunicaciones utilizan HTTPS y JSON mediante DTOs.
- El personal se autentica con JWT; los roles administrativos incorporan verificación en dos pasos y autorización en el servidor.
- SignalR notifica a cliente, cocina y bar los cambios de estado de una comanda sin recargar la página.
- La pasarela de pago solo se integra para procesar el cobro y conservar la referencia de la transacción; no se almacenan datos sensibles de tarjetas.
- SQL Server concentra la base de datos del sistema y debe operar con respaldos automáticos.

La especificación completa se encuentra en [`Entregables/SP2/Arquitectura del sistema/Diseño arquitectura sistema.md`](./Entregables/SP2/Arquitectura%20del%20sistema/Dise%C3%B1o%20arquitectura%20sistema.md).

---

## <img src="./assets/icons/casos.svg" width="22" height="22" align="absmiddle" /> Casos de Uso

Representación formal interactiva de los actores del negocio (*Cliente*, *Mesero*, *Cocina/Bar*, *Asistente* y *Administrador*) y sus flujos de interacción con el sistema dentro del límite de la plataforma:

<p align="center">
  <img src="./assets/casos-de-uso.svg" width="100%" alt="Diagrama de Casos de Uso del Sistema PuraSed" />
</p>

---

## <img src="./assets/icons/base-datos.svg" width="22" height="22" align="absmiddle" /> Modelo de Base de Datos – Diagrama Entidad-Relación

El sistema Pura Sed cuenta con un modelo de base de datos relacional diseñado en SQL Server, conformado por **23 entidades** que permiten administrar los usuarios, productos, pedidos, reservas, inventario, pagos, facturación y espectáculos.

El diagrama entidad-relación representa las entidades del sistema, sus atributos, claves primarias (PK), claves foráneas (FK) y las relaciones existentes entre ellas. Para representar las cardinalidades se utiliza la notación de pata de gallo (*Crow's Foot*), que permite identificar las relaciones obligatorias y opcionales.

<p align="center">
  <img src="./assets/PuraSed_Diagrama_ER.svg" width="100%" alt="Diagrama Entidad-Relación de Pura Sed" />
</p>

El modelo permite relacionar los pedidos con las sesiones de mesa, controlar automáticamente los movimientos de inventario y gestionar las cuentas de los clientes. También contempla el cobro opcional de entradas por persona cuando existen espectáculos.

---

## <img src="./assets/icons/estructura.svg" width="22" height="22" align="absmiddle" /> Estructura de la Solución .NET

El repositorio utiliza el formato unificado de solución moderna de .NET (`.slnx`), garantizando un desacoplamiento limpio entre la capa de presentación y los servicios backend:

<p align="center">
  <img src="./assets/structure.svg" width="100%" alt="Explorador de Estructura de la Solución" />
</p>

```text
pura-sed/
├── PuraSed.slnx            # Archivo contenedor de la solución .NET (formato moderno)
├── README.md               # Documentación técnica interactiva del sistema
├── .gitignore              # Exclusiones de compilación binaria (bin, obj)
├── Entregables/                                  # Documentación y diagramas del proyecto
│   ├── SP1/                                      # Alcance y requisitos iniciales
│   └── SP2/                                      # Arquitectura, UML y diseño del prototipo
│       ├── Arquitectura del sistema/
│       │   ├── Arquitectura_Pura_Sed.png
│       │   └── Diseño arquitectura sistema.md
│       ├── Diagramas UML/
│       │   ├── Diagramas Casos de Uso/
│       │   │   ├── Pedido.png
│       │   │   ├── Reserva Mesas.png
│       │   │   ├── Pagos y Facturación.png
│       │   │   └── Reportes y estadisticas.png
│       │   ├── Diagrama de clases/
│       │   │   ├── PuraSed_Diagrama_ER.png
│       │   │   ├── PuraSed_DIS02_Diccionario_Ampliado.docx
│       │   │   └── PuraSed_DIS03_Convenciones_BD.docx
│       │   └── Listado de entidades y atributos principales.pdf
│       └── Link Figma_prototipo.md
│
├── assets/                 # Recursos vectoriales SVG, animaciones e identidad visual
│   ├── banner.svg          # Hero animado con burbujas y espuma de cerveza
│   ├── tags.svg            # Lienzo tecnológico con logos vectoriales oficiales
│   ├── architecture.svg    # Arquitectura en capas y ciclo SignalR con flujo animado
│   ├── casos-de-uso.svg    # Diagrama UML formal interactivo con actores y casos de uso
│   ├── structure.svg       # Explorador visual de la solución y componentes
│   ├── jira-board.svg      # Tarjeta interactiva de acceso al tablero Jira Software
│   ├── PuraSed_Diagrama_ER.svg  # Diagrama entidad-relación del sistema
│   ├── team/               # Tarjetas vectoriales del Scrum Team
│   └── icons/              # Iconografía vectorial cromática por sección
├── retrospective/          # Acualización de semana a semana de como se avanza en el trabajo colaborativo
│
├── PuraSed.WebApi/         # Backend: ASP.NET Core Web API 10.0 (REST & SignalR)
│   ├── Controllers/        # Controladores RESTful para mesas, pedidos, menú y caja
│   ├── Properties/         # launchSettings.json (Perfiles HTTP 5107 / HTTPS 7129)
│   ├── appsettings.json    # Cadenas de conexión a SQL Server y tokens JWT
│   └── Program.cs          # Inyección de dependencias, hubs SignalR y OpenAPI Explorer
│
└── PuraSed.Client/         # Frontend: Blazor WebAssembly 10.0 (Cliente Reactivo SPA)
    ├── Layout/             # Estructura maestra de navegación (MainLayout, NavMenu)
    ├── Pages/              # Vistas Razor reactivas por rol de usuario
    ├── Properties/         # launchSettings.json (Perfiles HTTP 5068 / HTTPS 7020)
    ├── wwwroot/            # Hojas de estilo CSS globales, iconos y runtime WebAssembly
    └── Program.cs          # Inicializador WebAssembly, inyección de HttpClient y servicios
```

---

## <img src="./assets/icons/ejecucion.svg" width="22" height="22" align="absmiddle" /> Guía de Ejecución Local

### Prerrequisitos del Entorno

* [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) instalado localmente.
* [Microsoft SQL Server 2022](https://www.microsoft.com/sql-server/) o instancia local de LocalDB.
* [Git](https://git-scm.com/) configurado en el sistema operativo.

### Pasos de Puesta en Marcha

1. **Clonar el repositorio y situarse en la rama de integración:**
   ```bash
   git clone https://github.com/angelch1595-glitch/pura-sed.git
   cd pura-sed
   git checkout develop
   ```

2. **Restaurar paquetes y compilar la solución:**
   ```bash
   dotnet build PuraSed.slnx
   ```

3. **Ejecutar el Backend (ASP.NET Core Web API):**
   ```bash
   dotnet run --project PuraSed.WebApi --launch-profile https
   ```
   * *Swagger / OpenAPI UI:* `https://localhost:7129/openapi/v1.json`
   * *Puerto HTTPS:* `7129` | *Puerto HTTP:* `5107`

4. **Ejecutar el Frontend (Blazor WebAssembly):**
   En una terminal adicional, iniciar el cliente web:
   ```bash
   dotnet run --project PuraSed.Client --launch-profile https
   ```
   * *Aplicación Web:* `https://localhost:7020`
   * *Puerto HTTPS:* `7020` | *Puerto HTTP:* `5068`

---

## <img src="./assets/icons/git.svg" width="22" height="22" align="absmiddle" /> Estrategia de Ramas y Flujo en Git

El proyecto sigue una adaptación rigurosa de **GitFlow**:

```text
main       ─────────────────────────────────────────── [Producción / Versiones estables]
               \                                /
develop     ────●──────●──────●──────●────────●─────── [Integración continua]
                  \   /        \   /
feature/*          ──●──────────●───────────────────── [Historias de usuario / Sprints]
```

### Políticas de Integración

* **Rama `main`**: Contiene únicamente código estable, probado y listo para despliegue en producción.
* **Rama `develop`**: Rama base de integración diaria donde convergen las características finalizadas.
* **Ramas de funcionalidad (`feature/<id>-<descripcion>`)**: Ramas temporales creadas a partir de `develop` para el desarrollo de cada historia de usuario.
* **Convención de Commits Semánticos**:
  * `feat:` Nueva funcionalidad o capacidad implementada.
  * `fix:` Corrección de errores o defectos detectados.
  * `docs:` Actualización de documentación técnica o README.
  * `style:` Ajustes de formato, maquetación o diseño visual.
  * `refactor:` Reorganización de código sin alterar su comportamiento externo.

---

## <img src="./assets/icons/backlog.svg" width="22" height="22" align="absmiddle" /> Backlog, Planificación y Sprints

El ciclo de desarrollo está organizado bajo el marco de trabajo **Scrum**, con planificación iterativa e historias de usuario gestionadas en el tablero oficial de Atlassian Jira Software:

<p align="center">
  <a href="https://jannysgarrido21.atlassian.net/jira/software/projects/SCRUM/boards/1">
    <img src="./assets/jira-board.svg" width="100%" alt="Tablero Oficial Scrum en Jira" />
  </a>
</p>

### Metodología de Trabajo y Gestión Ágil

* **Historias de Usuario (HU)**: Cada funcionalidad del sistema se modela bajo el formato formal:
  $$\text{Como } [\text{Rol de negocio}] \quad \text{quiero } [\text{Acción / Capacidad}] \quad \text{para } [\text{Beneficio esperado}]$$
* **Criterios de Aceptación (Definition of Done)**: Cada requerimiento cuenta con condiciones de validación verificables antes de ser promovido al siguiente estado.
* **Estimación por Story Points**: Planificación de esfuerzo basada en la serie de Fibonacci para medir la velocidad del equipo.
* **Flujo de Estados en el Tablero Jira**:
  $$\text{Backlog} \;\longrightarrow\; \text{Por Hacer (To Do)} \;\longrightarrow\; \text{En Progreso} \;\longrightarrow\; \text{Revisión/ QA} \;\longrightarrow\; \text{Listo (Done)}$$

---

## <img src="./assets/icons/licencia.svg" width="22" height="22" align="absmiddle" /> Licencia y Uso Académico

Este proyecto ha sido desarrollado con fines estrictamente académicos para la asignatura **ITID803 · Diseño de Aplicaciones y Servicios para Internet (DASI)** en la **Escuela Politécnica Nacional (EPN)**, Facultad de Ingeniería Eléctrica y Electrónica (FIEE), correspondiente al periodo lectivo **2026**.

Todos los derechos reservados al equipo de desarrollo y a la institución académica correspondiente.
