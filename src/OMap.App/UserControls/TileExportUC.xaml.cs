using OMap.App.Models;
using OMap.App.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OMap.App.UserControls
{
    /// <summary>
    /// Interaction logic for TileExportUC.xaml
    /// </summary>
    public partial class TileExportUC : UserControl
    {
        public TileExportViewModel ViewModel { get; set; }
        public TileExportUC()
        {
            InitializeComponent();

            ViewModel = new TileExportViewModel();
            this.DataContext = ViewModel;
        }

        private async void btnTest_Click(object sender, RoutedEventArgs e)
        {
            btnTest.IsEnabled = false;

            var tiles = TileModel.GetTiles(ViewModel.ZoomLevel);

            await tiles[0].DownloadTileAsync();

            img.Source = tiles[0].Image;

            btnTest.IsEnabled = true;
        }

        private async void btnStartDownload_Click(object sender, RoutedEventArgs e)
        {
            tileProgress.Visibility = Visibility.Visible;
            btnCancel.Visibility = Visibility.Visible;
            btnStartDownload.IsEnabled = false;

            await ViewModel.DownloadTiles();

            tileProgress.Visibility = Visibility.Hidden;
            btnCancel.Visibility = Visibility.Hidden;
            btnStartDownload.IsEnabled = true;
            btnCancel.IsEnabled = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            btnCancel.IsEnabled = false;
            ViewModel.CancelDownload();
        }
    }
}
