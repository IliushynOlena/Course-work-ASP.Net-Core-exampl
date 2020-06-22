using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ServiceDll.Interfaces;
using ServiceDll.Models;

namespace ServiceDll.Realization
{
     
    public class ProductApiService : IProductService
    {
        private string _url = "https://localhost:44329/api/product";
        public List<ProductModel> GetProducts(int categoryId, List<int> filtersId)
        {
            //Клієнт посилає запити на API
            WebClient client = new WebClient
            {
                Encoding = Encoding.UTF8
            };

            if (categoryId != -1)
            {
                string path = "?value=" + categoryId;
                _url += @"/GetByCategory/" + path;
            }
            else if (filtersId.Count != 0)
            {
                string path = "?value=" + filtersId[0];
                for (int i = 1; i < filtersId.Count; i++)
                {
                    path += "&value=" + filtersId[i];
                }
                _url += @"/GetByFilter/" + path;
            }

            //витягуємо дані з сервера по URL
            string data = client.DownloadString(_url);

            // перетворюємо string в object за допомогою json
            var list = JsonConvert.DeserializeObject<List<ProductModel>>(data);
            return list;
        }
        public Task<List<ProductModel>> GetProductsAsync(int categoryId, List<int> filtersId)
        {
            return Task.Run(() => GetProducts(categoryId, filtersId));
        }


        public Dictionary<string, string> Create(ProductAddModel product)
        {
            var http = (HttpWebRequest)WebRequest.Create(new Uri(_url));
            // тип відправлення
            http.Accept = "application/json";
            // тип прийому
            http.ContentType = "application/json";
            // тип запиту на сервер
            http.Method = "POST";

            // посилаємо запит
            string parsedContent = JsonConvert.SerializeObject(product);
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] bytes = encoding.GetBytes(parsedContent);

            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            // отримаємо відповідь
            try
            {
                var response = http.GetResponse();
            }
            catch (WebException ex)
            {
                // Помилки при валідації даних
                using (var stream = ex.Response.GetResponseStream())
                {
                    if (ex.Response != null)
                    {
                        using (var errorResponse = (HttpWebResponse)ex.Response)
                        {
                            using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                            {
                                string errorsString = reader.ReadToEnd();
                                var errorsObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(errorsString);

                                return errorsObj;
                            }
                        }
                    }
                }
            }

            return null;
        }    
        public Task<Dictionary<string, string>> CreateAsync(ProductAddModel product)
        {
            return Task.Run(() => Create(product));
        }


        public int Delete(ProductDeleteModel product)
        {
            var http = (HttpWebRequest)WebRequest.Create(new Uri(_url));
            // тип відправлення
            http.Accept = "application/json";
            // тип прийому
            http.ContentType = "application/json";
            // тип запиту на сервер
            http.Method = "DELETE";

            // посилаємо запит
            string parsedContent = JsonConvert.SerializeObject(product);
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] bytes = encoding.GetBytes(parsedContent);

            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            // отримаємо відповідь
            var response = http.GetResponse();

            var stream = response.GetResponseStream();
            var sr = new StreamReader(stream);
            var content = sr.ReadToEnd();

            return 0;
        }
        public Task<int> DeleteAsync(ProductDeleteModel product)
        {
            return Task.Run(() => Delete(product));
        }

        public ProductEditModel EditGetById(int id)
        {
            //Клієнт посилає запити на API
            WebClient client = new WebClient
            {
                Encoding = Encoding.UTF8
            };
            //витягуємо дані з сервера по URL
            string data = client.DownloadString($"{_url}/{id}");
            // перетворюємо string в object за допомогою json
            var p = JsonConvert.DeserializeObject<ProductEditModel>(data);
            return p;
        }

        public Dictionary<string, string> EditSave(ProductEditModel product)
        {
            var http = (HttpWebRequest)WebRequest.Create(new Uri(_url));
            // тип відправлення
            http.Accept = "application/json";
            // тип прийому
            http.ContentType = "application/json";
            // тип запиту на сервер
            http.Method = "PUT";

            // посилаємо запит
            string parsedContent = JsonConvert.SerializeObject(product);
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] bytes = encoding.GetBytes(parsedContent);

            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            // отримаємо відповідь
            try
            {
                var response = http.GetResponse();
            }
            catch (WebException ex)
            {
                // Помилки при валідації даних
                using (var stream = ex.Response.GetResponseStream())
                {
                    if (ex.Response != null)
                    {
                        using (var errorResponse = (HttpWebResponse)ex.Response)
                        {
                            using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                            {
                                string errorsString = reader.ReadToEnd();
                                var errorsObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(errorsString);

                                return errorsObj;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public Task<Dictionary<string, string>> EditSaveAsync(ProductEditModel product)
        {
            return Task.Run(() => EditSave(product));
        }
    }
}
