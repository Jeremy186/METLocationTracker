using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MapsProposal.Models
{
    public enum Sattelite
    {
        Sentinel,
        Worldview
    }
    
    public class Subscription
    {
        public int ID { get; set;  }
        public Sattelite Sattelite { get; set; }
    }
    
}