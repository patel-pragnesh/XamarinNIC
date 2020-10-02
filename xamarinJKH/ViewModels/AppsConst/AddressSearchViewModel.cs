using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using xamarinJKH.Server.RequestModel;

namespace xamarinJKH.ViewModels.AppsConst
{
    public class AddressSearchViewModel : BaseViewModel
    {
        public List<NamedValue> Districts { get; set; }
        public List<NamedValue> Streets { get; set; }
        public List<NamedValue> Quarts { get; set; }
        public List<HouseProfile> Houses { get; set; }
        public int? DistrictID;
        public int? HouseID;
        public int? PremiseID;
        public string Street;
        public Command LoadDistricts { get; set; }
        public Command LoadHouses { get; set; }
        public AddressSearchViewModel()
        {
            Districts = new List<NamedValue>();
            Streets = new List<NamedValue>();
            Quarts = new List<NamedValue>();
            Houses = new List<HouseProfile>();

            LoadDistricts = new Command(async () =>
            {
                var response = await Server.GetHouseGroups();
                foreach (var district in response.Data)
                {
                    Districts.Add(district);
                }
            });

            LoadHouses = new Command(async () =>
            {
                var response = await Server.GetHouse(string.IsNullOrEmpty(this.Street) ? null : this.Street);
                Houses.Clear();
                foreach (var house in response.Data)
                {
                    Houses.Add(house);
                }
            });

        }
    }
}
