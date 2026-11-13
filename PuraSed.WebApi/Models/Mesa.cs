namespace PuraSed.WebApi.Models;

public class Mesa
{
    public int Id { get; set; }

    /// <summary>Número identificador de mesa física (ej. 1, 2, 3...)</summary>
    public int Numero { get; set; }

    /// <summary>Capacidad máxima de personas en la mesa</summary>
    public int Capacidad { get; set; } = 4;

    /// <summary>Estado de la mesa: Disponible, Ocupada, Reservada, EnMantenimiento</summary>
    public string Estado { get; set; } = "Disponible";

    /// <summary>ID opcional del mesero asignado para atender esta mesa</summary>
    public string? MeseroAsignadoId { get; set; }

    /// <summary>Nombre o referencia del mesero asignado</summary>
    public string? MeseroAsignadoNombre { get; set; }
}
