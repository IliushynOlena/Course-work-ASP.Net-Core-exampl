using System.Collections.Generic;
using System.Linq;
using WebServerCursova.Entities;
using WebServerCursova.ViewModels;

namespace WebServerCursova.Services
{
    public class FiltersService
    {
        public static List<FilterVM> GetFilterList(EFDbContext context)
        {
            var queryName = from n in context.FilterNames
                            select n;
            var queryGroup = from g in context.FilterNameGroups
                             select g;

            //Отримуємо загальну множину значень
            var query = from n in queryName
                        join g in queryGroup on n.Id equals g.FilterNameId into ng
                        from aEmp in ng.DefaultIfEmpty()
                        select new
                        {
                            FNameId = n.Id,
                            FName = n.Name,
                            FValueId = (aEmp != null) ? aEmp.FilterValueId : 0,
                            FValue = (aEmp != null) ? aEmp.FilterValueOf.Name : null,
                        };

            //Групуємо по іменам і сортуємо по спаданю імен
            var groupNames = (from f in query
                              group f by new
                              {
                                  Id = f.FNameId,
                                  Name = f.FName
                              } into g
                              select g);
            //.OrderBy(g => g.Key.Name);

            //По групах отримуємо
            var result = from fName in groupNames
                         select
                         new FilterVM
                         {
                             Id = fName.Key.Id,
                             Name = fName.Key.Name,
                             Children = (from v in fName
                                         group v by new FilterValueVM
                                         {
                                             Id = v.FValueId,
                                             Name = v.FValue
                                         } into g
                                         select g.Key)
                                         .ToList()
                         };

            return result.ToList();
        }
    }
}
