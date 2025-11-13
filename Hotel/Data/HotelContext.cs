using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Hotel.Models;

namespace Hotel.Data
{
    public class HotelContext : DbContext
    {
        public HotelContext (DbContextOptions<HotelContext> options)
            : base(options)
        {
        }

        public DbSet<Hotel.Models.Habitacion> Habitacion { get; set; } = default!;
        public DbSet<Hotel.Models.Cliente> Cliente { get; set; } = default!;
        public DbSet<Hotel.Models.Alquiler> Alquiler { get; set; } = default!;
        public DbSet<Hotel.Models.Acceso> Acceso { get; set; } = default!;

        public DbSet<Hotel.Models.Estado> Estado { get; set; } = default!;
        public DbSet<Hotel.Models.TipoDoc> TipoDoc { get; set; } = default!;
        public DbSet<Hotel.Models.TipoHab> TipoHab { get; set; } = default!;
    }
}
