using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace CosmosDocDBExample
{
    public class DocumentDbManager
    {
        private const string EndpointUrl = "https://cosmosdoc.documents.azure.com:443/";
        private const string PrimaryKey = "7tWjWDQ8k7CxODpWaVr2TiOThgYBFzCmmeZMKWl8O0ZNTBEQdp2JrhEGNrFaeeak7k2z2C7sYNkP2IOjR3s9NQ==";
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
    }
}
