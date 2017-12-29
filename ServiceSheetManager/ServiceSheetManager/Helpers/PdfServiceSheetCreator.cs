using System;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using ServiceSheetManager.Models;
using PdfSharp.Pdf;
using System.Drawing;
using System.IO;
using ServiceSheetManager.Properties;
using System.Configuration;
using System.Net;
using System.Text;
using System.Linq;
using System.Collections.Generic;
//using PdfSharp.Drawing;

namespace ServiceSheetManager.Helpers
{
    public class PdfServiceSheetCreator
    {
        private Document serviceSheetDoc;
        private ServiceSheet currentSheet;
        private string COLUMN_ONE_WIDTH = "6.43cm";
        private string COLUMN_TWO_WIDTH = "9.87cm";
        private MigraDoc.DocumentObjectModel.Color headerGrey = new MigraDoc.DocumentObjectModel.Color(191, 191, 191);
        private MigraDoc.DocumentObjectModel.Color entryHeaderGrey = new MigraDoc.DocumentObjectModel.Color(242, 242, 242);
        private MigraDoc.DocumentObjectModel.Color timesheetDayGrey = new MigraDoc.DocumentObjectModel.Color(217, 217, 217);
        private MigraDoc.DocumentObjectModel.Color tableBorderColour = new MigraDoc.DocumentObjectModel.Color(191, 191, 191);
        private MigraDoc.DocumentObjectModel.Color mttPurple = new MigraDoc.DocumentObjectModel.Color(80, 77, 133);
        private MigraDoc.DocumentObjectModel.Color separatorGrey = new MigraDoc.DocumentObjectModel.Color(183, 202, 216);
        private double borderWidth = 0.5;

        private Table jobDetailsTable;
        private Table timesheetTable;
        private Table serviceReportTable;
        private Table followupTable;
        private Table signoffTable;

        private string image1 = null;
        private string image2 = null;
        private string image3 = null;
        private string image4 = null;
        private string image5 = null;
        private string engineerSignature = null;
        private string customerSignature = null;

        private bool ukServiceSheet = true;

        public PdfDocument CreatePdfSheetForSubmission(ServiceSheet serviceSubmissionSheet, bool includeImage1, bool includeImage2, bool includeImage3, bool includeImage4, bool includeImage5, bool includeCustomerSignature)
        {
            try
            {
                currentSheet = serviceSubmissionSheet;

                IdentifyUsaServiceReport();

                //RT - Load all the images from the canvas api
                LoadCanvasImages();

                serviceSheetDoc = new Document();

                DefineDocumentStyles();
                CreateSheetTitle();
                CreateJobDetailsSection();
                CreateTimesheetSection();
                CreateServiceReportSection();
                CreateFollowupSection(includeImage1, includeImage2, includeImage3, includeImage4, includeImage5);
                CreateSignoffSection(includeCustomerSignature);
                CreateFooter();
                CreateHeader();
                CreateWatermark();
                SetPageSize();

                PdfDocumentRenderer docRenderer = new PdfDocumentRenderer()
                {
                    Document = serviceSheetDoc
                };
                docRenderer.RenderDocument();

                return docRenderer.PdfDocument;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.ToString());
            }
            return null;
        }

        private void IdentifyUsaServiceReport()
        {
            //If the job number begins with US, then USA service report
            if (currentSheet.MttJobNumber.StartsWith("US"))
            {
                ukServiceSheet = false;
            }
        }

        private void LoadCanvasImages()
        {
            string image1Url = currentSheet.Image1Url;
            string image2Url = currentSheet.Image2Url;
            string image3Url = currentSheet.Image3Url;
            string image4Url = currentSheet.Image4Url;
            string image5Url = currentSheet.Image5Url;
            string engineerSignatureUrl = currentSheet.MttEngSignatureUrl;
            string customerSignatureUrl = currentSheet.CustomerSignatureUrl;
            
            image1 = CanvasImageHelpers.LoadCanvasImageForUrlPdfVersion(image1Url);
            image2 = CanvasImageHelpers.LoadCanvasImageForUrlPdfVersion(image2Url);
            image3 = CanvasImageHelpers.LoadCanvasImageForUrlPdfVersion(image3Url);
            image4 = CanvasImageHelpers.LoadCanvasImageForUrlPdfVersion(image4Url);
            image5 = CanvasImageHelpers.LoadCanvasImageForUrlPdfVersion(image5Url);

            engineerSignature = CanvasImageHelpers.LoadCanvasImageForUrlPdfVersion(engineerSignatureUrl);
            customerSignature = CanvasImageHelpers.LoadCanvasImageForUrlPdfVersion(customerSignatureUrl);

        }

