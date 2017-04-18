using System.Collections.Generic;

using EvacuationFacilityLib.Models;
using EvacuationFacilityLib.Repositories;

namespace CreateEvacuationFacilityDb
{
    class Program
    {
        static void Main(string[] args)
        {
            // DocumentDB初期化
            EvacuationFacilityInfoRepository.Initialize(true);
            
            // XMLをロードしてカスタムクラスコレクションに変換
            // xmlのパスは書き換えてください！
            List<EvacuationFacilityInfo> evacuationFacilityInfoList = 
                GmlToCustomFormatConvertor.Load(@"J:\Projects\Github\research\DocumentDB\GIS\P20-12_13.xml");

            // カスタムクラスコレクションをDocumentDBに保存
            EvacuationFacilityInfoRepository.SaveEvacuationFacilityInfos(evacuationFacilityInfoList);
        }

    }
}
