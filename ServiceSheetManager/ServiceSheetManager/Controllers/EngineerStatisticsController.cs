using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ServiceSheetManager.Models;
using ServiceSheetManager.ViewModels;
using ServiceSheetManager.ViewModels.Helpers;
using ServiceSheetManager.Models.NonDbModels;
using System.Globalization;

namespace ServiceSheetManager.Controllers
{
    public class EngineerStatisticsController : Controller
    {
        private ServiceSheetsEntities db = new ServiceSheetsEntities();

        public async Task<ActionResult> EngineerCalendar(EngineerCalendarVM weekNumberSelect)
        {
            var earliestSheetDate = await db.ServiceDays.Select(s => DbFunctions.TruncateTime(s.DtReport)).OrderBy(d => DbFunctions.TruncateTime(d.Value)).FirstAsync();
            DateTime firstSheetDate;

            if (earliestSheetDate == null)
            {
                throw new Exception("No sheet date found");
            }
            else
            {
                firstSheetDate = earliestSheetDate.Value.Date;
            }
        
            var latestSheetDate = DateTime.Now.AddDays(7);

            var weekNumbers = WeekNumberCreator.createWeekNumberDictionary(firstSheetDate, latestSheetDate);

            EngineerCalendarVM calendarVm = new EngineerCalendarVM();

            WeekNumber selectedWeek;

            if (weekNumberSelect.SelectedWeek == null)
            {
                string currentWeek = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString()
                + " - " + DateTime.Now.Year.ToString();

                if (!weekNumbers.TryGetValue(currentWeek, out selectedWeek))
                {
                    throw new Exception("Week missing from select list");
                }

                SelectList weekNumberList = new SelectList(weekNumbers, "Key", "Key", selectedWeek);
                calendarVm.WeeksList = weekNumberList;
                calendarVm.SelectedWeek = currentWeek;
            }
            else
            {
                if (!weekNumbers.TryGetValue(weekNumberSelect.SelectedWeek, out selectedWeek))
                {
                    throw new Exception("Week missing from select list");
                }
                //selectedWeek = weekNumberSelect.SelectedWeek;
                SelectList weekNumberList = new SelectList(weekNumbers, "Key", "Key", selectedWeek);
                calendarVm.WeeksList = weekNumberList;
            }



            var serviceSheets = await db.ServiceDays.Where(m => m.DtReport >= selectedWeek.StartDate)
                .Where(m => m.DtReport < selectedWeek.EndDate).Include(m => m.ServiceSheet)
                .OrderBy(m => m.DtReport).ToListAsync();

            //Create the list of days on the engineer calendar
            DateTime dateCounter = selectedWeek.StartDate;
            do
            {
                calendarVm.CalendarDays.Add(dateCounter, new EngineerCalendarDay(dateCounter));
                dateCounter = dateCounter.AddDays(1);
            } while (dateCounter < selectedWeek.EndDate);


            //Build the engineer calendar
            DateTime currentDay;

            foreach (var sheet in serviceSheets)
            {
                currentDay = sheet.DtReport;
                EngineerCalendarDay calDayFound;
                if (calendarVm.CalendarDays.TryGetValue(currentDay, out calDayFound))
                {
                    //Check if the engineer has a day assigned yet
                    EngineerCalendarDayDetails dayDetails;
                    if (calDayFound.CalendarDayDetails.TryGetValue(sheet.ServiceSheet.Username, out dayDetails))
                    {
                        dayDetails.TotalHours += sheet.TotalOnsiteTime + sheet.TotalTravelTime;
                        string customer = sheet.ServiceSheet.Customer;
                        string mttJobNumber = sheet.ServiceSheet.MttJobNumber;
                        double onsiteTime = sheet.TotalOnsiteTime;
                        double travelTime = sheet.TotalTravelTime;
                        JobDetails engJobDetails = new JobDetails(mttJobNumber, customer, onsiteTime, travelTime);
                        dayDetails.JobsList.Add(engJobDetails);
                    }
                    else
                    {
                        EngineerCalendarDayDetails dayDetailsAdd = new EngineerCalendarDayDetails();
                        dayDetailsAdd.Engineer = sheet.ServiceSheet.UserFirstName + " " + sheet.ServiceSheet.UserSurname.First();
                        dayDetailsAdd.TotalHours = sheet.TotalOnsiteTime + sheet.TotalTravelTime;
                        string customer = sheet.ServiceSheet.Customer;
                        string mttJobNumber = sheet.ServiceSheet.MttJobNumber;
                        double onsiteTime = sheet.TotalOnsiteTime;
                        double travelTime = sheet.TotalTravelTime;
                        JobDetails engJobDetails = new JobDetails(mttJobNumber, customer, onsiteTime, travelTime);
                        dayDetailsAdd.JobsList.Add(engJobDetails);
                        dayDetailsAdd.MissingDay = false;
                        dayDetailsAdd.EngineerInitials = sheet.ServiceSheet.UserFirstName.First().ToString() + sheet.ServiceSheet.UserSurname.First().ToString();
                        calDayFound.CalendarDayDetails.Add(sheet.ServiceSheet.Username, dayDetailsAdd);
                    }

                }
                else
                {
                    throw new Exception("Day not found for: " + currentDay.Date.ToString());
                }
                

            }

            //Create a list of all possible engineers
            List<string> allEngineers = await db.ServiceSheets.Select(s => s.Username).Distinct().ToListAsync();
            allEngineers.Remove("c.newton@mtt.uk.com");
            allEngineers.Remove("d.malcolm@mtt.uk.com");
            allEngineers.Remove("p.henderson@mtt.uk.com");
            allEngineers.Remove("s.lewis@mtt.uk.com");


            foreach (var calDay in calendarVm.CalendarDays.Values)
            {
                foreach (var engineer in allEngineers)
                {
                    if (!calDay.CalendarDayDetails.ContainsKey(engineer.ToString()))
                    {
                        EngineerCalendarDayDetails missingDayDetails = new EngineerCalendarDayDetails();
                        missingDayDetails.Engineer = engineer;
                        missingDayDetails.MissingDay = true;
                        missingDayDetails.TotalHours = 0;
                        calDay.CalendarDayDetails.Add(engineer.ToString(), missingDayDetails);
                    }
                }
            }
            

            return View(calendarVm);
        }
    }
}