        private void SetPageSize()
        {
            //RT 30/1/17 - If US sheet, then page size is letter
            foreach (Section documentSection in serviceSheetDoc.Sections)
            {
                documentSection.PageSetup = serviceSheetDoc.DefaultPageSetup.Clone();
                if (ukServiceSheet == false)
                {
                    documentSection.PageSetup.PageFormat = PageFormat.Letter;
                }
                else if (ukServiceSheet == true)
                {
                    documentSection.PageSetup.PageFormat = PageFormat.A4;
                }
                else
                {
                    Console.WriteLine("UK/US job not identified");
                }
            }

        }

        private void CreateWatermark()
        {
            //Each watermark is currently the same for all pages

            Section currentSection = (Section)serviceSheetDoc.Sections.LastObject;
            MigraDoc.DocumentObjectModel.Shapes.TextFrame tfWatermarkPrimary = currentSection.Headers.Primary.AddTextFrame();
            CreateWatermarkForSection(tfWatermarkPrimary);

            MigraDoc.DocumentObjectModel.Shapes.TextFrame tfWatermarkSecondary = currentSection.Headers.FirstPage.AddTextFrame();
            CreateWatermarkForSection(tfWatermarkSecondary);
        }

        private void CreateWatermarkForSection(MigraDoc.DocumentObjectModel.Shapes.TextFrame tfWatermark)
        {
            //Creates the text for the side watermark
            tfWatermark.Orientation = MigraDoc.DocumentObjectModel.Shapes.TextOrientation.Upward;
            Paragraph lineOne = tfWatermark.AddParagraph();

            if (ukServiceSheet == true)
            {
                AddServiceToWaterMark(lineOne, "BREAKDOWN RESPONSE", true);
                AddServiceToWaterMark(lineOne, "SCHEDULED SERVICING", true);
                AddServiceToWaterMark(lineOne, "MPEOM PROCESS", true);
                AddServiceToWaterMark(lineOne, "RETROFIT & REBUILD", true);
                AddServiceToWaterMark(lineOne, "RELOCATION & INSTALLATION", true);
                AddServiceToWaterMark(lineOne, "MACHINE TOOL CALIBRATION", false);

                Paragraph lineTwo = tfWatermark.AddParagraph();

                AddServiceToWaterMark(lineTwo, "VIBRATION ANALYSIS", true);
                AddServiceToWaterMark(lineTwo, "THERMAL ANALYSIS", true);
                AddServiceToWaterMark(lineTwo, "SPINDLE ANALYSIS", true);
                AddServiceToWaterMark(lineTwo, "RESEARCH & DEVELOPMENT", true);
                AddServiceToWaterMark(lineTwo, "CUSTOMER TRAINING", true);
                AddServiceToWaterMark(lineTwo, "CONSULTATION", true);
            }
            else if (ukServiceSheet == false)
            {
                AddServiceToWaterMark(lineOne, "MPEOM PROCESS", true);
                AddServiceToWaterMark(lineOne, "RETROFIT & REBUILD", true);
                AddServiceToWaterMark(lineOne, "RELOCATION & INSTALLATION", true);
                AddServiceToWaterMark(lineOne, "MACHINE TOOL CALIBRATION", true);
                AddServiceToWaterMark(lineOne, "VIBRATION ANALYSIS", true);
                AddServiceToWaterMark(lineOne, "THERMAL ANALYSIS", true);

                Paragraph lineTwo = tfWatermark.AddParagraph();

                AddServiceToWaterMark(lineTwo, "SPINDLE ANALYSIS", true);
                AddServiceToWaterMark(lineTwo, "RESEARCH & DEVELOPMENT", true);
                AddServiceToWaterMark(lineTwo, "CUSTOMER TRAINING", true);
                AddServiceToWaterMark(lineTwo, "CONSULTATION", false);
            }
            else
            {
                Console.WriteLine("UK/US job not identified");
            }


            tfWatermark.Left = MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Left;
            tfWatermark.Top = MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Center;
            tfWatermark.RelativeHorizontal = MigraDoc.DocumentObjectModel.Shapes.RelativeHorizontal.Page;
            tfWatermark.MarginLeft = 15;
            tfWatermark.Height = new Unit(23.5, UnitType.Centimeter);
            tfWatermark.Width = new Unit(2.5, UnitType.Centimeter);
        }

        private void AddServiceToWaterMark(Paragraph lineOne, string serviceText, Boolean includeSeparator)
        {
            FormattedText serviceFormatted = lineOne.AddFormattedText(serviceText);
            serviceFormatted.Font.Size = 11;
            serviceFormatted.Font.Name = "Impact";
            serviceFormatted.Font.Bold = false;
            serviceFormatted.Color = mttPurple;

            if (includeSeparator)
            {
                FormattedText lineSeparator = lineOne.AddFormattedText(" | ");
                lineSeparator.Font.Size = 10;
                lineSeparator.Color = separatorGrey;
            }
        }

