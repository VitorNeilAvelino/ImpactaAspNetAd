﻿using Northwind.Dominio;
using System.Data.Entity.ModelConfiguration;

namespace NorthWind.Repositorios.SqlServer.EF.ModelConfiguration
{
    public class ClienteConfiguration : EntityTypeConfiguration<Cliente>
    {
        public ClienteConfiguration()
        {
            HasRequired(c => c.Endereco)
                .WithRequiredPrincipal(e => e.Cliente)
                .WillCascadeOnDelete(true);
        }
    }
}