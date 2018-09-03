using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace OMap.Web.Controllers
{
    public class MapController : Controller
    {
        [HttpGet]
        [Route("api/tile/{z}/{x}/{y}")]
        public ActionResult Tile(int z, int x, int y)
        {
            var tilesFolder = @"D:\Repos\OMap\src\build\Mapbox";

            var tilePath = $@"{tilesFolder}\{z}\{x}\{y}.png";

            if (System.IO.File.Exists(tilePath))
            {
                var imageBytes = System.IO.File.ReadAllBytes(tilePath);

                return File(imageBytes, "image/png");
            }
            else
                return HttpNotFound();


        }
    }
}
