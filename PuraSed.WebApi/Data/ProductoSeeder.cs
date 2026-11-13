using Microsoft.EntityFrameworkCore;
using PuraSed.WebApi.Models;

namespace PuraSed.WebApi.Data;

public static class ProductoSeeder
{
    public static async Task SeedAsync(PuraSedDbContext context)
    {
        // Si existen productos de cafetería residuales de las pruebas de la guía, limpiarlos
        var productosCafeteria = await context.Productos
            .Where(p => p.Nombre.Contains("Café") || 
                        p.Nombre.Contains("Cafe") || 
                        p.Nombre.Contains("Espresso") || 
                        p.Nombre.Contains("Capuccino") || 
                        p.Nombre.Contains("Pastel de chocolate") || 
                        p.Nombre.Contains("Chocolate caliente"))
            .ToListAsync();

        if (productosCafeteria.Count > 0)
        {
            context.Productos.RemoveRange(productosCafeteria);
            await context.SaveChangesAsync();
        }

        if (await context.Productos.AnyAsync())
        {
            return;
        }

        var catalogoSerenoMoreno = new List<Producto>
        {
            // --- FERMENTOS VIVOS (MUCHA CHICHA) ---
            new Producto
            {
                Nombre = "Mucha Chicha Maíz (Roja / Morado)",
                Descripcion = "Bebida de maíz morado fermentada. 6.0% Vol. Alc. 330 ml.",
                Precio = 4.00m,
                Categoria = "Fermentos Vivos",
                ImagenUrl = "/catalogo_imagenes/fermentos/botella_roja_float.png",
                Activo = true
            },
            new Producto
            {
                Nombre = "Mucha Chicha Maíz (Amarilla / Clásica)",
                Descripcion = "Bebida de maíz clásico fermentada. 4.5% v/v. 330 ml.",
                Precio = 4.00m,
                Categoria = "Fermentos Vivos",
                ImagenUrl = "/catalogo_imagenes/fermentos/botella_amarilla_float.png",
                Activo = true
            },
            new Producto
            {
                Nombre = "Mucha Chicha Morocho (Lata 500ml)",
                Descripcion = "Fermento de maíz morocho con notas cítricas. 4.5% v/v. 500 ml.",
                Precio = 5.50m,
                Categoria = "Fermentos Vivos",
                ImagenUrl = "/catalogo_imagenes/fermentos/lata_morocho_float.png",
                Activo = true
            },
            new Producto
            {
                Nombre = "Mucha Chicha Maíz Morado (Lata 500ml)",
                Descripcion = "Fermento artesanal de maíz morado. 6.0% v/v. 500 ml.",
                Precio = 5.50m,
                Categoria = "Fermentos Vivos",
                ImagenUrl = "/catalogo_imagenes/fermentos/lata_morado_float.png",
                Activo = true
            },
            new Producto
            {
                Nombre = "Mucha Chicha Yuca (Lata 500ml)",
                Descripcion = "Fermento amazónico de yuca viva. 7.0% v/v. 500 ml.",
                Precio = 5.50m,
                Categoria = "Fermentos Vivos",
                ImagenUrl = "/catalogo_imagenes/fermentos/lata_yuca_float.png",
                Activo = true
            },

            // --- CERVECERÍA ARTESANAL & BAR ---
            new Producto
            {
                Nombre = "Cerveza Artesanal Sereno Blonde Ale",
                Descripcion = "Cerveza rubia artesanal ligera, notas a cereal y miel andina. 4.8% ABV. 330 ml.",
                Precio = 4.50m,
                Categoria = "Cervecería Artesanal",
                ImagenUrl = "/catalogo_imagenes/si_hay_si_hay/vaso_artesanal_float.png",
                Activo = true
            },
            new Producto
            {
                Nombre = "Cerveza Artesanal Sereno IPA Andina",
                Descripcion = "India Pale Ale con lúpulos frescos, aroma a maracuyá y amargor herbal. 6.2% ABV. 330 ml.",
                Precio = 5.00m,
                Categoria = "Cervecería Artesanal",
                ImagenUrl = "/catalogo_imagenes/si_hay_si_hay/vaso_artesanal_float.png",
                Activo = true
            },
            new Producto
            {
                Nombre = "Cerveza Artesanal Sereno Stout de Panela",
                Descripcion = "Cerveza oscura con malta tostada, notas a cacao fino de aroma y panela. 5.8% ABV. 330 ml.",
                Precio = 5.00m,
                Categoria = "Cervecería Artesanal",
                ImagenUrl = "/catalogo_imagenes/si_hay_si_hay/vaso_artesanal_float.png",
                Activo = true
            },

            // --- COCINA DE PAILA & PICANTERÍA (PIDA NO MÁS) ---
            new Producto
            {
                Nombre = "Locrito de Papa",
                Descripcion = "Locro cremoso de papa chola con queso de hoja y aguacate.",
                Precio = 5.50m,
                Categoria = "Cocina de Paila",
                ImagenUrl = "/catalogo_imagenes/pida_no_mas/locrito_de_papa_5.50_float.png",
                Activo = true
            },
            new Producto
            {
                Nombre = "Hongos Portobello",
                Descripcion = "Portobellos andinos salteados al ajillo con chicha y panela.",
                Precio = 8.50m,
                Categoria = "Cocina de Paila",
                ImagenUrl = "/catalogo_imagenes/pida_no_mas/hongos_portobello_float.png",
                Activo = true
            },
            new Producto
            {
                Nombre = "Ceviche de Coco",
                Descripcion = "Leche de coco biche fresca con palmito, choclo y chifles.",
                Precio = 5.50m,
                Categoria = "Cocina de Paila",
                ImagenUrl = "/catalogo_imagenes/pida_no_mas/ceviche_de_coco_float.png",
                Activo = true
            },
            new Producto
            {
                Nombre = "Choclo Papa con Queso",
                Descripcion = "Choclo tierno desgranado con papas chauchas y salsa criolla.",
                Precio = 4.75m,
                Categoria = "Cocina de Paila",
                ImagenUrl = "/catalogo_imagenes/pida_no_mas/choclopapaconqueso_float.png",
                Activo = true
            },
            new Producto
            {
                Nombre = "Maduro con Queso",
                Descripcion = "Maduro asado en fogón con queso tierno de hacienda.",
                Precio = 3.50m,
                Categoria = "Cocina de Paila",
                ImagenUrl = "/catalogo_imagenes/pida_no_mas/maduroconqueso_float.png",
                Activo = true
            },
            new Producto
            {
                Nombre = "Fritada Tradicional de Fogón",
                Descripcion = "Fritada glaseada en paila de bronce con mote sucio, llapingachos y encurtido criollo.",
                Precio = 8.50m,
                Categoria = "Cocina de Paila",
                ImagenUrl = "/catalogo_imagenes/lleve_lleve/costilla_precocida_float.png",
                Activo = true
            },

            // --- MERCADO AL VACÍO (LLEVE LLEVE) ---
            new Producto
            {
                Nombre = "Costilla Precocida",
                Descripcion = "Costilla de cerdo sellada al vacío, lista para dorar en paila.",
                Precio = 12.00m,
                Categoria = "Mercado al Vacío",
                ImagenUrl = "/catalogo_imagenes/lleve_lleve/costilla_precocida_float.png",
                Activo = true
            },
            new Producto
            {
                Nombre = "Codillo Glaseado",
                Descripcion = "Codillo tierno ahumado con panela y especias de la serranía.",
                Precio = 11.50m,
                Categoria = "Mercado al Vacío",
                ImagenUrl = "/catalogo_imagenes/lleve_lleve/codillo_precocido_float.png",
                Activo = true
            },
            new Producto
            {
                Nombre = "Muchines de Yuca (Pack)",
                Descripcion = "Empaque de 6 unidades congeladas para freír en casa.",
                Precio = 6.50m,
                Categoria = "Mercado al Vacío",
                ImagenUrl = "/catalogo_imagenes/lleve_lleve/muchines_yuca_float.png",
                Activo = true
            },
            new Producto
            {
                Nombre = "Queso de Hoja Artesanal",
                Descripcion = "Queso de pasta hilada envuelto en hoja de achira.",
                Precio = 3.75m,
                Categoria = "Mercado al Vacío",
                ImagenUrl = "/catalogo_imagenes/lleve_lleve/queso_hoja_float.png",
                Activo = true
            },

            // --- ARTE & INDUMENTARIA (SI HAY SI HAY) ---
            new Producto
            {
                Nombre = "Camiseta Verde 'Si Hay'",
                Descripcion = "Algodón orgánico peinado con serigrafía tradicional a mano.",
                Precio = 20.00m,
                Categoria = "Arte & Indumentaria",
                ImagenUrl = "/catalogo_imagenes/si_hay_si_hay/camiseta_verde_float.png",
                Activo = true
            },
            new Producto
            {
                Nombre = "Gorra Trucker Sereno",
                Descripcion = "Gorra estructurada con parche bordado de la luna creciente.",
                Precio = 15.00m,
                Categoria = "Arte & Indumentaria",
                ImagenUrl = "/catalogo_imagenes/si_hay_si_hay/gorra_sereno_float.png",
                Activo = true
            },
            new Producto
            {
                Nombre = "Tote Bag Linografía",
                Descripcion = "Bolso de lona gruesa con grabado botánico de maíz y chicha.",
                Precio = 12.00m,
                Categoria = "Arte & Indumentaria",
                ImagenUrl = "/catalogo_imagenes/si_hay_si_hay/totebag_linografia_float.png",
                Activo = true
            },
            new Producto
            {
                Nombre = "Vaso Cervecero Artesanal",
                Descripcion = "Cristal templado serigrafiado 'Ni Tan Joven Ni Tan Viejo'.",
                Precio = 8.00m,
                Categoria = "Arte & Indumentaria",
                ImagenUrl = "/catalogo_imagenes/si_hay_si_hay/vaso_artesanal_float.png",
                Activo = true
            }
        };

        await context.Productos.AddRangeAsync(catalogoSerenoMoreno);
        await context.SaveChangesAsync();
    }
}
