using ShopCarApi.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static ShopCarApi.ViewModels.CarComparer;

namespace ShopCarApi.ViewModels
{
   public class CarComparer : IEqualityComparer<CarVM>
    {
        

        bool IEqualityComparer<CarVM>.Equals(CarVM x, CarVM y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.Id == y.Id;
        }

        int IEqualityComparer<CarVM>.GetHashCode(CarVM obj)
        { //Check whether the object is null
            if (Object.ReferenceEquals(obj, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashProductName = obj.Id == null ? 0 : obj.Id.GetHashCode();

            //Get hash code for the Code field.
            int hashProductCode = obj.Id.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductName ^ hashProductCode;
        }
    }
        public class CarVM
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public string UniqueName { get; set; }
        public string Name { get; set; }
        public List<FNameGetViewModel> filters { get; set; }
    }
    public class CarUpdateVM
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string MainImage { get; set; }
        public List<string> AdditionalImage { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        public string UniqueName { get; set; }
        public string Name { get; set; }
        public FilterAddWithCarVM FilterAdd { get; set; }
    }
    public class CarsByFilterVM
    {
        public int Id { get; set; }     
        public decimal Price { get; set; }
        public string Image { get; set; }

        public string UniqueName { get; set; }
        public string Name { get; set; }
    }
    
    public class CarAddVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Поле не може бути пустим")]
        public DateTime Date { get; set; }
        [Required(ErrorMessage = "Поле не може бути пустим")]
        public string MainImage { get; set; }
        [Required(ErrorMessage = "Поле не може бути пустим")]
        public List<string> AdditionalImage { get; set; }
        [Required(ErrorMessage = "Поле не може бути пустим")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Поле не може бути пустим")]
        public int Count { get; set; }
        [Required(ErrorMessage = "Поле не може бути пустим")]
        public string UniqueName { get; set; }
        [Required(ErrorMessage = "Поле не може бути пустим")]    
        public string Name { get; set; }
    }
    public class CarDeleteVM
    {
        public int Id { get; set; }
    }
}
