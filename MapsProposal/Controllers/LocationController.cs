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
            //&BBOX=3237474,5039357,3243535,5045417
            string requestUrl = "http://postcards.sentinel-hub.com/v1/wms/bc5af835-ee20-4d98-bda9-b64b2fd975a8?SERVICE=WMS&REQUEST=GetMap&LAYERS=TRUE_COLOR&MAXCC=50&WIDTH=640&HEIGHT=640&gain=1&FORMAT=image/jpeg&bgcolor=00000000&transparent=1&TIME=2016-02-08";
            string bboxParams = "&BBOX=" + location.SouthWestLongitude + "," + location.NorthEastLongitude + "," + location.SouthWestLatitude + "," + location.NorthEastLatitude;
            requestUrl += bboxParams;

            var request = WebRequest.Create(requestUrl);
            var response = request.GetResponse();
            var bitmap = System.Drawing.Image.FromStream(response.GetResponseStream());
            response.Close();

            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] imageData = ms.ToArray();


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
