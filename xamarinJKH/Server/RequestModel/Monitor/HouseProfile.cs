using xamarinJKH.ViewModels;

namespace xamarinJKH.Server.RequestModel
{
    public class HouseProfile:BaseViewModel
    {
        public int ID { get; set; }
        public string Address { get; set; }       
        public string Number { get; set; }
        public string UniqueNum { get; set; }
        public string DistrictId { get; set; }
        bool selected;
        public bool Selected
        {
            get => selected;
            set
            {
                selected = value;
                OnPropertyChanged("Selected");
            }
        }

    }
}