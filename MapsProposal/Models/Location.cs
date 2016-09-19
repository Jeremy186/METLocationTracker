using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MapsProposal.Models
{
    public enum LocationType
    {
        Point,
        Rectangle
    }

    public class Location
    {

        public int ID { get; set; }
        public string Name { get; set; }

        public bool VegetationCover { get; set; }
        public bool LandUse { get; set; }
        public bool Forestry { get; set; }
        public bool Influx { get; set; }
        public bool SocialInfrastructure { get; set; }
        public bool SettlementSize { get; set; }
        public bool Water { get; set; }
        public bool Area { get; set; }
        public bool Height { get; set; }

        public LocationType LocationType { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? NorthEastLatitude { get; set; }
        public double? NorthEastLongitude { get; set; }
        public double? SouthWestLatitude { get; set; }
        public double? SouthWestLongitude { get; set; }
        public double? RectangleArea { get; set; }
       
    }
}