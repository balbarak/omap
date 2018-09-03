using OMap.App.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OMap.App.ViewModels
{
    public class TileExportViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        public string Url
        {
            get { return TileModel.Url; }
            set
            {
                TileModel.Url = value;
                NotifyPropertyChanged();
            }
        }

        public string Path
        {
            get { return TileModel.DefaultFolder; }
            set
            {
                TileModel.DefaultFolder = value;
                NotifyPropertyChanged();
            }
        }

        private bool saveAsPbf;

        public bool SaveAsPbf
        {
            get { return saveAsPbf; }
            set { saveAsPbf = value; NotifyPropertyChanged(); }
        }
        
        private int zoomLevel;

        public int ZoomLevel
        {
            get { return zoomLevel; }
            set
            {
                zoomLevel = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(NumberOfTiles));
            }
        }

        public decimal NumberOfTiles
        {
            get
            {
                return TileModel.CalculateNumberOfTiles(ZoomLevel);
            }
        }

        public int ProgressStatus { get; private set; } = 0;

        public bool IsCancelDownload { get; private set; } = false;

        public async Task DownloadTiles()
        {
            await Task.Run(() =>
            {
                var tiles = TileModel.GetTiles(this.ZoomLevel);
                var count = tiles.Count;
                int index = 1;


                Parallel.ForEach(tiles, (item, loopState) =>
                 {
                     if (IsCancelDownload)
                         loopState.Stop();

                     item.DownloadTile();

                     if (SaveAsPbf)
                         item.SaveTileAsfdb();
                     else
                         item.SaveTile();

                     ProgressStatus = (index * 100) / count;

                     NotifyPropertyChanged(nameof(ProgressStatus));

                     index++;

                 });
                
                ProgressStatus = 0;
                NotifyPropertyChanged(nameof(ProgressStatus));

                this.IsCancelDownload = false;
            });
            
        }

        public void CancelDownload()
        {
            this.IsCancelDownload = true;
        }

        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
