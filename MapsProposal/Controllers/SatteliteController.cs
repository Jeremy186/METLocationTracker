using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Xml;
using System.Text;

namespace MapsProposal.Controllers
{
    public class Layer
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public double xMin { get; set; }
        public double xMax { get; set; }
        public double yMin { get; set; }
        public double yMax { get; set; }

        public Layer(string title, string name, double xmin, double xmax, double ymin, double ymax)
        {
            Title = title;
            Name = name;
            xMax = xmax;
            xMin = xmin;
            yMin = ymin;
            yMax = ymax;
        }
    }
    public class SatteliteController : Controller
    {
        // GET: Sattelite
        public ActionResult Layers()
        {
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
                    var rootLayerBBox = rootLayer.SelectSingleNode("BoundingBox");
                    layers.Add(new Layer(rootLayer.SelectSingleNode("Title").Value,
                        rootLayer.SelectSingleNode("Name").InnerText,
                        Convert.ToDouble(rootLayerBBox.Attributes["minx"].Value),
                        Convert.ToDouble(rootLayerBBox.Attributes["maxx"].Value),
                        Convert.ToDouble(rootLayerBBox.Attributes["miny"].Value),
                        Convert.ToDouble(rootLayerBBox.Attributes["maxy"].Value)
                        ));
                }

                var layerCollection = rootLayer.SelectNodes("Layer");
                foreach (XmlNode layer in layerCollection)
                {
                    XmlNode title = layer.SelectSingleNode("Title");
                    var subLayerCollection = layer.SelectNodes("Layer");
                    foreach (XmlNode subLayer in subLayerCollection)
                    {
                        XmlNode subLayerTitle = subLayer.SelectSingleNode("Title");

                        if (subLayer.SelectSingleNode("BoundingBox") != null)
                        {
                            var subLayerBBox = subLayer.SelectSingleNode("BoundingBox");
                            layers.Add(new Layer(subLayer.SelectSingleNode("Title").InnerText,
                                subLayer.SelectSingleNode("Name").InnerText,
                                Convert.ToDouble(subLayerBBox.Attributes["minx"].Value),
                                Convert.ToDouble(subLayerBBox.Attributes["maxx"].Value),
                                Convert.ToDouble(subLayerBBox.Attributes["miny"].Value),
                                Convert.ToDouble(subLayerBBox.Attributes["maxy"].Value)
                                ));

                        }

                    }
                }
            }

            ViewBag.Layers = layers;
            return View();
        }

        private static XmlDocument GetCapabilities()
        {
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

            return doc;
        }

        public ActionResult Image(string layerTitle)
        {
            XmlDocument doc = GetCapabilities();

            XmlNode layerName = doc.SelectSingleNode("//Name[text()='" + layerTitle + "']");
            XmlNode layer = layerName.ParentNode;
            XmlNode styleName = layer.SelectSingleNode("Style/Name");
            XmlNode bBox = layer.SelectSingleNode("LatLonBoundingBox");

            var xMin = bBox.Attributes["minx"];
            var yMin = bBox.Attributes["miny"];
            var xMax = bBox.Attributes["maxx"];
            var yMax = bBox.Attributes["maxy"];


            string requestUrl = "https://resources.giscloud.com/wms/f5dff74a4a4d330bfc38bda9ad28faa6?SERVICE=WMS&REQUEST=GetMap&WIDTH=640&HEIGHT=640&FORMAT=image/png&SRS=EPSG:4326&bbox=" + xMin.Value + "," + yMin.Value + "," + xMax.Value + "," + yMax.Value + "&layers=" + layerName.InnerText + "&styles=" + styleName.InnerText;

            var request = WebRequest.Create(requestUrl);
            var response = request.GetResponse();
            var bitmap = System.Drawing.Image.FromStream(response.GetResponseStream());
            response.Close();

            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] imageData = ms.ToArray();

            ViewBag.ImageData = imageData;

            return View();
        }
    }
}