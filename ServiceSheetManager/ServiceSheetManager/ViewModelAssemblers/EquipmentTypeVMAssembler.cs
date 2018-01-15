using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceSheetManager.Models;
using System.Data.Entity;
using System.Threading.Tasks;

namespace ServiceSheetManager.ViewModelAssemblers
{
    public class EquipmentTypeVMAssembler
    {
        public static async Task<List<SelectListItem>> GetAllTypes(ServiceSheetsEntities db)
        {
            List<SelectListItem> retval = new List<SelectListItem>();

            List<EquipmentType> allTypes = await db.EquipmentTypes.Where(t => t.Deleted.HasValue == false).ToListAsync();
            foreach (var type in allTypes)
            {
                SelectListItem ss = new SelectListItem
                {
                    Text = type.Description,
                    Value = type.Id.ToString()
                };
                retval.Add(ss);
            }

            return retval;
        }
    }
}