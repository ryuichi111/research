﻿using System;
using System.Collections.Generic;

namespace CosmosDocDBExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("DocumentDbManagerクラスのEndpointUrlとPrimaryKeyをあなたの環境の値に設定の上で実行してください。");
            Console.WriteLine("また、2,500 RUのコレクションを作成します。コレクションを作成すると、RUに比例したCosmos DB利用料金が課金されますのでご注意ください。");
            Console.WriteLine("処理を続行して、よろしいですか？(y/n)");
            if (Console.ReadLine().ToUpper() != "Y")
            {
                Console.WriteLine("処理を中断しました。");
                return;
            }

            // データベースとコレクションを作成します。
            var manager = new DocumentDbManager();
            manager.CreateDatabase().Wait();
            manager.CreateCollection().Wait();

            // 利用する機能をコメントアウトしてください。
            //CallCreateDocument();
            //CallSaveDocument();
            //CallFindByRoom();
            //CallFindByRoom2();
            //CallCountByRoom();
            //CallFindByAssignMember();
            //CallFindById();
            //CallFindByIdWithParam();
            //CallDeleteById();
        }

        // リスト 9
        private static void CallCreateDocument()
        {
            var assignMembers = new List<AssignMember>();
            assignMembers.Add(new AssignMember()
            { UserId = "tanaka", UserName = "田中和夫" });
            assignMembers.Add(new AssignMember()
            { UserId = "sakamoto", UserName = "坂本寛子" });
            RoomReservationInfo item = new RoomReservationInfo()
            {
                Id = "00001",
                Room = "第１会議室",
                Title = "Cosmos DB移行についての打ち合わせ",
                ReservedUserId = "daigo",
                ReservedUserName = "醍醐竜一",
                Start = new DateTime(2017, 5, 30, 10, 0, 0),
                End = new DateTime(2017, 5, 30, 11, 0, 0),
                AssignMembers = assignMembers
            };

            // ドキュメント作成メソッド呼び出し
            var manager = new DocumentDbManager();
            manager.CreateDocument(item).Wait();
        }

        // リスト 12
        private static void CallSaveDocument()
        {
            var manager = new DocumentDbManager();

            var assignMembers = new List<AssignMember>();
            assignMembers.Add(new AssignMember() { UserId = "tanaka", UserName = "田中和夫" });
            assignMembers.Add(new AssignMember() { UserId = "sakamoto", UserName = "坂本寛子" });
            RoomReservationInfo item = new RoomReservationInfo()
            {
                Id = "00001",
                Room = "第１会議室",
                Title = "Cosmos DB移行についての打ち合わせ",
                ReservedUserId = "daigo",
                ReservedUserName = "醍醐竜一",
                Start = new DateTime(2017, 5, 30, 10, 0, 0),
                End = new DateTime(2017, 5, 30, 11, 0, 0),
                AssignMembers = assignMembers
            };
            manager.SaveDocument(item).Wait();

            //
            item.Room = "第２会議室";
            manager.SaveDocument(item).Wait();

            //
            item.Room = "第１会議室";
            item.Title = "タイトルを変更！";
            manager.SaveDocument(item).Wait();
        }

        // リスト 15
        private static void CallFindByRoom()
        {
            var manager = new DocumentDbManager();

            List<RoomReservationInfo> roomReservationIngos =
              manager.FindByRoom("スタンディングテーブル");

            // 検索結果出力
            Console.WriteLine(string.Format("{0}件", roomReservationIngos.Count));
            foreach (var info in roomReservationIngos)
            {
                Console.WriteLine(
                  string.Format("{0} / {1} / {2} ～ {3}", info.Room, info.Title, info.Start, info.End));
            }
        }

        // リスト 19
        private static void CallFindByRoom2()
        {
            var manager = new DocumentDbManager();
            List<RoomReservationInfo> roomReservationInfos = manager.FindByRoom2("第１会議室");

            Console.WriteLine(string.Format("{0}件", roomReservationInfos.Count));
            foreach (var info in roomReservationInfos)
            {
                Console.WriteLine(
                    string.Format("{0} / {1} / {2} ～ {3}", info.Room, info.Title, info.Start, info.End));
            }
        }

        private static void CallCountByRoom()
        {
            var manager = new DocumentDbManager();
            int count = manager.CountByRoom("第１会議室");
        }

        // リスト 22
        private static void CallFindByAssignMember()
        {
            var manager = new DocumentDbManager();

            List<RoomReservationInfo> roomReservationInfos = 
                manager.FindByAssignMember(
                    "第１会議室", 
                    new AssignMember() { UserId = "daigo", UserName = "醍醐竜一" });

            Console.WriteLine(string.Format("{0}件", roomReservationInfos.Count));
            foreach (var info in roomReservationInfos)
            {
                Console.WriteLine(
                    string.Format("{0} / {1} / {2} ～ {3}", info.Room, info.Title, info.Start, info.End));
            }
        }

        private static void CallFindById()
        {
            var manager = new DocumentDbManager();

            RoomReservationInfo info = manager.FindById("スタンディングテーブル", "0000000004");
        }

        private static void CallFindByIdWithParam()
        {
            var manager = new DocumentDbManager();

            RoomReservationInfo info = manager.FindByIdWithParam("スタンディングテーブル", "0000000004");
        }

        private static void CallDeleteById()
        {
            var manager = new DocumentDbManager();

            manager.DeleteById("スタンディングテーブル", "0000000004").Wait();
        }
    }
}
