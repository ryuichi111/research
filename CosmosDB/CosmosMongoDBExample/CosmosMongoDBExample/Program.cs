using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Driver;

namespace CosmosMongoDBExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("MongoDbDbManagerクラスのhost / userName / password をあなたの環境の値に設定の上で実行してください。");
            Console.WriteLine("また、1,000RUのコレクションを作成します。コレクションを作成すると、RUに比例したCosmos DB利用料金が課金されますのでご注意ください。");
            Console.WriteLine("処理を続行して、よろしいですか？(y/n)");
            if (Console.ReadLine().ToUpper() != "Y")
            {
                Console.WriteLine("処理を中断しました。");
                return;
            }

            // 利用する機能をコメントアウトしてください。
            //  データベース・コレクションの作成
            //CreateCollection();
            //  1件のドキュメントを作成
            //CreateRoomReservationInfo();
            //  Roomによる検索
            //CallFindByRoom();
            //  Roomによる検索
            //CallFindByRoomEx();
            //  RoomとAssignMemberによる検索
            //CallFindByRoomAndAssignMember();
            //  LINQでRoomよる検索
            //CallFindByRoomLinq();
            //  更新
            //CallUpdate();
            //  削除処理
            //CallDelete();
        }

        public static void CreateCollection()
        {
            MongoDbManager manager = new MongoDbManager();
            bool result = manager.CreateCollection().Result;
        }

        public static void CreateRoomReservationInfo()
        {
            MongoDbManager manager = new MongoDbManager();

            RoomReservationInfo roomReservationInfo = new RoomReservationInfo();
            roomReservationInfo.Id = "A0000000001";
            roomReservationInfo.Room = "第１会議室";
            roomReservationInfo.ReservedUserId = "daigo";
            roomReservationInfo.ReservedUserName = "醍醐竜一";
            roomReservationInfo.Title = "MongoDB導入打ち合わせ";
            roomReservationInfo.Start = new DateTime(2017, 7, 5, 10, 0, 0);
            roomReservationInfo.End = new DateTime(2017, 7, 5, 12, 0, 0);
            roomReservationInfo.AssignMembers = new List<AssignMember>();
            roomReservationInfo.AssignMembers.Add(
                new AssignMember() { UserId = "sakata", UserName = "坂田守" });

            bool result = manager.CreateRoomReservationInfo(roomReservationInfo).Result;
        }

        public static void CallFindByRoom()
        {
            MongoDbManager manager = new MongoDbManager();

            var roomReservationInfos = manager.FindByRoom("第１会議室").Result;

            Console.WriteLine(string.Format("{0}件", roomReservationInfos.Count));
            foreach (var info in roomReservationInfos)
            {
                Console.WriteLine(
                    string.Format("{0} / {1} / {2} ～ {3}", info.Room, info.Title, info.Start, info.End));
            }
        }

        public static void CallFindByRoomEx()
        {
            MongoDbManager manager = new MongoDbManager();

            var roomReservationInfos = manager.FindByRoomEx("第１会議室").Result;

            Console.WriteLine(string.Format("{0}件", roomReservationInfos.Count));
            foreach (var info in roomReservationInfos)
            {
                Console.WriteLine(
                    string.Format("{0} / {1} / {2} ～ {3}", info.Room, info.Title, info.Start, info.End));
            }
        }

        public static void CallFindByRoomAndAssignMember()
        {
            MongoDbManager manager = new MongoDbManager();

            var roomReservationInfos = manager.FindByRoomAndAssignMember("第１会議室", "daigo").Result;

            Console.WriteLine(string.Format("{0}件", roomReservationInfos.Count));
            foreach (var info in roomReservationInfos)
            {
                Console.WriteLine(
                    string.Format("{0} / {1} / {2} ～ {3}", info.Room, info.Title, info.Start, info.End));
            }
        }

        public static void CallFindByRoomLinq()
        {
            MongoDbManager manager = new MongoDbManager();

            var roomReservationInfos = manager.FindByRoomLinq("第１会議室").Result;

            Console.WriteLine(string.Format("{0}件", roomReservationInfos.Count));
            foreach (var info in roomReservationInfos)
            {
                Console.WriteLine(
                    string.Format("{0} / {1} / {2} ～ {3}", info.Room, info.Title, info.Start, info.End));
            }
        }

        public static void CallUpdate()
        {
            MongoDbManager manager = new MongoDbManager();

            var roomReservationInfo = manager.FindByRoomAndId("第１会議室", "0000000000").Result;

            roomReservationInfo.Title = "タイトル変更！！！！";

            var result = manager.UpdateTitle(roomReservationInfo).Result;
        }

        public static void CallDelete()
        {
            MongoDbManager manager = new MongoDbManager();

            var roomReservationInfo = manager.FindByRoomAndId("第１会議室", "0000000001").Result;

            var result = manager.Delete(roomReservationInfo).Result;
        }
    }
}
