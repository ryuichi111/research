using Microsoft.Azure.Documents.Spatial;

namespace EvacuationFacilityLib.Models
{
    public class EvacuationFacilityInfo
    {
        // ID
        public string ID { get; set; }

        // 緯度
        public double Latitude { get; set; }

        // 経度
        public double Longitude { get; set; }

        // DocumentDBクエリー用のPoint型
        public Point Location { get; set; }

        // 施設名
        public string Name { get; set; }

        // 住所
        public string Address { get; set; }

        // 施設タイプ
        public string FacilityType { get; set; }
    }
}