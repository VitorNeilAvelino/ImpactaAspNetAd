﻿using Pubs.Dominio;
using MongoDB.Driver;
using System.Collections.Generic;
using System;
using System.Linq.Expressions;
using System.Configuration;
using MongoDB.Bson;

namespace Pubs.Repositorios.MongoDb
{
    public class RepositorioBase<T> where T : EntidadeBase
    {
        //private readonly IMongoDatabase _db;
        protected IMongoCollection<T> _colecao; /*refatorar depois*/

        protected RepositorioBase()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["pubsConnectionString"].ConnectionString;
            var databaseName = MongoUrl.Create(connectionString).DatabaseName;

            //As of today's version of MongoDB (v2.0.1.27 for MongoDB.Driver), 
            //there's no need to close or dispose of connections. The client handles it automatically.
            //https://stackoverflow.com/questions/32703051/properly-shutting-down-mongodb-database-connection-from-c-sharp-2-1-driver
            var _db = new MongoClient(connectionString).GetDatabase(databaseName);
            _colecao = _db.GetCollection<T>(typeof(T).Name);            
        }

        public void Inserir(T entidade)
        {
            _colecao.InsertOne(entidade);
        }

        public List<T> Selecionar()
        {
            return _colecao.Find(new BsonDocument()).ToList();
        }

        public T Selecionar(Guid guid)
        {
            return _colecao.Find(p => p.Id == guid).SingleOrDefault();
        }

        public List<T> Selecionar(Expression<Func<T, bool>> query)
        {
            return _colecao.Find(query).ToList();
        }

        public void Atualizar(T entidade)
        {
            _colecao.ReplaceOne(e => e.Id == entidade.Id, entidade);
        }

        public void Excluir(Guid id)
        {
            _colecao.DeleteOne(e => e.Id == id);
        }
    }
}