﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Loja.Dominio;
using System.Data.Entity;
using System.Linq;
using NorthWind.Repositorios.SqlServer.EF.Migrations;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System;

namespace Loja.Repositorios.SqlServer.EF.Tests
{
    [TestClass()]
    public class LojaDbContextTests
    {
        private static LojaDbContext _contexto = new LojaDbContext();

        [ClassInitialize]
        public static void Inicializar(TestContext testContext)
        {
            // 3o. Roda uma vez apenas para essa classe, independentemente da quantidade de testes que ela tenha.
            //_contexto.Database.Initialize(force: true);

            //_contexto.Database.Log = q => Debug.WriteLine(q);
            _contexto.Database.Log = LogarQuery;
        }

        private static void LogarQuery(string query)
        {
            Debug.WriteLine(query);
        }

        [TestMethod()]
        public void LojaDbContextTest()
        {
            // 1o.
            //using (var contexto = new LojaDbContext())
            //{
            //    var papelaria = new Categoria { Nome = "Papelaria" };

            //    contexto.Categorias.Add(papelaria);
            //    contexto.SaveChanges();
            //}

            // 2o.
            //new LojaDbContext().Database.Initialize(false);
        }

        [TestMethod()]
        public void InserirProdutoTest()
        {
            var caneta = new Produto();
            caneta.Nome = "Caneta";
            caneta.Preco = 15.42m;
            caneta.Estoque = 42;
            //caneta.Categoria = _contexto.Categorias.Single(ComparararNomeCategoria);
            //caneta.Categoria = _contexto.Categorias.Single(delegate (Categoria c) { return c.Nome == "Papelaria"; });
            //caneta.Categoria = _contexto.Categorias.Single((Categoria c) => c.Nome == "Papelaria");
            caneta.Categoria = _contexto.Categorias.Single(c => c.Nome == "Papelaria");

            _contexto.Produtos.Add(caneta);

            _contexto.SaveChanges();
        }

        [TestMethod]
        public void InserirProdutoComNovaCategoriaTeste()
        {
            var barbeador = new Produto();
            barbeador.Nome = "Barbeador";
            barbeador.Preco = 104.60m;
            barbeador.Estoque = 46;
            barbeador.Categoria = new Categoria { Nome = "Perfumaria" };

            _contexto.Produtos.Add(barbeador);

            _contexto.SaveChanges();
        }

        [TestMethod]
        public void EditarProduto()
        {
            var caneta = _contexto.Produtos.Single(p => p.Nome == "Caneta");

            caneta.Preco = 10;

            _contexto.SaveChanges();

            caneta = _contexto.Produtos.Single(p => p.Nome == "Caneta");

            Assert.IsTrue(caneta.Preco == 10);
        }

        [TestMethod]
        public void ExcluirProduto()
        {
            var caneta = _contexto.Produtos.Single(p => p.Nome == "Caneta");

            _contexto.Produtos.Remove(caneta);

            _contexto.SaveChanges();

            Assert.IsFalse(_contexto.Produtos.Any(p => p.Nome == "Caneta"));
        }

        [TestMethod]
        public void ObterPrecoMedioPapelaria()
        {
            var media = _contexto.Produtos
                .Where(p => p.Categoria.Id == 1 && !p.Descontinuado)
                .Average(p => p.Preco);

            Assert.AreNotEqual(media, 0m);
        }

        [TestMethod]
        public void LazyLoadDesligadoTeste()
        {
            //_contexto.Configuration.LazyLoadingEnabled = true; // Não funciona sem o virtual.

            var grampeador = _contexto.Produtos.Single(p => p.Nome == "Grampeador");
            Assert.IsNull(grampeador.Categoria);
        }

        [TestMethod]
        public void LazyLoadDesligadoLoadTeste()
        {
            var caneta = _contexto.Produtos.First(p => p.Nome == "Caneta");

            _contexto.Entry(caneta).Reference(p => p.Categoria).Load();

            Assert.IsNotNull(caneta.Categoria);
        }

        [TestMethod]
        public void LazyLoadLigadoTeste()
        {
            // 1o - colocar o virtual nas properties.
            // 2o - demonstrar que são feitas duas queries.

            var grampeador = _contexto.Produtos.Single(p => p.Nome == "Grampeador");
            Assert.IsTrue(grampeador.Categoria.Nome == "Papelaria");
        }

        [TestMethod]
        public void IncludeTeste()
        {
            // 1o - remover o virtual das properties.
            // 2o - demonstrar que é feita uma query, com join.

            //using System.Data.Entity para usar o Include com lambda.
            var grampeador = _contexto.Produtos.Include(p => p.Categoria).Single(p => p.Nome == "Grampeador");
            Assert.IsTrue(grampeador.Categoria.Nome == "Papelaria");
        }

        [TestMethod]
        public void ProxyDesativadoTeste()
        {
            //https://msdn.microsoft.com/en-us/library/dd456854(v=vs.100).aspx
            //funciona apenas para entidades carregadas pelo lazy load (pedido.ItensPedido <--).
            _contexto.Configuration.ProxyCreationEnabled = false;

            var lapis = new Produto();
            lapis.Nome = "Lápis";
            lapis.Preco = 9.36m;
            lapis.Estoque = 36;
            lapis.Categoria = _contexto.Categorias.Single(c => c.Nome == "Papelaria");

            _contexto.Produtos.Add(lapis);
            _contexto.SaveChanges();

            Assert.IsFalse(_contexto.Produtos.Any(p => p.Nome == "Lápis"));

            //var caneta = _contexto.Produtos.Single(p => p.Nome == "Grampeador");
            //caneta.Preco = 9.34m;
            //_contexto.SaveChanges();

            //caneta = _contexto.Produtos.Single(p => p.Nome == "Grampeador");

            //Assert.AreNotEqual(caneta.Preco, 9.34m);
        }

        [TestMethod]
        public void QueryableTeste()
        {
            var query = _contexto.Produtos.Where(p => p.Preco > 10);

            if (true)
            {
                query = query.Where(p => p.Estoque > 5);
            }

            query = query.OrderBy(p => p.Preco);

            var primeiro = query.First();
            //var ultimo = query.Last(); // Não pode ser convertido para uma consulta T-SQL.
            var unico = query.Single();
            var todos = query.ToList();
        }

        [TestMethod]
        public void AdicionarCliente()
        {
            var cliente = new Cliente();
            cliente.Nome = "Vítor";
            cliente.Endereco = new Endereco { Logradouro = "R. Tal" };

            _contexto.Clientes.Add(cliente);

            _contexto.SaveChanges();
        }

        [TestMethod]
        public void ExlcuirCliente()
        {
            var cliente = _contexto.Clientes.Single(c => c.Id == 1);

            _contexto.Clientes.Remove(cliente);

            _contexto.SaveChanges();
        }

        private bool ComparararNomeCategoria(Categoria c) { return c.Nome == "Papelaria"; }

        [ClassCleanup]
        public static void DescartarContexto()
        {
            //_contexto.Database.Delete();
            _contexto.Dispose();
        }
    }
}