        private void CreateHeader()
        {

            Section currentSection = (Section)serviceSheetDoc.Sections.LastObject;
            HeaderFooter header = currentSection.Headers.Primary;
            Paragraph headerPara = header.AddParagraph("Report No. " + currentSheet.SubmissionNumber);
            headerPara.Format.Alignment = ParagraphAlignment.Right;
            header.Style = "Normal";

            //Separate header for first page
            HeaderFooter firstPageHeader = currentSection.Headers.FirstPage;
            Paragraph headerParaFirstPage = firstPageHeader.AddParagraph();
            headerParaFirstPage.Format.Alignment = ParagraphAlignment.Right;

            Image imgHeaderLogo = null;
            if (ukServiceSheet == true)
            {
                imgHeaderLogo = Resources.MTTHeaderLogo;
            }
            else if (ukServiceSheet == false)
            {
                imgHeaderLogo = Resources.logo;
            }
            else
            {
                Console.WriteLine("UK/US job not identified");
            }

            string headerLogoStr = ImageToMigradocString(imgHeaderLogo);
            var headerFirstPage = headerParaFirstPage.AddImage(headerLogoStr);

            headerFirstPage.LockAspectRatio = true;
            if (ukServiceSheet)
            {
                headerFirstPage.Height = new Unit(1, UnitType.Centimeter);
            }
            else
            {
                headerFirstPage.Height = new Unit(1.3, UnitType.Centimeter);
            }
            headerFirstPage.Top = MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Top;
            headerFirstPage.Left = MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Right;

        }

        private void CreateFooter()
        {
            Section currentSection = (Section)serviceSheetDoc.Sections.LastObject;

            MigraDoc.DocumentObjectModel.Shapes.TextFrame footerTf = currentSection.Footers.Primary.AddTextFrame();
            Table footerTable = footerTf.AddTable();
            footerTf.RelativeVertical = MigraDoc.DocumentObjectModel.Shapes.RelativeVertical.Page;

            if (ukServiceSheet == true)
            {
                footerTf.MarginTop = new Unit(28.5, UnitType.Centimeter);
            }
            else if (ukServiceSheet == false)
            {
                footerTf.MarginTop = new Unit(26.7, UnitType.Centimeter);
            }
            else
            {
                Console.WriteLine("UK/US job not identified");
            }

            footerTable.AddColumn("4cm");
            footerTable.AddColumn("10cm");
            footerTable.AddColumn("4cm");

            if (ukServiceSheet == true)
            {
                footerTf.MarginTop = new Unit(28.5, UnitType.Centimeter);
            }
            else if (ukServiceSheet == false)
            {
                footerTf.MarginTop = new Unit(26.7, UnitType.Centimeter);
            }
            else
            {
                Console.WriteLine("UK/US job not identified");
            }

            if (ukServiceSheet == true)
            {
                CreateUkFooterSection(footerTable);
            }
            else if (ukServiceSheet == false)
            {
                CreateUsFooterSection(footerTable);
            }
            else
            {
                Console.WriteLine("UK/US job not identified");
            }


            currentSection.Footers.FirstPage = currentSection.Footers.Primary.Clone();

        }

        private void CreateUsFooterSection(Table footerTable)
        {
            Row footerRow1 = footerTable.AddRow();
            footerTable.Style = "footerStyle";
            Paragraph addressParagraph = footerRow1.Cells[1].AddParagraph();
            addressParagraph.AddText("Machine Tool Technology, Inc., Registered Office: 7 Burroughs,");

            Row footerRow2 = footerTable.AddRow();
            Paragraph addressPara2 = footerRow2.Cells[1].AddParagraph();
            addressPara2.AddText("Irvine, California, 92618, USA.");

            Row footerRow3 = footerTable.AddRow();
            Paragraph addressPara3 = footerRow3.Cells[1].AddParagraph();
            addressPara3.AddText("Registered in California, USA. Entity Number C3844807");

            Row footerRow4 = footerTable.AddRow();
            Paragraph addressPara4 = footerRow4.Cells[1].AddParagraph();
            addressPara4.AddText("Tel. (949) 697-9062 Web Address: www.mttinc.us.com");

            //Merge the first cell, to contain the logo
            footerRow1.Cells[0].MergeDown = 3;
            footerRow1.Cells[2].MergeDown = 3;

            Paragraph pageNumberParagraph = footerRow1.Cells[2].AddParagraph();
            pageNumberParagraph.AddPageField();
            pageNumberParagraph.AddText(" (");
            pageNumberParagraph.AddNumPagesField();
            pageNumberParagraph.AddText(")");

            footerRow1.Cells[2].Style = "Normal";

            Image imgFooterLogo = Resources.logo;

            string imageFileName = ImageToMigradocString(imgFooterLogo);
            var mttIncFooter = footerRow1.Cells[0].AddImage(imageFileName);
            mttIncFooter.Height = new Unit(1.2, UnitType.Centimeter);
        }

