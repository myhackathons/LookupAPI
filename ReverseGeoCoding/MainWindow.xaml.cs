using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using LookupAPI.Models;
using System.IO;

namespace ReverseGeoCoding
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string url = "http://maps.googleapis.com/maps/api/geocode/json?sensor=true&latlng=" + 37.78 + "," + -122.42;
            WebRequest request = WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var responseObject = new LookupAPI.Models.LatLongToLocation.RootObject();
            var jsonSerializer = new DataContractJsonSerializer(typeof(LookupAPI.Models.LatLongToLocation.RootObject));
            responseObject = (LookupAPI.Models.LatLongToLocation.RootObject)jsonSerializer.ReadObject(response.GetResponseStream());

            string rawJson = new StreamReader(response.GetResponseStream()).ReadToEnd();
            foreach (LookupAPI.Models.LatLongToLocation.Result item in responseObject.results)
            {
                if (item.types.Contains("country"))
                {
                    country = ((List<LookupAPI.Models.LatLongToLocation.AddressComponent>)item.address_components)[0].long_name;
                    countrycode = ((List<LookupAPI.Models.LatLongToLocation.AddressComponent>)item.address_components)[0].short_name;
                }
                if (item.types.Contains("administrative_area_level_1"))
                {
                    state = ((List<LookupAPI.Models.LatLongToLocation.AddressComponent>)item.address_components)[0].long_name;
                    statecode = ((List<LookupAPI.Models.LatLongToLocation.AddressComponent>)item.address_components)[0].short_name;
                }
                if (item.types.Contains("administrative_area_level_2"))
                {
                    city = ((List<LookupAPI.Models.LatLongToLocation.AddressComponent>)item.address_components)[0].long_name;
                }
            }
            write();

        }

        void GetPOIsCallback(IAsyncResult result)
        {
            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            HttpWebResponse response = null;
            if (request != null)
            {
                try
                {
                    response = (HttpWebResponse)request.EndGetResponse(result);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        private void write()
        {
            lbl.Content = country + " " + countrycode + " " + state + " " + statecode + " " + city;

        }
    }
}
