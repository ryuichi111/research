// 一般的に利用する名前空間
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;

// MongoDB関連の名前空間
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Driver.Linq;

namespace CosmosMongoDBExample
{
    public class MongoDbManager
    {
        private const string host = "[your Cosmos DB HOST]";
        private const string userName = "[your UserName]";
        private const string password = "[your Password]";
        private const string database = "GroupwareDB";
        private const string collection = "RoomReservations";
        private const int port = 10255;

        private MongoClient client = null;

        public MongoDbManager()
        {
            // MongoClientの初期化
            var settings = new MongoClientSettings
            {
                Server = new MongoServerAddress(MongoDbManager.host, MongoDbManager.port),
                ServerSelectionTimeout = TimeSpan.FromSeconds(5)
            };
            settings.SslSettings = new SslSettings();
            settings.UseSsl = true;
            settings.SslSettings.EnabledSslProtocols = SslProtocols.Tls12;
            // ローカルMongoDBにオレオレ証明書で接続する場合は、以下のコメントはずす
            //settings.SslSettings.CheckCertificateRevocation = false;
            //settings.VerifySslCertificate = false;

            MongoIdentity identity = new MongoInternalIdentity(MongoDbManager.database, MongoDbManager.userName);
            MongoIdentityEvidence evidence = new PasswordEvidence(MongoDbManager.password);

            settings.Credentials = new List<MongoCredential>()
            {
                new MongoCredential("SCRAM-SHA-1", identity, evidence)
            };
            this.client = new MongoClient(settings);
        }

        public async Task<bool> CreateCollection()
        {
            IMongoDatabase mongoDatabase = this.client.GetDatabase(MongoDbManager.database);
            await mongoDatabase.CreateCollectionAsync(MongoDbManager.collection);

            return true;
        }

        public async Task<bool> CreateRoomReservationInfo(RoomReservationInfo roomReservationInfo)
        {
            var mongoDatabase = this.client.GetDatabase(MongoDbManager.database);
            var mongoCollection = mongoDatabase.GetCollection<RoomReservationInfo>(MongoDbManager.collection);
            await mongoCollection.InsertOneAsync(roomReservationInfo);

            return true;
        }

        public async Task<RoomReservationInfo> FindByRoomAndId(string room, string id)
        {
            var mongoDatabase = this.client.GetDatabase(MongoDbManager.database);
            var mongoCollection = mongoDatabase.GetCollection<RoomReservationInfo>(MongoDbManager.collection);

            var find = await mongoCollection.FindAsync<RoomReservationInfo>("{ Room: \"" + room + "\", _id: \"" + id + "\"}");

            return find.FirstOrDefault();
        }

        public async Task<List<RoomReservationInfo>> FindByRoom(string room)
        {
            var mongoDatabase = this.client.GetDatabase(MongoDbManager.database);
            var mongoCollection = mongoDatabase.GetCollection<RoomReservationInfo>(MongoDbManager.collection);

            var find = await mongoCollection.FindAsync<RoomReservationInfo>("{ Room: \"" + room + "\"}");

            return find.ToList();
        }

        public async Task<List<RoomReservationInfo>> FindByRoomEx(string room)
        {
            var mongoDatabase = this.client.GetDatabase(MongoDbManager.database);
            var mongoCollection = mongoDatabase.GetCollection<RoomReservationInfo>(MongoDbManager.collection);

            FilterDefinition<RoomReservationInfo> query = 
                Builders<RoomReservationInfo>.Filter.Eq(r => r.Room, room);
            var find = await mongoCollection.FindAsync<RoomReservationInfo>(query);

            return find.ToList();
        }

        public async Task<List<RoomReservationInfo>> FindByRoomAndAssignMember(string room, string assignMemberId)
        {
            var mongoDatabase = this.client.GetDatabase(MongoDbManager.database);
            var mongoCollection = mongoDatabase.GetCollection<RoomReservationInfo>(MongoDbManager.collection);

            var find = await mongoCollection.FindAsync<RoomReservationInfo>("{ Room: \"" + room + "\" , AssignMembers: { $elemMatch: { UserId: \"" + assignMemberId + "\"}}}");

            return find.ToList();
        }

        public async Task<List<RoomReservationInfo>> FindByRoomLinq(string room)
        {
            var mongoDatabase = this.client.GetDatabase(MongoDbManager.database);
            var mongoCollection = mongoDatabase.GetCollection<RoomReservationInfo>(MongoDbManager.collection);

            var find = await mongoCollection.FindAsync<RoomReservationInfo>(r => r.Room == room);

            return find.ToList();
        }

        public async Task<bool> UpdateTitle(RoomReservationInfo roomReservationInfo)
        {
            var mongoDatabase = this.client.GetDatabase(MongoDbManager.database);
            var mongoCollection = mongoDatabase.GetCollection<RoomReservationInfo>(MongoDbManager.collection);

            var result = await mongoCollection.UpdateOneAsync<RoomReservationInfo>(
                r => r.Id == roomReservationInfo.Id && r.Room == roomReservationInfo.Room, 
                new UpdateDefinitionBuilder<RoomReservationInfo>().Set(i => i.Title, roomReservationInfo.Title));

            return true;
        }

        public async Task<bool> Delete(RoomReservationInfo roomReservationInfo)
        {
            var mongoDatabase = this.client.GetDatabase(MongoDbManager.database);
            var mongoCollection = mongoDatabase.GetCollection<RoomReservationInfo>(MongoDbManager.collection);

            var result = await mongoCollection.DeleteOneAsync<RoomReservationInfo>(
                r => r.Id == roomReservationInfo.Id && r.Room == roomReservationInfo.Room);

            return true;
        }
    }
}