        private static void CreateUkFooterSection(Table footerTable)
        {
            Row footerRow1 = footerTable.AddRow();
            footerTable.Style = "footerStyle";
            Paragraph addressParagraph = footerRow1.Cells[1].AddParagraph();
            addressParagraph.AddText("Machine Tool Technologies Ltd, 1H Ribble Court, 1 Meadway");

            Row footerRow2 = footerTable.AddRow();
            Paragraph addressPara2 = footerRow2.Cells[1].AddParagraph();
            addressPara2.AddText("Shuttleworth Mead Business Park, Padiham, Lancashire, BB12 7NG, UK");

            Row footerRow3 = footerTable.AddRow();
            Paragraph addressPara3 = footerRow3.Cells[1].AddParagraph();
            addressPara3.AddText("Tel. +44 (0) 845 077 9345  Fax. +44 (0) 1282 779 615  Web Address: www.mtt.uk.com");

            //Merge the first cell, to contain the logo
            footerRow1.Cells[0].MergeDown = 2;
            footerRow1.Cells[2].MergeDown = 2;

            Paragraph pageNumberParagraph = footerRow1.Cells[2].AddParagraph();
            pageNumberParagraph.AddPageField();
            pageNumberParagraph.AddText(" (");
            pageNumberParagraph.AddNumPagesField();
            pageNumberParagraph.AddText(")");

            footerRow1.Cells[2].Style = "Normal";

            Image imgFooterLogo = Resources.MTTFooterLogo;

            string imageFileName = ImageToMigradocString(imgFooterLogo);
            footerRow1.Cells[0].AddImage(imageFileName);
        }

        private static string ImageToMigradocString(Image selectedImage)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                selectedImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Close();

