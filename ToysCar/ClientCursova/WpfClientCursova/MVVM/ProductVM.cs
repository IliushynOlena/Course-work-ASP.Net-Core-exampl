using System.ComponentModel;

namespace WpfClientCursova.MVVM
{
    public class ProductVM : INotifyPropertyChanged
    {
        public int Id { get; set; }
        private string _name;
        private decimal _price;
        private string _photoPath;

        public string Name
        {
            get { return this._name; }
            set
            {
                if (this._name != value)
                {
                    this._name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }
        public decimal Price
        {
            get { return this._price; }
            set
            {
                if (this._price != value)
                {
                    this._price = value;
                    NotifyPropertyChanged("Price");
                }
            }
        }
        public string PhotoPath
        {
            get { return this._photoPath; }
            set
            {
                if (this._photoPath != value)
                {
                    this._photoPath = value;
                    NotifyPropertyChanged("PhotoPath");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
