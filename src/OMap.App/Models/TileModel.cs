using MapControl.Caching;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OMap.App.Models
{
    public class TileModel
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Zoom { get; set; }

        public byte[] ImageBytes { get; set; }

        private string FdbKey
        {
            get
            {
                return $"OpenStreetMap\\{Zoom}\\{X}\\{Y}.jpg";
            }
        }

        private static FileDbCache FileDb { get; set; } = new FileDbCache("MapData", Directory.GetCurrentDirectory());

        public ImageSource Image
        {
            get
            {
                if (ImageBytes == null)
                    return null;

                BitmapImage img = new BitmapImage();

                MemoryStream ms = new MemoryStream(ImageBytes);

                img.BeginInit();
                img.StreamSource = ms;
                img.EndInit();



                return img as ImageSource;
            }
        }

        public TileModel()
        {
            FileDb = new FileDbCache("MapData", DefaultFolder);
        }

        public TileModel(int x, int y, int zoomLevel)
        {
            this.X = x;
            this.Y = y;
            this.Zoom = zoomLevel;
        }

        public void DownloadTile()
        {
            try
            {
                var url = String.Format(TileModel.Url, Zoom, X, Y);

                var request = WebRequest.CreateHttp(url);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            memoryStream.Seek(0, SeekOrigin.Begin);

                            ImageBytes = memoryStream.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
        
        public async Task DownloadTileAsync()
        {
            try
            {
                var url = String.Format(TileModel.Url, Zoom, X, Y);

                var request = WebRequest.CreateHttp(url);

                using (var response =  await request.GetResponseAsync())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            memoryStream.Seek(0, SeekOrigin.Begin);

                            ImageBytes = memoryStream.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public void SaveTile()
        {
            if (ImageBytes == null)
                return;

            var tilePath = $"{TileModel.DefaultFolder}\\{Zoom}\\{X}\\{Y}.pbf";

            if (!Directory.Exists(TileModel.DefaultFolder))
                Directory.CreateDirectory(DefaultFolder);

            if (!Directory.Exists($"{TileModel.DefaultFolder}\\{Zoom}"))
                Directory.CreateDirectory($"{TileModel.DefaultFolder}\\{Zoom}");

            if (!Directory.Exists($"{TileModel.DefaultFolder}\\{Zoom}\\{X}"))
                Directory.CreateDirectory($"{TileModel.DefaultFolder}\\{Zoom}\\{X}");

            File.WriteAllBytes(tilePath, ImageBytes);

        }

        public void SaveTileAsfdb()
        {
            if (ImageBytes == null)
                return;

            
            
            FileDb.Set(FdbKey, ImageBytes);
        }
        
        public static TileModel CreateTile(double lon, double lat, int zoom)
        {
            TileModel result = new TileModel();

            result.X = (int)((lon + 180.0) / 360.0 * (1 << zoom));

            result.Y = (int)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) +
                1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));

            result.Zoom = zoom;

            return result;
        }

        public static decimal CalculateNumberOfTiles(int zoomLevel)
        {
            decimal result = 0;

            int powerValue = 2 * zoomLevel;

            result = (decimal)Math.Pow(2, powerValue);

            return result;
        }

        public static string Url { get; set; } = "http://tile.openstreetmap.org/{0}/{1}/{2}.png";

        public static string DefaultFolder { get; set; } = $"{Directory.GetCurrentDirectory()}";

        public static List<TileModel> GetTiles(int zoomLevel)
        {
            List<TileModel> result = new List<TileModel>();

            decimal numberOfTiles = CalculateNumberOfTiles(zoomLevel);

            int xMax = (int)Math.Pow(2, zoomLevel);
            int yMax = (int)Math.Pow(2, zoomLevel);

            for (int x = 0; x < xMax; x++)
            {
                for (int y = 0; y < yMax; y++)
                {
                    result.Add(new TileModel(x, y, zoomLevel));
                }
            }

            return result;
        }
    }
}
