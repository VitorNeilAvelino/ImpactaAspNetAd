﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using NorthWind.Repositorios.SqlServer.Ado;

namespace NorthWind.Repositorios.SqlServer.Tests
{
    [TestClass()]
    public class CategoriaRepositorioTests
    {
        [TestMethod()]
        public void ObterTest()
        {
            var categoriasDataTable = new CategoriaRepositorio().Selecionar();

            Assert.AreNotEqual(categoriasDataTable.Rows.Count, 0);

            System.Console.WriteLine(categoriasDataTable.Rows[0]["CategoryName"]);

            foreach (DataRow registro in categoriasDataTable.Rows)
            {
                System.Console.WriteLine($"{registro[0]} - {registro[1]}");
            }
        }
    }
}