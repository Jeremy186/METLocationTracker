using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MapsProposal.DAL;
using MapsProposal.Models;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using System.IO;
using System.Text;
using System.Xml;

namespace MapsProposal.Controllers
{
    public class LocationController : Controller
    {
        private MapsProposalContext db = new MapsProposalContext();

        // GET: Location
        public ActionResult Index()
        {
            var userId = Guid.Parse(User.Identity.GetUserId());
            return View(db.Locations.Where(l => l.UserId == userId).ToList());


        }

        public class Layer
        {
            public string Name;
            public double xMin { get; set; }
            public double xMax { get; set; }
            public double yMin { get; set; }
            public double yMax { get; set; }

            public Layer(string name, double xmin, double xmax, double ymin, double ymax)
            {
                Name = name;
                xMax = xmax;
                xMin = xmin;
                yMin = ymin;
                yMax = ymax;
            }
        }

        public ActionResult Images(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }

            var getCapabilitiesRequest = WebRequest.Create("https://resources.giscloud.com/wms/f5dff74a4a4d330bfc38bda9ad28faa6?SERVICE=WMS&REQUEST=GetCapabilities");

            var getCapabilitiesResponse = getCapabilitiesRequest.GetResponse();
            Stream receiveStream = getCapabilitiesResponse.GetResponseStream();
            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            string xml = readStream.ReadToEnd();
            getCapabilitiesResponse.Close();
            readStream.Close();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            List<Layer> layers = new List<Layer>();

            var rootLayers = doc.SelectNodes("/WMT_MS_Capabilities/Capability/Layer");
            foreach (XmlNode rootLayer in rootLayers)
            {
                if (rootLayer.SelectSingleNode("BoundingBox") != null)
                {
                    var rootLayerBBox = rootLayer.SelectSingleNode("BBox");
                    layers.Add(new Layer(rootLayer.SelectSingleNode("Title").Value, 
                        Convert.ToDouble(rootLayerBBox.Attributes["xmin"].Value),
                        Convert.ToDouble(rootLayerBBox.Attributes["xmax"].Value),
                        Convert.ToDouble(rootLayerBBox.Attributes["ymin"].Value),
                        Convert.ToDouble(rootLayerBBox.Attributes["ymax"].Value)
                        ));
                }
                    
                var layerCollection = rootLayer.SelectNodes("Layer");
                foreach (XmlNode layer in layerCollection)
                {
                    XmlNode title = layer.SelectSingleNode("Title");
                    Console.WriteLine(title.InnerText);
                    var subLayerCollection = layer.SelectNodes("Layer");
                    foreach (XmlNode subLayer in subLayerCollection)
                    {
                        XmlNode subLayerTitle = subLayer.SelectSingleNode("Title");
                        Console.WriteLine("\t" + subLayerTitle.InnerText);
                        if (subLayer.SelectSingleNode("BoundingBox") != null)
                        {
                            DisplayBox(subLayer);
                        }
                            
                    }
                }
            }

            //&BBOX=3237474,5039357,3243535,5045417
            string requestUrl = "https://resources.giscloud.com/wms/f5dff74a4a4d330bfc38bda9ad28faa6?SERVICE=WMS&REQUEST=GetMap&WIDTH=640&HEIGHT=640&FORMAT=image/png&bbox=-118.439916807,34.8322196664,-118.008444218,35.1945509515&styles=layer1437818_style&SRS=EPSG:4326&layers=1437818:sentinel";
            //string bboxParams = "&BBOX=" + location.SouthWestLongitude + "," + location.NorthEastLongitude + "," + location.SouthWestLatitude + "," + location.NorthEastLatitude;
            //requestUrl += bboxParams;

            var request = WebRequest.Create(requestUrl);
            var response = request.GetResponse();
            var bitmap = System.Drawing.Image.FromStream(response.GetResponseStream());
            response.Close();

            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] imageData = ms.ToArray();

            ViewBag.Layers = layers;
            ViewBag.ImageData = imageData;
            return View(location);
        }

        // GET: Location/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // GET: Location/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Location/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Latitude,Longitude,Name,LocationType,NorthEastLatitude,NorthEastLongitude,SouthWestLatitude,SouthWestLongitude,VegetationCover,LandUse,Forestry,SocialInfrastructure,SettlementSize,Water,Area,Height,RectangleArea")] Location location)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    location.UserId = Guid.Parse(User.Identity.GetUserId());
                    db.Locations.Add(location);
                    db.SaveChanges();
                    return RedirectToAction("Monitor", new { id = location.ID });
                }
                var errors = ModelState.Values.SelectMany(v => v.Errors);
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes");
            }
            return View(location);
        }

        // GET: Location/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }


        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditLocation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var locationToUpdate = db.Locations.Find(id);
            if (TryUpdateModel(locationToUpdate, "",
            new string[] { "VegetationCover", "LandUse", "Forestry", "Influx", "SocialInfrastructure", "SettlementSize", "Water", "Area", "Height" }))
            {
                try
                {
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(locationToUpdate);
        }


        // GET: Location/Edit/5
        public ActionResult Monitor(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }


        [HttpPost, ActionName("Monitor")]
        [ValidateAntiForgeryToken]
        public ActionResult MonitorLocation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var locationToUpdate = db.Locations.Find(id);
            if (TryUpdateModel(locationToUpdate, "",
            new string[] { "VegetationCover", "LandUse", "Forestry", "Influx", "SocialInfrastructure", "SettlementSize", "Water", "Area", "Height" }))
            {
                try
                {
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(locationToUpdate);
        }

        // GET: Location/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Location location = db.Locations.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // POST: Location/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Location location = db.Locations.Find(id);
            db.Locations.Remove(location);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
