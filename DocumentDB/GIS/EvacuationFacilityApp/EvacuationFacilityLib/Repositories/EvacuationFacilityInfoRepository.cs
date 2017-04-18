#define LOCAL_DB

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

using Microsoft.Azure.Documents.Spatial;

using EvacuationFacilityLib.Models;

namespace EvacuationFacilityLib.Repositories
{
    public class EvacuationFacilityInfoRepository
    {
        /// <summary>
        /// 任意のデータベースID
        /// </summary>
        private static readonly string DatabaseId = "EvacuationFacilityInfoDb";

        /// <summary>
        /// 任意のコレクションID
        /// </summary>
        private static readonly string CollectionId = "EvacuationFacilityInfoCollection";

        /// <summary>
        /// エンドポイント
        /// </summary>
#if LOCAL_DB
        private static readonly string EndPoint = "https://localhost:8081/";
#else
        private static readonly string EndPoint = "[your endpoint]";
#endif

        /// <summary>
        /// 認証キー（固定）
        /// </summary>
#if LOCAL_DB
            private static readonly string AuthKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
#else
        private static readonly string AuthKey = "[your auth key]";
#endif

        private static DocumentClient client;


        public static void SaveEvacuationFacilityInfos(List<EvacuationFacilityInfo> evacuationFacilityInfos)
        {
            foreach (var info in evacuationFacilityInfos)
            {
                client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), info);
            }
        }

        public static List<EvacuationFacilityInfo> Search(double longitude, double latitude, int distance)
        {
            List<EvacuationFacilityInfo> result = null;

            var query = 
                client.CreateDocumentQuery<EvacuationFacilityInfo>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId),
                    new FeedOptions { MaxItemCount = -1 })
                    .Where(i => i.Location.Distance(new Point(longitude, latitude)) < distance);

            result = query.ToList();

            return result;
        }

        /// <summary>
        /// データベース・コレクションの初期化を行います。
        /// </summary>
        public static void Initialize(bool initDatabase = false)
        {
            client = new DocumentClient(new Uri(EndPoint), AuthKey, new ConnectionPolicy { EnableEndpointDiscovery = false });
            if (initDatabase)
            {
                CreateDatabaseIfNotExistsAsync().Wait();
                CreateCollectionIfNotExistsAsync().Wait();
            }
        }

        /// <summary>
        /// 存在しなければデータベースを作成します。
        /// </summary>
        /// <returns></returns>
        private static async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = DatabaseId });
                }
                else
                {
                    throw;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 存在しなければコレクションを作成します。
        /// </summary>
        /// <returns></returns>
        private static async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    // DataType.Point に対して SpatialIndex 付加してコレクションを作成
                    await client.CreateDocumentCollectionAsync(
                      UriFactory.CreateDatabaseUri(DatabaseId),
                      new DocumentCollection { Id = CollectionId, IndexingPolicy = new IndexingPolicy(new SpatialIndex(DataType.Point)) },
                      new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
