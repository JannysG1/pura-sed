namespace PuraSed.WebApi.Constants;

/// <summary>
/// Catálogo centralizado de Roles y Políticas de Autorización para Pura Sed.
/// Diseñado bajo principios SOLID (SRP, OCP) y DRY para evitar cadenas mágicas
/// y garantizar consistencia entre la base de datos, el token JWT y los atributos [Authorize].
/// Basado en Definición de Alcance (Sección d.ii) e Historias de Usuario (HU-02, HU-03, HU-13, HU-15, HU-18, HU-20, HU-23).
/// </summary>
public static class Roles
{
    // ==========================================
    // 1. ROLES INDIVIDUALES DEL SISTEMA
    // ==========================================
    /// <summary>Equipo técnico con control total del sistema, bitácora y usuarios (HU-03, HU-04)</summary>
    public const string SuperAdmin = "SuperAdmin";

    /// <summary>Gerente general del bar: supervisa menú, promociones, cobros, facturación y reportes</summary>
    public const string Admin = "Admin";

    /// <summary>Personal operativo de salón: atención a mesas, toma de pedidos por mesa y confirmación de cobro en efectivo</summary>
    public const string Mesero = "Mesero";

    /// <summary>Responsable del área culinaria: pedidos de cocina (KDS) e inventario de cocina (HU-11, HU-15)</summary>
    public const string AdminCocina = "AdminCocina";

    /// <summary>Responsable de barra y fermentos: pedidos de bar (KDD) e inventario de bebidas (HU-11, HU-15)</summary>
    public const string AdminBar = "AdminBar";

    /// <summary>Encargado de la gestión de reservas telefónicas y en línea, y estado de mesas (HU-18)</summary>
    public const string Asistente = "Asistente";

    /// <summary>Comensal o cliente en mesa con acceso a menú, pedidos de su mesa y pago (HU-01, HU-13)</summary>
    public const string Cliente = "Cliente";

    /// <summary>Rol base estándar de usuario registrado</summary>
    public const string Usuario = "Usuario";

    /// <summary>Lista consolidada de todos los roles para validación y sembrado en Identity</summary>
    public static readonly string[] Todos =
    {
        SuperAdmin,
        Admin,
        Mesero,
        AdminCocina,
        AdminBar,
        Asistente,
        Cliente,
        Usuario
    };

    // ==========================================
    // 2. AGRUPACIONES DE ROLES PARA [Authorize(Roles = ...)]
    // ==========================================
    /// <summary>Todo el personal del local (excluye comensales)</summary>
    public const string TodosPersonal = $"{SuperAdmin},{Admin},{Mesero},{AdminCocina},{AdminBar},{Asistente}";

    /// <summary>Nivel gerencial y de administración técnica</summary>
    public const string Administracion = $"{SuperAdmin},{Admin}";

    /// <summary>Permiso para crear, editar o eliminar productos y promociones del menú base</summary>
    public const string GestionMenu = $"{SuperAdmin},{Admin}";

    /// <summary>
    /// Permiso para registrar pedidos en las mesas (HU-13).
    /// Habilita explícitamente al Mesero (en dispositivo móvil), al Cliente (en tablet de mesa) y a la Administración.
    /// </summary>
    public const string TomaPedidos = $"{SuperAdmin},{Admin},{Mesero},{Cliente}";

    /// <summary>Personal encargado de la operatividad y estado físico de mesas</summary>
    public const string OperativoMesas = $"{SuperAdmin},{Admin},{Mesero},{Asistente}";

    /// <summary>Áreas de preparación de comandas (Cocina y Bar)</summary>
    public const string CocinaYBar = $"{SuperAdmin},{Admin},{AdminCocina},{AdminBar}";

    /// <summary>Área de preparación de cocina</summary>
    public const string GestionCocina = $"{SuperAdmin},{Admin},{AdminCocina}";

    /// <summary>Área de preparación de bar / fermentos</summary>
    public const string GestionBar = $"{SuperAdmin},{Admin},{AdminBar}";

    /// <summary>Gestión de inventario y marcado de productos agotados</summary>
    public const string GestionInventario = $"{SuperAdmin},{Admin},{AdminCocina},{AdminBar}";

    /// <summary>Gestión y agenda de reservas</summary>
    public const string GestionReservas = $"{SuperAdmin},{Admin},{Asistente}";

    /// <summary>Habilitación y confirmación de cobros (efectivo / caja / mesa)</summary>
    public const string GestionCobro = $"{SuperAdmin},{Admin},{Mesero}";
}
