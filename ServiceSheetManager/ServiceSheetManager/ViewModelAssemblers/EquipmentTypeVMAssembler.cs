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
        public static async Task<SelectList> GetAllTypes(ServiceSheetsEntities db, int? selectedItemId)
        {
            List<SelectListItem> sl = new List<SelectListItem>();

            List<EquipmentType> allTypes = await db.EquipmentTypes.Where(t => t.Deleted.HasValue == false).ToListAsync();
            foreach (var type in allTypes)
            {
                SelectListItem ss = new SelectListItem
                {
                    Text = type.Description,
                    Value = type.Id.ToString()
                };

                if (selectedItemId != null && type.Id == selectedItemId.Value)
                {
                    ss.Selected = true;
                }

                sl.Add(ss);
            }

            SelectList retval = new SelectList(sl, "Value", "Text");
            return retval;
        }

        public static SelectList GetFilterList(List<EquipmentType> allEquipmentTypes)
        {
            List<SelectListItem> retval = new List<SelectListItem>
            {
                new SelectListItem() { Text = "All", Value = "-1" }
            };

            foreach (var item in allEquipmentTypes)
            {
                retval.Add(new SelectListItem() { Text = item.Description, Value = item.Id.ToString()});
            }

            return new SelectList(retval, "Value", "Text");
        }
    }
}