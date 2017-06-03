using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace CosmosDocDBExample
{
    public class DocumentDbManager
    {
        //private const string EndpointUrl = "https://cosmosdoc.documents.azure.com:443/";
        private const string EndpointUrl = "[your Cosmos DB URI";
        private const string PrimaryKey = "[your key]";
        private const string DatabaseId = "GroupwareDB";
        private const string CollectionId = "RoomReservations";

        private DocumentClient client = null;

        public DocumentDbManager()
        {
            this.client =
                new DocumentClient(
                    new Uri(DocumentDbManager.EndpointUrl),
                    DocumentDbManager.PrimaryKey);

        }

        public async Task<List<PartitionKeyRange>> ReadPartitionKeyRange()
        {
            var pkRanges =
                await this.client.ReadPartitionKeyRangeFeedAsync(
                    UriFactory.CreateDocumentCollectionUri(DocumentDbManager.DatabaseId, DocumentDbManager.CollectionId));

            return pkRanges.ToList();
        }

        public async Task<Database> CreateDatabase()
        {
            // データベースを作成
            Database database =
                await this.client.CreateDatabaseIfNotExistsAsync(
                    new Database { Id = DocumentDbManager.DatabaseId });

            return database;
        }

        public async Task<DocumentCollection> CreateCollection()
        {
            // パーティションキー指定あり
            DocumentCollection collection = new DocumentCollection();
            collection.Id = DocumentDbManager.CollectionId;
            collection.PartitionKey.Paths.Add("/Room");

            // スループットは 2500RU
            RequestOptions options = new RequestOptions();
            options.OfferThroughput = 2500;

            // コレクションを作成
            collection =
                await this.client.CreateDocumentCollectionIfNotExistsAsync(
                    UriFactory.CreateDatabaseUri(DocumentDbManager.DatabaseId),
                    collection, options);

            return collection;
        }

        public async Task<Document> CreateDocument(RoomReservationInfo roomReservationInfo)
        {
            var document =
                await this.client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(
                    DocumentDbManager.DatabaseId,
                    DocumentDbManager.CollectionId),
                roomReservationInfo);

            return document;
        }

        public async Task<Document> CreateDocumentWithPreTrigger(RoomReservationInfo roomReservationInfo)
        {
            RequestOptions options = new RequestOptions();
            options.PreTriggerInclude = new List<string>();
            options.PreTriggerInclude.Add("validatePreTrriger");

            var document =
                await this.client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(
                    DocumentDbManager.DatabaseId,
                    DocumentDbManager.CollectionId),
                roomReservationInfo,
                options);

            return document;
        }

        public async Task<Document> CreateDocumentWithPostTrigger(RoomReservationInfo roomReservationInfo)
        {
            RequestOptions options = new RequestOptions();
            options.PostTriggerInclude = new List<string>();
            options.PostTriggerInclude.Add("postTriggerExample");

            var document =
                await this.client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(
                    DocumentDbManager.DatabaseId,
                    DocumentDbManager.CollectionId),
                roomReservationInfo,
                options);

            return document;
        }


        public async Task<Document> SaveDocument(RoomReservationInfo roomReservationInfo)
        {

            var document =
                await this.client.UpsertDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(
                    DocumentDbManager.DatabaseId,
                    DocumentDbManager.CollectionId),
                roomReservationInfo);

            return document;
        }
        public async Task<Document> ReplaceDocument(RoomReservationInfo roomReservationInfo)
        {
            var doc = await this.client.ReplaceDocumentAsync(
                UriFactory.CreateDocumentUri(
                    DocumentDbManager.DatabaseId,
                    DocumentDbManager.CollectionId,
                    roomReservationInfo.Id),
                roomReservationInfo,
                new RequestOptions()
                {
                    AccessCondition = new AccessCondition()
                    {
                        Condition = roomReservationInfo._etag,
                        Type = AccessConditionType.IfMatch
                    }
                });

            return doc;
        }

        public List<RoomReservationInfo> FindByRoom(string room)
        {
            var query =
                this.client.CreateDocumentQuery<RoomReservationInfo>(
                    UriFactory.CreateDocumentCollectionUri(DocumentDbManager.DatabaseId, DocumentDbManager.CollectionId)).
                    Where(r => r.Room == room);
            var result = query.ToList();

            return result;
        }

        public List<RoomReservationInfo> FindByReservedUserId(string userId)
        {
            FeedOptions feedOptions = new FeedOptions()
            {
                EnableCrossPartitionQuery = true
            };

            var query =
                this.client.CreateDocumentQuery<RoomReservationInfo>(
                    UriFactory.CreateDocumentCollectionUri(DocumentDbManager.DatabaseId, DocumentDbManager.CollectionId),
                    feedOptions)
                    .Where(r => r.ReservedUserId == userId);
            var result = query.ToList();

            return result;
        }

        public List<RoomReservationInfo> FindByRoom2(string room)
        {
            FeedOptions feedOptions = new FeedOptions()
            {
                MaxItemCount = 5
            };

            var query =
                this.client.CreateDocumentQuery<RoomReservationInfo>(
                    UriFactory.CreateDocumentCollectionUri(DocumentDbManager.DatabaseId, DocumentDbManager.CollectionId),
                    feedOptions).
                    Where(r => r.Room == room);
            var result = query.ToList();

            return result;
        }


        public int CountByRoom(string room)
        {
            var query =
                this.client.CreateDocumentQuery<RoomReservationInfo>(
                    UriFactory.CreateDocumentCollectionUri(DocumentDbManager.DatabaseId, DocumentDbManager.CollectionId)).
                    Where(r => r.Room == room);
            var result = query.Count();

            return result;
        }

        public DateTime MaxByRoom(string room)
        {
            var query =
                this.client.CreateDocumentQuery<RoomReservationInfo>(
                    UriFactory.CreateDocumentCollectionUri(DocumentDbManager.DatabaseId, DocumentDbManager.CollectionId)).
                    Where(r => r.Room == room);
            var result = query.Max(r => r.Start);

            return result;
        }

        public List<RoomReservationInfo> FindByAssignMember(string room, AssignMember member)
        {
            var query =
                this.client.CreateDocumentQuery<RoomReservationInfo>(
                    UriFactory.CreateDocumentCollectionUri(DocumentDbManager.DatabaseId, DocumentDbManager.CollectionId))
                    .Where(r => r.Room == room && r.AssignMembers.Contains(member));
            var result = query.ToList();

            return result;
        }

        public List<RoomReservationInfo> FindByAssignMember(AssignMember member)
        {
            FeedOptions feedOptions =new FeedOptions() {
                EnableCrossPartitionQuery = true                
            };

            var query =
                this.client.CreateDocumentQuery<RoomReservationInfo>(
                    UriFactory.CreateDocumentCollectionUri(DocumentDbManager.DatabaseId, DocumentDbManager.CollectionId),
                    feedOptions).
                    Where(r => r.AssignMembers.Contains(member));
            var result = query.ToList();

            return result;
        }

        public List<RoomReservationInfo> FindByDate(string room, DateTime date)
        {
            var query =
                this.client.CreateDocumentQuery<RoomReservationInfo>(
                    UriFactory.CreateDocumentCollectionUri(DocumentDbManager.DatabaseId, DocumentDbManager.CollectionId))
                    .Where(r => r.Room == room && r.Start > date.Date && r.Start < date.Date.AddDays(1) );
            var result = query.ToList();

            return result;
        }

        public RoomReservationInfo FindById(string room, string id)
        {
            var query =
                this.client.CreateDocumentQuery<RoomReservationInfo>(
                    UriFactory.CreateDocumentCollectionUri(DocumentDbManager.DatabaseId, DocumentDbManager.CollectionId),
                    new SqlQuerySpec(string.Format("SELECT * FROM root WHERE root.Room = '{0}' AND root.id = '{1}'", room, id)));
            var list = query.ToList();

            return list.Count > 0 ? list[0] : null;
        }

        public RoomReservationInfo FindByIdWithParam(string room, string id)
        {
            SqlQuerySpec sqlQuerySpec = new SqlQuerySpec();
            sqlQuerySpec.QueryText = "SELECT * FROM root WHERE root.Room = @room AND root.id = @id";
            sqlQuerySpec.Parameters.Add(new SqlParameter("@room", room));
            sqlQuerySpec.Parameters.Add(new SqlParameter("@id", id));

            var query =
                this.client.CreateDocumentQuery<RoomReservationInfo>(
                    UriFactory.CreateDocumentCollectionUri(DocumentDbManager.DatabaseId, DocumentDbManager.CollectionId),
                    sqlQuerySpec);

            var list = query.ToList();

            return list.Count > 0 ? list[0] : null;
        }

        public async Task<Document> DeleteById(string room, string id)
        {
            RequestOptions requestOptions = new RequestOptions();
            requestOptions.PartitionKey = new PartitionKey(room);

            var document = await this.client.DeleteDocumentAsync(
                UriFactory.CreateDocumentUri(DocumentDbManager.DatabaseId, DocumentDbManager.CollectionId, id),
                requestOptions);

            return document;
        }


        public async Task<string> CallHelloStoredProcedure(string yourName)
        {
            RequestOptions options = new RequestOptions();
            options.PartitionKey = new PartitionKey("第１会議室");
            var result = await this.client.ExecuteStoredProcedureAsync<string>(
                UriFactory.CreateStoredProcedureUri(DocumentDbManager.DatabaseId, DocumentDbManager.CollectionId, "helloStoredProcedure"), options,
                new DateTime(2017, 6, 1));
                //yourName);

            return result;
            
            /* パーティションキーの無いコレクションに対する呼び出しの場合
            var result = await this.client.ExecuteStoredProcedureAsync<string>(
                UriFactory.CreateStoredProcedureUri(DocumentDbManager.DatabaseId, DocumentDbManager.CollectionId, "helloStoredProcedure"), 
                yourName);

            return result;
            */
        }

        public async Task<bool> CreateStoredProcedure(string scriptFileName)
        {
            string procedureId = Path.GetFileNameWithoutExtension(scriptFileName);

            var storedProcedure = new StoredProcedure
            {
                Id = procedureId,
                Body = File.ReadAllText(scriptFileName)
            };

            Uri collectionUrl =
                UriFactory.CreateDocumentCollectionUri(
                    DocumentDbManager.DatabaseId,
                    DocumentDbManager.CollectionId);

            // 存在したら削除
            var currentStoredProcedure = this.client.CreateStoredProcedureQuery(collectionUrl).Where(sp => sp.Id == procedureId).AsEnumerable().FirstOrDefault();
            if (currentStoredProcedure != null)
            {
                var sp = await this.client.DeleteStoredProcedureAsync(currentStoredProcedure.SelfLink);
            }

            // 作成
            storedProcedure = await client.CreateStoredProcedureAsync(collectionUrl, storedProcedure);

            return true;
        }

        public async Task<int> CallBulkDeleteStoredProcedure(DateTime fromDate, DateTime toDate, string room)
        {
            RequestOptions options = new RequestOptions();
            options.PartitionKey = new PartitionKey(room);
            var result = await this.client.ExecuteStoredProcedureAsync<int>(
                UriFactory.CreateStoredProcedureUri(
                    DocumentDbManager.DatabaseId, 
                    DocumentDbManager.CollectionId, "bulkDeleteStoredProcedure"), 
                options,
                fromDate, toDate);

            return result;

            /* パーティションキーの無いコレクションに対する呼び出しの場合（この場合、すべてのRoomが削除対象となる）
            var result = await this.client.ExecuteStoredProcedureAsync<int>(
                UriFactory.CreateStoredProcedureUri(
                    DocumentDbManager.DatabaseId, 
                    DocumentDbManager.CollectionId, "bulkDeleteStoredProcedure"), 
                fromDate, toDate);

            return result;
            */
        }

        public async Task<bool> CreatePreTrigger(string scriptFileName)
        {
            string triggerId = Path.GetFileNameWithoutExtension(scriptFileName);

            var trigger = new Trigger
            {
                Id = triggerId,
                TriggerType = TriggerType.Pre,
                TriggerOperation = TriggerOperation.Create,
                Body = File.ReadAllText(scriptFileName)
            };

            Uri collectionUrl =
                UriFactory.CreateDocumentCollectionUri(
                    DocumentDbManager.DatabaseId,
                    DocumentDbManager.CollectionId);

            // 存在したら削除
            var currentTrigger = this.client.CreateTriggerQuery(collectionUrl).Where(tr => tr.Id == triggerId).AsEnumerable().FirstOrDefault();
            if (currentTrigger != null)
            {
                var sp = await this.client.DeleteTriggerAsync(currentTrigger.SelfLink);
            }

            // 作成
            trigger = await client.CreateTriggerAsync(collectionUrl, trigger);

            return true;
        }

        public async Task<bool> CreatePostTrigger(string scriptFileName)
        {
            string triggerId = Path.GetFileNameWithoutExtension(scriptFileName);

            var trigger = new Trigger
            {
                Id = triggerId,
                TriggerType = TriggerType.Post,
                TriggerOperation = TriggerOperation.Create,
                Body = File.ReadAllText(scriptFileName)
            };

            Uri collectionUrl =
                UriFactory.CreateDocumentCollectionUri(
                    DocumentDbManager.DatabaseId,
                    DocumentDbManager.CollectionId);

            // 存在したら削除
            var currentTrigger = this.client.CreateTriggerQuery(collectionUrl).Where(tr => tr.Id == triggerId).AsEnumerable().FirstOrDefault();
            if (currentTrigger != null)
            {
                var sp = await this.client.DeleteTriggerAsync(currentTrigger.SelfLink);
            }

            // 作成
            trigger = await client.CreateTriggerAsync(collectionUrl, trigger);

            return true;
        }

        public async Task<bool> CreateUdf(string scriptFileName)
        {
            string udfId = Path.GetFileNameWithoutExtension(scriptFileName);

            var udf = new UserDefinedFunction
            {
                Id = udfId,
                Body = File.ReadAllText(scriptFileName)
            };

            Uri collectionUrl =
                UriFactory.CreateDocumentCollectionUri(
                    DocumentDbManager.DatabaseId,
                    DocumentDbManager.CollectionId);

            // 存在したら削除
            var currentUdf = this.client.CreateUserDefinedFunctionQuery(collectionUrl).Where( u => u.Id == udfId).AsEnumerable().FirstOrDefault();
            if (currentUdf != null)
            {
                var sp = await this.client.DeleteUserDefinedFunctionAsync(
                    UriFactory.CreateUserDefinedFunctionUri(
                        DocumentDbManager.DatabaseId, 
                        DocumentDbManager.CollectionId, 
                        currentUdf.Id));
            }

            // 作成
            udf = await client.CreateUserDefinedFunctionAsync(collectionUrl, udf);

            return true;
        }

        public List<RoomReservationInfo> FindUsingUdf(string room, int minutes)
        {
            var query =
                this.client.CreateDocumentQuery<RoomReservationInfo>(
                    UriFactory.CreateDocumentCollectionUri(DocumentDbManager.DatabaseId, DocumentDbManager.CollectionId),
                    new SqlQuerySpec(
                        string.Format(
                            "SELECT * FROM root WHERE root.Room = '{0}' " +
                            "AND udf.calcTimeUdf(root.Start, root[\"End\"]) > {1}"
                            , room, minutes)));
            var list = query.ToList();

            return list;
        }
    }

}
