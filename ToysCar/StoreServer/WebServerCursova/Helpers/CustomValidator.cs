using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace WebServerCursova.Helpers
{
    public class CustomValidator
    {
        public static IDictionary<string, string> GetErrorsByModel(
            ModelStateDictionary modelErrors)
        {
            var errors = new Dictionary<string, string>();

            var errorList = modelErrors
                       .Where(x => x.Value.Errors.Count > 0)
                       .ToDictionary(
                            k => k.Key,
                            v => v.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                       );

            foreach (var item in errorList)
            {
                string key = item.Key;
                key = char.ToLower(key[0]).ToString() + key.Substring(1);

                string value = "";
                foreach (var e in item.Value)
                {
                    value += e + "\n";
                }
                errors.Add(key, value);
            }

            return errors;
        }


    }
}
