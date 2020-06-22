using ServiceDll.Models;
using ServiceDll.Realization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfClientCursova.MVVM;
using WpfClientCursova.Windows;
using System.Windows.Media;

namespace WpfClientCursova
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string userEmail;
        static List<int> IndexesFilters = new List<int>();
        static int indexCategory = -1;

        private ObservableCollection<ProductVM> Products = new ObservableCollection<ProductVM>();

        public MainWindow(Dictionary<string, string> responseObj)
        {
            InitializeComponent();

            userEmail = responseObj["Email"];

            lblUser.Content = responseObj["Role"] + "\n";
            lblUser.Content += responseObj["FirstName"] + " " + responseObj["LastName"] + "\n";
            lblUser.Content += responseObj["Phone"];

            ShowFilters();
            ShowCategories();
            UpdateDatabase();

        }
        private async void ShowCategories()
        {
            CategoryApiService cService = new CategoryApiService();
            var categoryList = await cService.GetCategoriesAsync();
            tvCategories.ItemsSource = categoryList;
        }
        private async void ShowFilters()
        {
            FilterApiService fService = new FilterApiService();
            var filterList = await fService.GetFiltersAsync();
            tvFilters.ItemsSource = filterList;
        }
        private async void UpdateDatabase()
        {
            ProductApiService service = new ProductApiService();
            var list = await service.GetProductsAsync(indexCategory, IndexesFilters);

            Products.Clear();

            foreach (var item in list)
            {
                string hostUrl = ConfigurationManager.AppSettings["HostUrl"];
                ProductVM newProduct = new ProductVM
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    PhotoPath = $"{hostUrl}images/{item.PhotoName}"
                };
                Products.Add(newProduct);
            }
            ShowPage();

        }


        #region Pagination
        public void ShowPage(int currentPage = 1)
        {
            int countDataInPage = 6;

            // вибираємо товари на конкретній сторінці
            var productsInPage = Products
                .Skip(countDataInPage * (currentPage - 1))
                .Take(countDataInPage)
                .ToList();

            //відображаємо на екран
            lbxProducts.ItemsSource = productsInPage;

            // генеруємо кількість кнопок
            double countButtons = Products.Count / countDataInPage;
            if (Products.Count % countDataInPage != 0)
            {
                countButtons++;
            }

            // малюємо панель з кнопками
            ShowPaginationPanel(Convert.ToInt32(countButtons), currentPage);
        }
        public void ShowPaginationPanel(int countButtons, int currentPage)
        {
            int lastPage = countButtons;
            int step = -1;
            int maxButtons = 10;

            //cut max counts of buttons
            if (countButtons > maxButtons)
                countButtons = maxButtons;

            //create container
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;
            gbPages.Content = sp;

            //create childrens(buttons)
            for (int i = 0; i < countButtons; i++)
            {
                Button dynamicButton = CreateDynamicButton();

                if (lastPage <= maxButtons)
                {
                    SetButtonAsPage(dynamicButton, i + 1, currentPage);
                }
                else
                {
                    ///////////////////////////////////////////////////////////
                    //if current page is in the left
                    ///////////////////////////////////////////////////////////
                    if (currentPage <= countButtons / 2)
                    {
                        //set(...) (is almost in the end)
                        if (i == countButtons - 2 && i != 0)
                        {
                            SetButtonAsElipsis(dynamicButton);
                        }
                        else
                        {
                            //button's numerable(1-8 ... lastPage)
                            int number = (i != countButtons - 1) ? i + 1 : lastPage;
                            SetButtonAsPage(dynamicButton, number, currentPage);
                        }
                    }
                    ///////////////////////////////////////////////////////////
                    //if current page is in the middle
                    ///////////////////////////////////////////////////////////
                    else if (currentPage > countButtons / 2 && currentPage <= lastPage - countButtons / 2 + 1)
                    {
                        //set(...) (is between numbers in the middle)

                        if (i == 3 || i == countButtons - 2)
                        {
                            SetButtonAsElipsis(dynamicButton);
                        }
                        //set numbers
                        else
                        {
                            int number;

                            if (i < 3)
                            {
                                number = i + 1;
                            }
                            else if (i == countButtons - 1)
                            {
                                number = lastPage;
                            }
                            else
                            {
                                number = currentPage + step;
                                step++;
                            }

                            SetButtonAsPage(dynamicButton, number, currentPage);
                        }
                    }

                    ///////////////////////////////////////////////////////////
                    //if current page >= 12...15(in the rigth)
                    ///////////////////////////////////////////////////////////
                    else if (currentPage >= countButtons - 3)
                    {
                        //set(...) (is almost in the left)
                        if (i == 3)
                        {
                            SetButtonAsElipsis(dynamicButton);
                        }
                        //add button
                        else
                        {
                            //button's numerable(1 2 3 ... (lastPage - 12) - lastPage)
                            int number = (i < 3) ? (i + 1) : (i + lastPage - countButtons + 1);
                            SetButtonAsPage(dynamicButton, number, currentPage);
                        }
                    }
                }

                sp.Children.Add(dynamicButton);
            }
        }
        public Button CreateDynamicButton()
        {
            Button dynamicButton = new Button();

            dynamicButton.Width = 35;
            dynamicButton.Margin = new Thickness(5, 5, 0, 0);
            dynamicButton.HorizontalAlignment = HorizontalAlignment.Left;
            dynamicButton.VerticalAlignment = VerticalAlignment.Stretch;

            return dynamicButton;
        }
        public void SetButtonAsElipsis(Button btn)
        {
            btn.Content = "";
            btn.IsEnabled = false;
        }
        public void SetButtonAsPage(Button btn, int number, int currentPage)
        {
            btn.Content = number;

            Brush selectColor = Brushes.Green;
            Brush defaultColor = Brushes.White;

            btn.Background = (number == currentPage) ? selectColor : defaultColor;

            btn.Click += DynamicButton_Click;
        }
        private void DynamicButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int page = int.Parse(btn.Content.ToString());
            ShowPage(page);
        }
        #endregion

        #region CreateDeleteEdit
        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            AddProductWindow dlg = new AddProductWindow();
            dlg.ShowDialog();

            UpdateDatabase();
        }
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = lbxProducts.SelectedItem as ProductVM;
            if (selectedItem != null)
            {
                if (MessageBox.Show("Видалити продукт " + selectedItem.Name + "?", "Видалення",
                    MessageBoxButton.YesNo)
                    == MessageBoxResult.Yes)
                {
                    ProductApiService service = new ProductApiService();
                    service.Delete(new ProductDeleteModel
                    {
                        Id = selectedItem.Id
                    });
                }

                UpdateDatabase();
            }
        }
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = lbxProducts.SelectedItem as ProductVM;
            if (selectedItem != null)
            {
                int id = selectedItem.Id;
                EditProductWindow dlg = new EditProductWindow();
                dlg.IdProduct = id;
                dlg.ShowDialog();

                UpdateDatabase();
            }
        }

        #endregion

        private async void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            AccountApiService service = new AccountApiService();
            bool Logout = await service.LogoutAsync(userEmail);
            if (Logout)
            {
                LoginWindow window = new LoginWindow();
                window.Show();
                this.Close();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox ch = sender as CheckBox;
            int id = ch.TabIndex;
            IndexesFilters.Add(id);

            UpdateDatabase();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox ch = sender as CheckBox;
            int id = ch.TabIndex;
            IndexesFilters.Remove(id);

            UpdateDatabase();
        }

        private void TbCategory_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TextBlock t = sender as TextBlock;
            indexCategory = int.Parse(t.Tag.ToString());

            UpdateDatabase();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            indexCategory = -1;

            UpdateDatabase();
        }
    }
}
