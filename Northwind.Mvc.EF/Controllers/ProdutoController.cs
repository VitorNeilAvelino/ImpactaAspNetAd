﻿using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using NorthWind.Repositorios.SqlServer.EF;
using Northwind.Dominio;
using Northwind.Mvc.EF.ViewModels;

namespace Northwind.Mvc.EF.Controllers
{
    public class ProdutoController : Controller
    {
        private LojaDbContext db = new LojaDbContext();

        // GET: Produto
        public ActionResult Index()
        {
            return View(db.Produtos.OrderBy(p => p.Nome).ToList());
        }

        // GET: Produto/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Produto produto = db.Produtos.Find(id);
            if (produto == null)
            {
                return HttpNotFound();
            }
            return View(produto);
        }

        // GET: Produto/Create
        public ActionResult Create()
        {
            return View(new ProdutoViewModel(db.Categorias.ToList()));
        }

        // POST: Produto/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Produto produto)
        {
            if (ModelState.IsValid)
            {
                produto.Categoria = db.Categorias.Find(produto.Categoria.Id);

                db.Produtos.Add(produto);

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(produto);
        }

        // GET: Produto/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var produto = db.Produtos.Find(id);

            if (produto == null)
            {
                return HttpNotFound();
            }

            return View(new ProdutoViewModel(db.Categorias.ToList(), produto));
        }

        // POST: Produto/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Produto produto)
        {
            if (ModelState.IsValid)
            {
                var produtoBanco = db.Produtos.Find(produto.Id);

                db.Entry(produtoBanco).CurrentValues.SetValues(produto);

                produtoBanco.Categoria = db.Categorias.Single(c => c.Id == produto.Categoria.Id);
                
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(produto);
        }

        // GET: Produto/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Produto produto = db.Produtos.Find(id);
            if (produto == null)
            {
                return HttpNotFound();
            }
            return View(produto);
        }

        // POST: Produto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Produto produto = db.Produtos.Find(id);
            db.Produtos.Remove(produto);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}