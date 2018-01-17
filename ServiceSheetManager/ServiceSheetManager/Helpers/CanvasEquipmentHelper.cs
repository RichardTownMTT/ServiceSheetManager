using ServiceSheetManager.Models;
using ServiceSheetManager.Models.NonDbModels.CanvasRawDataModels;
using ServiceSheetManager.ViewModelAssemblers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceSheetManager.Helpers
{
    public class CanvasEquipmentHelper
    {
        public static async Task<List<EquipmentLocation>> ProcessEquipmentLocationSubmissions(List<CanvasEquipmentSubmission> submissions, ServiceSheetsEntities db)
        {
            List<EquipmentLocation> retval = new List<EquipmentLocation>();

            foreach (var submission in submissions)
            {
                List<string> barcodesNotFound = new List<string>();

                //Get all the submitted data
                string username = submission.Username;
                string firstName = submission.UserFirstName;
                string surname = submission.UserSurname;
                int submissionNumber = submission.SubmissionNumber;
                DateTime scanned = DateTime.Parse(submission.DateScanned);
                string location = GetLocationFromSubmission(submission);

                if (string.IsNullOrEmpty(location))
                {
                    System.Diagnostics.Trace.TraceError("Location not set for submission " + submissionNumber);
                    return null;
                }

                int locationCode = EquipmentLocationVMAssembler.SetLocationCodeForLocation(location);

                if (locationCode == -1)
                {
                    System.Diagnostics.Trace.TraceError("Unknown return location " + location);
                    return null;
                }

                List<CanvasResponse> equipmentResponsesToProcess = LoadEquipmentResponses(submission);
                if (equipmentResponsesToProcess == null)
                {
                    return null;
                }

                foreach (var response in equipmentResponsesToProcess)
                {
                    //Check if the barcode exists
                    string barcode = response.Value;
                    Equipment equipmentFound = CheckBarcodeExistsForEquipment(barcode, db);
                    EquipmentKit equipmentKitFound = null;

                    if (equipmentFound == null)
                    {
                        equipmentKitFound = CheckBarcodeExistsForEquipmentKit(barcode, db);
                    }

                    if (equipmentFound == null && equipmentKitFound == null)
                    {
                        System.Diagnostics.Trace.TraceError("Equipment not found for barcode: " + barcode);
                        barcodesNotFound.Add(barcode);
                    }
                    else
                    {
                        EquipmentLocation locationEntity = new EquipmentLocation
                        {
                            CanvasSubmissionNumber = submissionNumber,
                            ScannedUserFirstName = firstName,
                            ScannedUserName = username,
                            ScannedUserSurname = surname,
                            DtScanned = scanned,
                            LocationCode = locationCode
                        };

                        if (equipmentFound != null)
                        {
                            locationEntity.EquipmentId = equipmentFound.Id;
                        }
                        else
                        {
                            locationEntity.EquipmentKitId = equipmentKitFound.Id;
                        }

                        retval.Add(locationEntity);
                    }
                    

                    //if (equipmentFound == null)
                    //{
                    //    System.Diagnostics.Trace.TraceError("Equipment not found for barcode: " + barcode);
                    //    barcodesNotFound.Add(barcode);
                    //    //await SendGridEmail.SendEmail("Equipment not found for barcode: " + barcode);
                    //}
                    //else
                    //{
                    //    EquipmentLocation locationEntity = new EquipmentLocation
                    //    {
                    //        CanvasSubmissionNumber = submissionNumber,
                    //        ScannedUserFirstName = firstName,
                    //        ScannedUserName = username,
                    //        ScannedUserSurname = surname,
                    //        EquipmentId = equipmentFound.Id,
                    //        DtScanned = scanned,
                    //        LocationCode = locationCode
                    //    };
                    //    retval.Add(locationEntity);
                    //}
                }

                //Email the missing barcodes
                if (barcodesNotFound.Count > 0)
                {
                    string messageBarcodes = "User: " + username + ".  Submission Number: " + submissionNumber + ". Equipment not found for barcode(s): ";
                    foreach (var code in barcodesNotFound)
                    {
                        messageBarcodes = messageBarcodes + code + ", ";
                    }
                    System.Diagnostics.Trace.TraceError("Email: " + messageBarcodes);
                    await SendGridEmail.SendEmail(messageBarcodes);
                }
            }

            return retval;
        }

        private static EquipmentKit CheckBarcodeExistsForEquipmentKit(string barcode, ServiceSheetsEntities db)
        {
            EquipmentKit match = db.EquipmentKits.Where(k => k.Barcode.Equals(barcode)).FirstOrDefault();
            return match;
        }

        private static Equipment CheckBarcodeExistsForEquipment(string barcode, ServiceSheetsEntities db)
        {
            Equipment match = db.Equipments.Where(e => e.Barcode.Equals(barcode)).FirstOrDefault();
            return match;
        }

        private static List<CanvasResponse> LoadEquipmentResponses(CanvasEquipmentSubmission submission)
        {
            //Load the equipment section and find each item checkout
            CanvasSection equipmentSection = submission.Sections.Where(s => s.Name.Equals("Equipment")).FirstOrDefault();
            if (equipmentSection == null)
            {
                System.Diagnostics.Trace.TraceError("Unable to find equipment section from xml");
                return null;
            }

            CanvasScreen equipmentScreen = equipmentSection.CanvasScreenDetail.Where(s => s.Name.Equals("Equipment")).FirstOrDefault();

            //There may be one or multiple responses.  These are handled separately.
            List<CanvasResponse> equipmentResponses = equipmentScreen.Responses;
            if (equipmentResponses == null)
            {
                System.Diagnostics.Trace.TraceError("Unable to find responses.  No barcodes submitted?");
                return null;
            }

            List<CanvasResponse> equipmentResponsesToProcess = new List<CanvasResponse>();
            //if (equipmentResponses.Response != null)
            //{
            //    equipmentResponsesToProcess.Add(equipmentResponses.Response);
            //}
            //else
            //{
            //    equipmentResponsesToProcess.AddRange(equipmentResponses.Responses);
            //}
            equipmentResponsesToProcess.AddRange(equipmentResponses);

            return equipmentResponsesToProcess;
        }

        private static string GetLocationFromSubmission(CanvasEquipmentSubmission submission)
        {
            //Load the checkout section and find the end location
            CanvasSection location = submission.Sections.Where(s => s.Name.Equals("Check out")).FirstOrDefault();
            if (location == null)
            {
                System.Diagnostics.Trace.TraceError("Unable to find location section from xml");
                return null;
            }

            CanvasScreen locationScreen = location.CanvasScreenDetail.Where(s => s.Name.Equals("Check out")).FirstOrDefault();
            if (locationScreen == null)
            {
                System.Diagnostics.Trace.TraceError("Unable to find location screen from xml");
                return null;
            }

            //Location always has one response
            CanvasResponse locationResponse = locationScreen.Responses.FirstOrDefault();
            string destination = locationResponse.Value;
            return destination;
        }
    }
}