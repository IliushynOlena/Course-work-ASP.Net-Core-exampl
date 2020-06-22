using Microsoft.Win32;
using ServiceDll.Helpers;
using ServiceDll.Models;
using ServiceDll.Realization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfClientCursova.Windows
{
    /// <summary>
    /// Interaction logic for AddProductWindow.xaml
    /// </summary>
    public partial class AddProductWindow : Window
    {
        StackPanel spCategories = new StackPanel();
        string base64Image = "";
        int filtersIdType = -1;
        int categoryIdType = -1;


        public AddProductWindow()
        {
            InitializeComponent();
            ShowCategories();
            ShowTypes();
        }
        private void ShowCategories()
        {
            CategoryApiService cService = new CategoryApiService();
            var cList = cService.GetCategories();

            CreateRbtCategories(cList);
            gbCategory.Content = spCategories;
        }

        private void CreateRbtCategories(List<CategoryModel> cList)
        {
            foreach (var item in cList)
            {
                if (item.Children.Count >  0)
                {
                    CreateRbtCategories(item.Children);
                }
                else
                {
                    //Radiobuttons
                    RadioButton rbtn = new RadioButton
                    {
                        TabIndex = item.Id,
                        Content = item.Name,
                        Margin = new Thickness(90, 5, 5, 5)
                    };
                    rbtn.Checked += rbtCategory_Checked;

                    //Add radiobutton to stackpanel
                    spCategories.Children.Add(rbtn);
                }
            }
        }

        private void ShowTypes()
        {
            //StackPanel
            StackPanel sp = new StackPanel();
            //GetFilterList
            FilterApiService fService = new FilterApiService();
            var fList = fService.GetFilters();

            foreach (var fName in fList)
            {
                if (fName.Name == "Вид")
                {
                    foreach (var fValue in fName.Children)
                    {
                        //Radiobuttons
                        RadioButton rbtn = new RadioButton
                        {
                            TabIndex = fValue.Id,
                            Content = fValue.Name,
                            Margin = new Thickness(90, 5, 5, 5)
                        };
                        rbtn.Checked += rbtFilter_Checked;

                        //Add radiobutton to stackpanel
                        sp.Children.Add(rbtn);
                    };
                }
            }
            gbTypes.Content = sp;
        }

        private void rbtFilter_Checked(object sender, RoutedEventArgs e)
        {
            var rbtn = sender as RadioButton;
            filtersIdType = rbtn.TabIndex;
        }
        private void rbtCategory_Checked(object sender, RoutedEventArgs e)
        {
            var rbtn = sender as RadioButton;
            categoryIdType = rbtn.TabIndex;
        }
        private async void BtnLoadPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) " +
              "| *.jpg; *.jpeg; *.jpe; *.jfif; *.png",
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    LogoWindow logo = new LogoWindow(this.Left, this.Top, this.Height, this.Width);
                    logo.Show();

                    await Task.Run(() =>
                    {
                        string filePath = dlg.FileName;
                        var image = System.Drawing.Image.FromFile(filePath);
                        base64Image = image.ConvertToBase64String();
                        this.Dispatcher.BeginInvoke((Action)(() => imgPhoto.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(filePath))));
                    });

                    logo.Close();
                }
                catch (Exception)
                {
                    MessageBox.Show("There was a problem downloading the file");
                }
            }
        }
        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            tbWarningName.Text = "";
            tbWarningPrice.Text = "";

            Dictionary<string, string> errorList = new Dictionary<string, string>();

            ProductApiService service = new ProductApiService();

            try
            {
                Decimal price = Convert.ToDecimal(tbPrice.Text);
            }
            catch
            {
                tbWarningPrice.Text = "Неправильне число";
                return;
            }

            if (filtersIdType == -1)
            {
                tbWarningType.Text = "Виберіть тип!";
                return;
            }

            LogoWindow logo = new LogoWindow(this.Left, this.Top, this.Height, this.Width);
            logo.Show();

            errorList = await service.CreateAsync(new ProductAddModel
            {
                Name = tbName.Text,
                Price = Convert.ToDecimal(tbPrice.Text),
                PhotoBase64 = base64Image,
                FilterIdType = filtersIdType,
                CategoryId = categoryIdType
            });

            logo.Close();

            // витягуємо помилки, якщо поля невалідні
            if (errorList != null)
            {
                foreach (var item in errorList)
                {
                    if ("name" == item.Key)
                        tbWarningName.Text = item.Value;

                    if ("price" == item.Key)
                        tbWarningPrice.Text = item.Value;
                }
            }
            // в іншому випадку - успішно
            else
            {
                this.Close();
            }
        }
    }
}
