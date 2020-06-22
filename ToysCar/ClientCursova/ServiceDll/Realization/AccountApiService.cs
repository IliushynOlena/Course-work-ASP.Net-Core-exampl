using Newtonsoft.Json;
using ServiceDll.Interfaces;
using ServiceDll.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDll.Realization
{
    public class AccountApiService : IAccountService
    {
        private readonly string _url = "https://localhost:44329/api/account";
        public Dictionary<string, string> Login(AccountModel model)
        {
            var http = (HttpWebRequest)WebRequest.Create(new Uri(_url + "/login"));
            // тип відправлення
            http.Accept = "application/json";
            // тип прийому
            http.ContentType = "application/json";
            // тип запиту на сервер
            http.Method = "POST";

            // посилаємо запит
            string parsedContent = JsonConvert.SerializeObject(model);
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] bytes = encoding.GetBytes(parsedContent);

            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            // отримаємо відповідь
            UserModel newUser = new UserModel();

            try
            {
                var response = http.GetResponse();

                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);
                string content = sr.ReadToEnd();
                var anon = new
                {
                    token = String.Empty
                };
                var tokenObj = JsonConvert.DeserializeAnonymousType(content, anon);

                var tokenStr = tokenObj.token;
                var handler = new JwtSecurityTokenHandler();
                var tokenJwtSec = handler.ReadToken(tokenStr) as JwtSecurityToken;

                Dictionary<string, string> user = new Dictionary<string, string>();

                foreach (var item in tokenJwtSec.Claims)
                {
                    if (item.Type != "Id" && item.Type != "exp")
                        user.Add(item.Type, item.Value);
                }
                return user;
            }
            catch
            {}

            return null;
        }
        public Task<Dictionary<string, string>> LoginAsync(AccountModel model)
        {
            return Task.Run(() => Login(model));
        }

        public bool Logout(string userEmail)
        {
            var http = (HttpWebRequest)WebRequest.Create(new Uri(_url + "/logout"));
            // тип відправлення
            http.Accept = "application/json";
            // тип прийому
            http.ContentType = "application/json";
            // тип запиту на сервер
            http.Method = "POST";

            // посилаємо запит
            string parsedContent = JsonConvert.SerializeObject(userEmail);
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] bytes = encoding.GetBytes(parsedContent);

            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            try
            {
                var response = http.GetResponse();
            }
            catch
            {
                return false;
            }

            return true;
        }

        public Task<bool> LogoutAsync(string userEmail)
        {
            return Task.Run(() => Logout(userEmail));
        }

        public Dictionary<string, string> Registration(UserModel model)
        {
            var http = (HttpWebRequest)WebRequest.Create(new Uri(_url + "/registration"));
            // тип відправлення
            http.Accept = "application/json";
            // тип прийому
            http.ContentType = "application/json";
            // тип запиту на сервер
            http.Method = "POST";

            // посилаємо запит
            string parsedContent = JsonConvert.SerializeObject(model);
            UTF8Encoding encoding = new UTF8Encoding();
            Byte[] bytes = encoding.GetBytes(parsedContent);

            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

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

        public Task<Dictionary<string, string>> RegistrationAsync(UserModel model)
        {
            return Task.Run(() => Registration(model));
        }
    }
}
