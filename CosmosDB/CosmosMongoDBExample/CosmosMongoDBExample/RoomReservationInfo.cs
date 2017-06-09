using System;
using System.Collections.Generic;

namespace CosmosMongoDBExample
{
    public class RoomReservationInfo
    {
        public string Id { get; set; }

        /// <summary>
        /// 会議室名を取得または設定します。
        /// </summary>
        public string Room { get; set; }

        /// <summary>
        /// 会議名を取得または設定します。
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 予約者IDを取得または設定します。
        /// </summary>
        public string ReservedUserId { get; set; }

        /// <summary>
        /// 予約者名を取得または設定します。
        /// </summary>
        public string ReservedUserName { get; set; }

        /// <summary>
        /// 開始日時を取得または設定します。
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// 終了日時を取得または設定します。
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// 参加メンバーを取得または設定します。
        /// </summary>
        public List<AssignMember> AssignMembers { get; set; }
    }

    public class AssignMember
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

    }
}
