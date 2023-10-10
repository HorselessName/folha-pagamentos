using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Folha_de_Pagamentos.Models;

namespace Folha_de_Pagamentos.Data
{
    public class Folha_de_PagamentosContext : DbContext
    {
        public Folha_de_PagamentosContext (DbContextOptions<Folha_de_PagamentosContext> options)
            : base(options)
        {
        }

        public DbSet<Folha_de_Pagamentos.Models.Funcionario> Funcionario { get; set; } = default!;
        public DbSet<Folha_de_Pagamentos.Models.FolhaDePagamento> FolhaDePagamento { get; set; } = default!;

    }
}
