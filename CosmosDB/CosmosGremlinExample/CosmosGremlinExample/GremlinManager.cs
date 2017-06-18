using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// Cosmos DB(Gremlin)関連の名前空間
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using System.Text;

namespace CosmosGremlinExample
{
    public class GremlinManager
    {
        private const string EndPoint = "【エンドポイント】";
        private const string AuthKey = "【認証キー】";
        private const string DatabaseId = "BookStoreDb";
        private const string CollectionId = "BookStoreCollection";

        private DocumentClient client = null;
        private DocumentCollection collection = null;

        public GremlinManager()
        {
            this.client = new DocumentClient(
                new Uri(GremlinManager.EndPoint),
                GremlinManager.AuthKey,
                new ConnectionPolicy
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConnectionProtocol = Protocol.Tcp
                });
        }

        public async Task<Database> CreateDatabase()
        {
            // データベースを作成
            Database database =
                await this.client.CreateDatabaseIfNotExistsAsync(
                    new Database { Id = GremlinManager.DatabaseId });

            return database;
        }

        public async Task<bool> DropVertexAndEdge()
        {
            var ret1 =
                await this.client.CreateGremlinQuery<dynamic>(
                    this.collection, "g.V().drop()")
                .ExecuteNextAsync();
            var ret2 =
                await this.client.CreateGremlinQuery<dynamic>(
                    this.collection, "g.E().drop()")
                .ExecuteNextAsync();

            return true;
        }

        public async Task<DocumentCollection> CreateCollection()
        {
            // パーティションキー指定はなし
            DocumentCollection collection = new DocumentCollection();
            collection.Id = GremlinManager.CollectionId;

            // スループットは 400RU
            RequestOptions options = new RequestOptions();
            options.OfferThroughput = 400;

            // コレクションを作成
            this.collection =
                await this.client.CreateDocumentCollectionIfNotExistsAsync(
                    UriFactory.CreateDatabaseUri(GremlinManager.DatabaseId),
                    collection, options);

            return collection;
        }

        public async Task<bool> AddVertex(string label, string id)
        {
            string gr = string.Format("g.addV('{0}').property('id', '{1}')", label, id);

            var ret = 
                await this.client.CreateGremlinQuery<dynamic>(
                    this.collection,gr)
                .ExecuteNextAsync();

            return true;
        }

        public async Task<bool> AddVertex(string label, string id, Dictionary<string, string> properties)
        {
            StringBuilder grSb = new StringBuilder(
                string.Format("g.addV('{0}').property('id', '{1}')", label, id));
            foreach (string key in properties.Keys)
            {
                grSb.Append(string.Format(".property('{0}', '{1}')", key, properties[key]));
            }

            var ret =
                await this.client.CreateGremlinQuery<dynamic>(
                    this.collection, grSb.ToString())
                .ExecuteNextAsync();

            return true;
        }

        public async Task<bool> SetProperties(string id, Dictionary<string, string> properties)
        {
            StringBuilder grSb = new StringBuilder(
                string.Format("g.V('{0}')", id));
            foreach (string key in properties.Keys)
            {
                grSb.Append(string.Format(".property('{0}', '{1}')", key, properties[key]));
            }

            var ret =
                await this.client.CreateGremlinQuery<dynamic>(
                    this.collection, grSb.ToString())
                .ExecuteNextAsync();

            return true;
        }

        public async Task<bool> AddEdge(string label, string fromId, string toId)
        {
            string gr = string.Format(
                "g.V('{0}').addE('{1}').to(g.V('{2}'))", 
                fromId, label, toId);

            var ret =
                await this.client.CreateGremlinQuery<dynamic>(
                    this.collection, gr)
                .ExecuteNextAsync();

            return true;
        }

        public async Task<List<dynamic>> GetAllVertex()
        {
            List<dynamic> result = new List<dynamic>();

            var query =
                this.client.CreateGremlinQuery<dynamic>(
                    this.collection, "g.V()");

            if(query.HasMoreResults)
            {
                foreach (dynamic item in await query.ExecuteNextAsync())
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public async Task<dynamic> GetVertexById(string id)
        {
            dynamic result = null;

            string gr = string.Format("g.V('{0}')", id);

            var query =
                this.client.CreateGremlinQuery<dynamic>(
                    this.collection, gr);

            if (query.HasMoreResults)
            {
                foreach (dynamic item in await query.ExecuteNextAsync())
                {
                    result = item;
                }
            }

            return result;
        }

        public async Task<List<dynamic>> GetAllEdge()
        {
            List<dynamic> result = new List<dynamic>();

            var query =
                this.client.CreateGremlinQuery<dynamic>(
                    this.collection, "g.E()");

            if (query.HasMoreResults)
            {
                foreach (dynamic item in await query.ExecuteNextAsync())
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public async Task<List<dynamic>> GetSameOrderCustomers(string id)
        {
            List<dynamic> result = new List<dynamic>();

            string gr = string.Format(
                "g.V('{0}').as('self').outE('order').inV().inE().outV().where(neq('self'))",
                id);
            // 上記の省略形は以下です。
            //string gr = string.Format(
            //    "g.V('{0}').as('self').out('order').in().where(neq('self'))",
            //    id);

            var query =
                this.client.CreateGremlinQuery<dynamic>(
                    this.collection, gr);

            if(query.HasMoreResults)
            {
                foreach (dynamic item in await query.ExecuteNextAsync())
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public async Task<List<dynamic>> GetRecomendBooks(string id)
        {
            List<dynamic> result = new List<dynamic>();

            string gr = string.Format(
                "g.V('{0}').as('self').outE('order').inV().as('sourceBook').inE().outV().where(neq('self')).outE('order').inV().where(neq('sourceBook'))",
                id);

            var query =
                this.client.CreateGremlinQuery<dynamic>(
                    this.collection, gr);

            if (query.HasMoreResults)
            {
                foreach (dynamic item in await query.ExecuteNextAsync())
                {
                    result.Add(item);
                }
            }

            return result;
        }
    }
}
