using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using Microsoft.Azure.Documents.Spatial;
using EvacuationFacilityLib.Models;

namespace CreateEvacuationFacilityDb
{
    public class GmlToCustomFormatConvertor
    {
        public static List<EvacuationFacilityInfo> Load(string xmlFilePath)
        {
            var result = new List<EvacuationFacilityInfo>();

            var xDoc = XDocument.Load(xmlFilePath);

            var nsmgr = new XmlNamespaceManager(new NameTable());
            nsmgr.AddNamespace("gml", "http://www.opengis.net/gml/3.2");
            nsmgr.AddNamespace("ksj", "http://nlftp.mlit.go.jp/ksj/schemas/ksj-app");
            nsmgr.AddNamespace("xlink", "http://www.w3.org/1999/xlink"); 

            var points = xDoc.XPathSelectElements("//gml:Point", nsmgr);
            foreach (var point in points)
            {
                var evacuationFacilityInfo = new EvacuationFacilityInfo();

                //var id = point.Attribute("gml:id").Value;
                evacuationFacilityInfo.ID = point.FirstAttribute.Value;
                
                //var pos = point.XPathSelectElement("gml:pos");
                string latitudeAndLongitude = point.Value;
                double dLatitude = double.Parse(latitudeAndLongitude.Split(' ')[0]);
                double dLongitude = double.Parse(latitudeAndLongitude.Split(' ')[1]);
                evacuationFacilityInfo.Latitude = dLatitude;
                evacuationFacilityInfo.Longitude = dLongitude;
                evacuationFacilityInfo.Location = new Point(dLongitude, dLatitude);

                var evacuationFacility = 
                    xDoc.XPathSelectElement("//ksj:EvacuationFacilities/ksj:position[@xlink:href='#" + evacuationFacilityInfo.ID + "']", nsmgr).Parent;
                evacuationFacilityInfo.Name = 
                    evacuationFacility.XPathSelectElement("ksj:name", nsmgr).Value;
                evacuationFacilityInfo.Address = 
                    evacuationFacility.XPathSelectElement("ksj:address", nsmgr).Value;
                evacuationFacilityInfo.FacilityType = 
                    evacuationFacility.XPathSelectElement("ksj:facilityType", nsmgr).Value;

                result.Add(evacuationFacilityInfo);
            }

            return result;
        }
    }
}