                byteArray = stream.ToArray();
            }

            return "base64:" + Convert.ToBase64String(byteArray);
        }

        private void CreateSignoffSection(bool includeCustomerSignature)
        {
            Section currentSection = (Section)serviceSheetDoc.Sections.LastObject;
            signoffTable = currentSection.AddTable();

            //Setup the table columns and borders
            Column tableColumnn = signoffTable.AddColumn(COLUMN_ONE_WIDTH);
            tableColumnn = signoffTable.AddColumn(COLUMN_TWO_WIDTH);
            signoffTable.Borders.Color = tableBorderColour;
            signoffTable.Borders.Width = borderWidth;

            Row row1Title = signoffTable.AddRow();
            row1Title.Cells[0].MergeRight = 1;
            Paragraph signoffPara = row1Title.Cells[0].AddParagraph();
            signoffPara.AddFormattedText("J", "allCapsFirstLetter");
            signoffPara.AddFormattedText("OB ", "allCapsNextLetter");
            signoffPara.AddFormattedText("S", "allCapsFirstLetter");
            signoffPara.AddFormattedText("IGNOFF", "allCapsNextLetter");
            row1Title.Cells[0].Shading.Color = headerGrey;

            row1Title.KeepWith = 6;
            Row row2Title = signoffTable.AddRow();
            row2Title.Cells[0].MergeRight = 1;
            Paragraph signoffParaCertify = row2Title.Cells[0].AddParagraph();
            signoffParaCertify.AddText("I hereby certify that the service work has been carried out to my satisfaction");
            row2Title.Cells[0].Shading.Color = entryHeaderGrey;

            AddImageToSignoffTable("Customer signature", customerSignature, 1, includeCustomerSignature);
            AddLineToSignoffTable("Customer name", currentSheet.CustomerName);
            //RT 7/9/17 - Adding USA date format
            if (ukServiceSheet == true)
            {
                AddLineToSignoffTable("Date", currentSheet.DtSigned.ToString("dd/MM/yyyy"));
            }
            else
            {
                AddLineToSignoffTable("Date", currentSheet.DtSigned.ToString("MM/dd/yyyy"));
            }
            //Engineer signature is always included
            AddImageToSignoffTable("MTT engineer signature", engineerSignature, 1, true);
            AddLineToSignoffTable("MTT engineer", currentSheet.UserFirstName + " " + currentSheet.UserSurname);
        }

        private void AddImageToSignoffTable(string rowTitle, string imageBase64String, double imageHeight, bool include)
        {
            Paragraph currentParagraph;
            Row currentRow = signoffTable.AddRow();
            currentParagraph = currentRow.Cells[0].AddParagraph();
            currentParagraph.AddText(rowTitle);
            currentParagraph.Style = "sectionHeader";

            currentParagraph = currentRow.Cells[1].AddParagraph();

            if (!String.IsNullOrEmpty(imageBase64String))
            {
                MigraDoc.DocumentObjectModel.Shapes.Image imageAdded = currentParagraph.AddImage(imageBase64String);
                imageAdded.LockAspectRatio = true;
                imageAdded.Height = new Unit(imageHeight, UnitType.Centimeter);
                imageAdded.Left = MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Left;
            }
            currentParagraph.Style = "Normal";
            currentRow.Cells[0].Shading.Color = entryHeaderGrey;
        }

        private void AddLineToSignoffTable(string rowTitle, string rowData)
        {
            Paragraph currentParagraph;
            Row currentRow = signoffTable.AddRow();
            currentParagraph = currentRow.Cells[0].AddParagraph();
            currentParagraph.AddText(rowTitle);
            currentParagraph.Style = "sectionHeader";

            currentParagraph = currentRow.Cells[1].AddParagraph();
            if (String.IsNullOrEmpty(rowData))
            {
                currentParagraph.AddText("");

            }
            else
            {
                currentParagraph.AddText(rowData);
            }
            currentParagraph.Style = "Normal";
            currentRow.Cells[0].Shading.Color = entryHeaderGrey;
        }

        private void CreateFollowupSection(bool includeImage1, bool includeImage2, bool includeImage3, bool includeImage4, bool includeImage5)
        {
            Section currentSection = (Section)serviceSheetDoc.Sections.LastObject;
            followupTable = currentSection.AddTable();

            //Setup the table columns and borders
            Column tableColumnn = followupTable.AddColumn(COLUMN_ONE_WIDTH);
            tableColumnn = followupTable.AddColumn(COLUMN_TWO_WIDTH);
            followupTable.Borders.Color = tableBorderColour;
            followupTable.Borders.Width = borderWidth;

            Row row1Title = followupTable.AddRow();
            row1Title.Cells[0].MergeRight = 1;
            Paragraph followupPara = row1Title.Cells[0].AddParagraph();
            followupPara.AddFormattedText("F", "allCapsFirstLetter");
            followupPara.AddFormattedText("OLLOW-UP ", "allCapsNextLetter");
            followupPara.AddFormattedText("W", "allCapsFirstLetter");
            followupPara.AddFormattedText("ORK", "allCapsNextLetter");
            row1Title.Cells[0].Shading.Color = headerGrey;

            row1Title.KeepWith = 9;

            AddLineToFollowupTable("Additional faults found", currentSheet.AdditionalFaults);
            AddLineToFollowupTable("Parts required", currentSheet.FollowUpPartsRequired);
            //RT 18/8/16 - Quote required should be yes/no
            bool quoteRequired = currentSheet.QuoteRequired;
            if (quoteRequired)
            {
                AddLineToFollowupTable("Quote required", "Yes");
            }
            else
            {
                AddLineToFollowupTable("Quote required", "No");
            }
            //addLineToFollowupTable("Quote required", currentSheet.QuoteRequired.ToString());

            Row rowImages = followupTable.AddRow();
            rowImages.Cells[0].MergeRight = 1;
            Paragraph followupImagesPara = rowImages.Cells[0].AddParagraph();
            followupImagesPara.AddFormattedText("F", "allCapsFirstLetter");
            followupImagesPara.AddFormattedText("OLLOW-UP WORK IMAGES", "allCapsNextLetter");
            rowImages.Cells[0].Shading.Color = headerGrey;

            AddImageToFollowupTable("Image 1", image1, 4, includeImage1);
            AddImageToFollowupTable("Image 2", image2, 4, includeImage2);
            AddImageToFollowupTable("Image 3", image3, 4, includeImage3);
            AddImageToFollowupTable("Image 4", image4, 4, includeImage4);
            AddImageToFollowupTable("Image 5", image5, 4, includeImage5);

            //Add a space before the next table
            currentSection.AddParagraph();
        }

        private void AddImageToFollowupTable(string rowTitle, string imageBase64String, double imageHeight, bool includeImage)
        {
            Paragraph currentParagraph;
            Row currentRow = followupTable.AddRow();
            currentParagraph = currentRow.Cells[0].AddParagraph();
            currentParagraph.AddText(rowTitle);
            currentParagraph.Style = "sectionHeader";

            currentParagraph = currentRow.Cells[1].AddParagraph();

            if (!string.IsNullOrEmpty(imageBase64String) && includeImage)
            {
                MigraDoc.DocumentObjectModel.Shapes.Image imageAdded = currentParagraph.AddImage(imageBase64String);
                imageAdded.LockAspectRatio = true;
                imageAdded.Height = new Unit(imageHeight, UnitType.Centimeter);
                imageAdded.Left = MigraDoc.DocumentObjectModel.Shapes.ShapePosition.Left;
            }
            currentParagraph.Style = "Normal";
            currentRow.Cells[0].Shading.Color = entryHeaderGrey;
        }

        private void AddLineToFollowupTable(string rowTitle, string rowData)
        {
            Paragraph currentParagraph;
            Row currentRow = followupTable.AddRow();
            currentParagraph = currentRow.Cells[0].AddParagraph();
            currentParagraph.AddText(rowTitle);
            currentParagraph.Style = "sectionHeader";

            currentParagraph = currentRow.Cells[1].AddParagraph();
            if (String.IsNullOrEmpty(rowData))
            {
                currentParagraph.AddText("");

            }
            else
            {
                currentParagraph.AddText(rowData);
            }
            currentParagraph.Style = "Normal";
            currentRow.Cells[0].Shading.Color = entryHeaderGrey;
        }

        private void CreateServiceReportSection()
        {
            Section currentSection = (Section)serviceSheetDoc.Sections.LastObject;
            serviceReportTable = currentSection.AddTable();

            //Setup the table columns and borders
            Column tableColumnn = serviceReportTable.AddColumn(COLUMN_ONE_WIDTH);
            tableColumnn = serviceReportTable.AddColumn(COLUMN_TWO_WIDTH);
            serviceReportTable.Borders.Color = tableBorderColour;
            serviceReportTable.Borders.Width = borderWidth;

            Row row1Title = serviceReportTable.AddRow();
            row1Title.Cells[0].MergeRight = 1;
            Paragraph serviceReportPara = row1Title.Cells[0].AddParagraph();
            serviceReportPara.AddFormattedText("S", "allCapsFirstLetter");
            serviceReportPara.AddFormattedText("ERVICE ", "allCapsNextLetter");
            serviceReportPara.AddFormattedText("R", "allCapsFirstLetter");
            serviceReportPara.AddFormattedText("EPORT", "allCapsNextLetter");
            row1Title.Cells[0].Shading.Color = headerGrey;

            row1Title.KeepWith = 8;

            AddLineToServiceReportTable("Total travel time", currentSheet.JobTotalTravelTime.ToString());
            AddLineToServiceReportTable("Total time onsite", currentSheet.JobTotalTimeOnsite.ToString());
            AddLineToServiceReportTable("Total mileage", currentSheet.JobTotalMileage.ToString());
            AddLineToServiceReportTable("Total daily allowances", currentSheet.TotalDailyAllowances.ToString());
            AddLineToServiceReportTable("Total overnight allowances", currentSheet.TotalOvernightAllowances.ToString());
            //RT - If there are no barier payments, then don't include the total
            double totalBarrierPayments = currentSheet.TotalBarrierPayments;
            if (totalBarrierPayments > 0)
            {
                AddLineToServiceReportTable("Total barrier payments", currentSheet.TotalBarrierPayments.ToString());
            }
            else
            {
                //Decrease the keepwith by 1 as we are removing a row
                row1Title.KeepWith = 7;
            }
            AddLineToServiceReportTable("Job status", currentSheet.JobStatus);
            AddLineToServiceReportTable("Job report", currentSheet.FinalJobReport);

            //Add a space before the next table
            currentSection.AddParagraph();
        }

        private void AddLineToServiceReportTable(string rowTitle, string rowData)
        {
            Paragraph currentParagraph;
            Row currentRow = serviceReportTable.AddRow();
            currentParagraph = currentRow.Cells[0].AddParagraph();
            currentParagraph.AddText(rowTitle);
            currentParagraph.Style = "sectionHeader";

            currentParagraph = currentRow.Cells[1].AddParagraph();
            if (String.IsNullOrEmpty(rowData))
            {
                currentParagraph.AddText("");

            }
            else
            {
                currentParagraph.AddText(rowData);
            }
            currentParagraph.Style = "Normal";
            currentRow.Cells[0].Shading.Color = entryHeaderGrey;
        }

        private void CreateTimesheetSection()
        {
            //Go through all the timesheets and create the table for each.  
            //Title only occurs on first day
            Section currentSection = (Section)serviceSheetDoc.Sections.LastObject;
            timesheetTable = currentSection.AddTable();

            //Setup the table columns and borders
            Column tableColumnn = timesheetTable.AddColumn(COLUMN_ONE_WIDTH);
            tableColumnn = timesheetTable.AddColumn(COLUMN_TWO_WIDTH);
            timesheetTable.Borders.Color = tableBorderColour;
            timesheetTable.Borders.Width = borderWidth;

            int counter = 0;

            //RT 22/12/17 - Order the service sheets
            List<ServiceDay> orderedServiceDays = currentSheet.ServiceDays.OrderBy(s => s.DtReport).ToList();

            //foreach (ServiceDay sd in currentSheet.ServiceDays)
            foreach (ServiceDay sd in orderedServiceDays)
            {
                if (counter == 0)
                {
                    Row row1Title = timesheetTable.AddRow();
                    row1Title.Cells[0].MergeRight = 1;
                    Paragraph jobDetailsPara = row1Title.Cells[0].AddParagraph();
                    jobDetailsPara.AddFormattedText("T", "allCapsFirstLetter");
                    jobDetailsPara.AddFormattedText("IMESHEET ", "allCapsNextLetter");
                    row1Title.Cells[0].Shading.Color = headerGrey;
                }

                AddDayToTimesheet(sd, counter, ukServiceSheet);

                counter = counter + 1;
            }


            //Add a gap before the next section
            currentSection.AddParagraph();
        }

        private void AddDayToTimesheet(ServiceDay currentDay, int dayCounter, bool? UkSheet)
        {
            //Add the header of the day title

            //Day number is counter plus 1
            int dayNumber = dayCounter + 1;

            Row row1Title = timesheetTable.AddRow();
            row1Title.Cells[0].MergeRight = 1;
            Paragraph jobDetailsPara = row1Title.Cells[0].AddParagraph();
            jobDetailsPara.AddFormattedText("D", "allCapsFirstLetter");
            jobDetailsPara.AddFormattedText("AY " + dayNumber, "allCapsNextLetter");
            row1Title.Cells[0].Shading.Color = timesheetDayGrey;

            //Keep the rows together
            row1Title.KeepWith = 14;

            //RT 7/9/17 - Adding USA date format
            if (UkSheet == true)
            {
                AddLineToTimesheet("Date", currentDay.DtReport.ToString("dd/MM/yyyy"));
            }
            else
            {
                AddLineToTimesheet("Date", currentDay.DtReport.ToString("MM/dd/yyyy"));
            }

            AddLineToTimesheet("Day", currentDay.DtReport.ToString("dddd"));
            AddLineToTimesheet("Travel start time", currentDay.TravelStartTime.ToShortTimeString());
            AddLineToTimesheet("Arrival time onsite", currentDay.ArrivalOnsiteTime.ToShortTimeString());
            AddLineToTimesheet("Departure time from site", currentDay.DepartureSiteTime.ToShortTimeString());
            AddLineToTimesheet("Travel end time", currentDay.TravelEndTime.ToShortTimeString());
            AddLineToTimesheet("Mileage", currentDay.Mileage.ToString());

            AddLineToTimesheet("Daily allowance", currentDay.DailyAllowance.ToString());

            AddLineToTimesheet("Overnight allowance", currentDay.OvernightAllowance.ToString());

            //RT - If there are no barrier payments, then don't include them on the sheet
            double totalBarrierPayments = currentSheet.TotalBarrierPayments;
            if (totalBarrierPayments > 0)
            {
                AddLineToTimesheet("Barrier payment", currentDay.BarrierPayment.ToString());
            }
            else
            {
                //Decrease the keepwith by 1 as we are removing a row
                row1Title.KeepWith = 13;
            }
            AddLineToTimesheet("Total travel time", currentDay.TotalTravelTime.ToString());
            AddLineToTimesheet("Total time onsite", currentDay.TotalOnsiteTime.ToString());
            AddLineToTimesheet("Daily report", currentDay.DailyReport);
            AddLineToTimesheet("Parts supplied today", currentDay.PartsSuppliedToday);
        }

        private void CreateSheetTitle()
        {
            //Create the report header inc service sheet number
            Section newSection = serviceSheetDoc.AddSection();
            Table timesheetHeaderTable = newSection.AddTable();

            Column tableColumnn = timesheetHeaderTable.AddColumn("12.11cm");
            tableColumnn = timesheetHeaderTable.AddColumn("4.19cm");

            Row row1 = timesheetHeaderTable.AddRow();
            row1.Cells[0].AddParagraph("SERVICE REPORT AND TIMESHEET");
            row1.Cells[1].AddParagraph("No. " + currentSheet.SubmissionNumber);
            timesheetHeaderTable.Style = "timesheetHeader";

            //Add a line break  at the end of the header
            newSection.AddParagraph();
        }

        private void DefineDocumentStyles()
        {
            //Change the normal style on the document
            MigraDoc.DocumentObjectModel.Style documentStyle = serviceSheetDoc.Styles["Normal"];
            documentStyle.Font.Name = "Verdana";
            documentStyle.Font.Size = 10;

            //Create a style for the section headers
            MigraDoc.DocumentObjectModel.Style sectionHeaderStyle = serviceSheetDoc.Styles.AddStyle("sectionHeader", "Normal");
            sectionHeaderStyle.Font.Size = 10;
            sectionHeaderStyle.Font.Bold = true;

            MigraDoc.DocumentObjectModel.Style timesheetHeaderStyle = serviceSheetDoc.Styles.AddStyle("timesheetHeader", "Normal");
            timesheetHeaderStyle.Font.Size = 18;

            MigraDoc.DocumentObjectModel.Style allCapsFirstLetterStyle = serviceSheetDoc.Styles.AddStyle("allCapsFirstLetter", "Normal");
            allCapsFirstLetterStyle.Font.Size = 11;
            allCapsFirstLetterStyle.Font.Bold = true;

            MigraDoc.DocumentObjectModel.Style allCapsNextLetterStyle = serviceSheetDoc.Styles.AddStyle("allCapsNextLetter", "Normal");
            allCapsNextLetterStyle.Font.Size = 9;
            allCapsNextLetterStyle.Font.Bold = true;

            MigraDoc.DocumentObjectModel.Style footerStyle = serviceSheetDoc.Styles.AddStyle("footerStyle", "Normal");
            footerStyle.Font.Size = 6;
            footerStyle.Font.Name = "Arial";
            footerStyle.Font.Color = headerGrey;

            //Document header is different for page 1
            serviceSheetDoc.DefaultPageSetup.DifferentFirstPageHeaderFooter = true;
        }

        private void CreateJobDetailsSection()
        {
            Section currentSection = (Section)serviceSheetDoc.Sections.LastObject;
            jobDetailsTable = currentSection.AddTable();

            //Setup the table columns and borders
            Column tableColumnn = jobDetailsTable.AddColumn(COLUMN_ONE_WIDTH);
            tableColumnn = jobDetailsTable.AddColumn(COLUMN_TWO_WIDTH);
            jobDetailsTable.Borders.Color = tableBorderColour;
            jobDetailsTable.Borders.Width = borderWidth;

            Row row1Title = jobDetailsTable.AddRow();
            row1Title.Cells[0].MergeRight = 1;
            Paragraph jobDetailsPara = row1Title.Cells[0].AddParagraph();
            jobDetailsPara.AddFormattedText("J", "allCapsFirstLetter");
            jobDetailsPara.AddFormattedText("OB ", "allCapsNextLetter");
            jobDetailsPara.AddFormattedText("D", "allCapsFirstLetter");
            jobDetailsPara.AddFormattedText("ETAILS ", "allCapsNextLetter");
            row1Title.Cells[0].Shading.Color = headerGrey;

            AddLineToJobDetails("Customer", currentSheet.Customer);
            AddLineToJobDetails("Address Line 1", currentSheet.AddressLine1);
            AddLineToJobDetails("Address Line 2", currentSheet.AddressLine2);
            //RT 7/9/17 - Adding USA formatting
            if (ukServiceSheet == true)
            {
                AddLineToJobDetails("Town/City", currentSheet.TownCity);
                AddLineToJobDetails("Postcode", currentSheet.Postcode);
            }
            else
            {
                AddLineToJobDetails("City", currentSheet.TownCity);
                AddLineToJobDetails("Zip code", currentSheet.Postcode);
            }


            AddLineToJobDetails("Customer contact", currentSheet.CustomerContact);
            AddLineToJobDetails("Customer phone number", currentSheet.CustomerPhoneNo);
            AddLineToJobDetails("Machine make and model", currentSheet.MachineMakeModel);
            AddLineToJobDetails("Machine serial number", currentSheet.MachineSerial);
            AddLineToJobDetails("CNC controller", currentSheet.CncControl);
            //RT 7/9/17 - Adding USA date format
            if (ukServiceSheet == true)
            {
                AddLineToJobDetails("Job start date", currentSheet.DtJobStart.ToString("dd/MM/yyyy"));
            }
            else
            {
                AddLineToJobDetails("Job start date", currentSheet.DtJobStart.ToString("MM/dd/yyyy"));
            }

            AddLineToJobDetails("Customer order no.", currentSheet.CustomerOrderNo);
            AddLineToJobDetails("MTT job no.", currentSheet.MttJobNumber);
            AddLineToJobDetails("Job description", currentSheet.JobDescription);

            //Add a line break between the tables
            currentSection.AddParagraph();

        }

        private void AddLineToJobDetails(string rowTitle, string rowData)
        {
            Paragraph currentParagraph;
            Row currentRow = jobDetailsTable.AddRow();
            currentParagraph = currentRow.Cells[0].AddParagraph();
            currentParagraph.AddText(rowTitle);
            currentParagraph.Style = "sectionHeader";

            currentParagraph = currentRow.Cells[1].AddParagraph();
            if (String.IsNullOrEmpty(rowData))
            {
                currentParagraph.AddText("");

            }
            else
            {
                currentParagraph.AddText(rowData);
            }
            currentParagraph.Style = "Normal";
            currentRow.Cells[0].Shading.Color = entryHeaderGrey;
        }

        private void AddLineToTimesheet(string rowTitle, string rowData)
        {
            Paragraph currentParagraph;
            Row currentRow = timesheetTable.AddRow();
            currentParagraph = currentRow.Cells[0].AddParagraph();
            currentParagraph.AddText(rowTitle);
            currentParagraph.Style = "sectionHeader";

            currentParagraph = currentRow.Cells[1].AddParagraph();
            if (String.IsNullOrEmpty(rowData))
            {
                currentParagraph.AddText("");

            }
            else
            {
                currentParagraph.AddText(rowData);
            }
            currentParagraph.Style = "Normal";
            currentRow.Cells[0].Shading.Color = entryHeaderGrey;
        }
    }
}
