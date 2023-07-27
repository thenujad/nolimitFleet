using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using nolimit.Data;
using nolimit.Models;

namespace nolimit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : BaseController
    {
        private readonly AppDbContext _db;

        public ReportsController(AppDbContext db, IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("generatevehiclelistreport")]
        public IActionResult GenerateVehicleListReport()
        {
            // Retrieve the vehicle data from the database
            var vehicles = GetVehicleListFromDatabase();

            // Create a new PDF document
            Document document = new Document();

            // Set up the PDF writer
            var output = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, output);

            // Open the document
            document.Open();

            // Add a title to the document
            document.Add(new Paragraph("Vehicle List Report", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD)));

            // Create a table with the appropriate number of columns
            PdfPTable table = new PdfPTable(10);
            table.WidthPercentage = 100;

            // Add table headers
            table.AddCell(new PdfPCell(new Phrase("Registration Number", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));
            table.AddCell(new PdfPCell(new Phrase("Brand", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));
            table.AddCell(new PdfPCell(new Phrase("Model", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));
            table.AddCell(new PdfPCell(new Phrase("Year", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));
            table.AddCell(new PdfPCell(new Phrase("License Plate Number", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));
            table.AddCell(new PdfPCell(new Phrase("Vehicle Type", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));
            table.AddCell(new PdfPCell(new Phrase("Fuel Type", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));
            table.AddCell(new PdfPCell(new Phrase("Current Mileage", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));
            table.AddCell(new PdfPCell(new Phrase("Owner's Name", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));
            table.AddCell(new PdfPCell(new Phrase("Insurance Provider", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));

            // Add vehicle data to the table
            foreach (var vehicle in vehicles)
            {
                table.AddCell(new PdfPCell(new Phrase(vehicle.RegistrationNumber)));
                table.AddCell(new PdfPCell(new Phrase(vehicle.Brand)));
                table.AddCell(new PdfPCell(new Phrase(vehicle.Model)));
                table.AddCell(new PdfPCell(new Phrase(vehicle.Year.ToString())));
                table.AddCell(new PdfPCell(new Phrase(vehicle.LicensePlateNumber)));
                table.AddCell(new PdfPCell(new Phrase(vehicle.VehicleType)));
                table.AddCell(new PdfPCell(new Phrase(vehicle.FuelType)));
                table.AddCell(new PdfPCell(new Phrase(vehicle.CurrentMileage.ToString())));
                table.AddCell(new PdfPCell(new Phrase(vehicle.OwnerName)));
                table.AddCell(new PdfPCell(new Phrase(vehicle.InsuranceProvider)));
            }

            // Add the table to the document
            document.Add(table);


            // Close the document
            document.Close();

            // Return the PDF file for download
            return File(output.ToArray(), "application/pdf", "VehicleListReport.pdf");
        }

        private IEnumerable<nolimit.Models.Vehicle> GetVehicleListFromDatabase()
        {
            // Retrieve the vehicle data from the database
            IEnumerable<nolimit.Models.Vehicle> vehicles = _db.Vehicles.ToList();

            return vehicles;
        }

        private class Vehicle
        {
            public string Brand { get; set; }
            public string Model { get; set; }
            public int Year { get; set; }
        }

        [HttpGet("generatefueltablereport")]
        public IActionResult GenerateFuelTableReport(string reportType)
        {
            // Retrieve the fuel data from the database based on the report type
            var fuelEntries = GetFuelEntriesFromDatabase(reportType);

            // Create a new PDF document
            Document document = new Document();

            // Set up the PDF writer
            var output = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, output);

            // Open the document
            document.Open();

            // Set up colors
            var blueColor = new BaseColor(75, 119, 190); // Define a blue color

            // Add a title to the document
            var titleFont = new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD, blueColor); // Apply blue color to the title
            var title = new Paragraph($"Fuel Table Report ({reportType})", titleFont);
            title.SpacingAfter = 20; // Add spacing after the title
            document.Add(title);

            // Create a table with the appropriate number of columns
            PdfPTable table = new PdfPTable(7);
            table.WidthPercentage = 100;

            // Set table styling
            table.DefaultCell.Padding = 8;
            table.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

            // Add table headers with blue background color
            var headerFont = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.BLACK); // Apply white color to the header text
            var headerBackgroundColor = new BaseColor(75, 119, 190); // Apply blue color to the header background
            table.AddCell(GetHeaderCell("License Plate Number", headerFont, headerBackgroundColor));
            table.AddCell(GetHeaderCell("Fuel Type", headerFont, headerBackgroundColor));
            table.AddCell(GetHeaderCell("Fuel Quantity", headerFont, headerBackgroundColor));
            table.AddCell(GetHeaderCell("Date", headerFont, headerBackgroundColor));
            table.AddCell(GetHeaderCell("Cost", headerFont, headerBackgroundColor));
            table.AddCell(GetHeaderCell("Driver First Name", headerFont, headerBackgroundColor));
            table.AddCell(GetHeaderCell("Driver Last Name", headerFont, headerBackgroundColor));

            // Add fuel data to the table with alternating row colors
            var rowFillColor1 = new BaseColor(224, 236, 255); // Light blue color for even rows
            var rowFillColor2 = new BaseColor(244, 248, 255); // Pale blue color for odd rows
            bool isEvenRow = false;
            foreach (var fuelEntry in fuelEntries)
            {
                // Set row background color based on the even/odd row flag
                var rowFillColor = isEvenRow ? rowFillColor1 : rowFillColor2;

                // Add table cells with the appropriate background color
                table.AddCell(GetDataCell(fuelEntry.LicensePlateNumber, rowFillColor));
                table.AddCell(GetDataCell(fuelEntry.FuelType, rowFillColor));
                table.AddCell(GetDataCell(fuelEntry.FuelQuantity.ToString(), rowFillColor));
                table.AddCell(GetDataCell(fuelEntry.Date.ToString(), rowFillColor));
                table.AddCell(GetDataCell(fuelEntry.Cost.ToString(), rowFillColor));
                table.AddCell(GetDataCell(fuelEntry.DriverFirstName, rowFillColor));
                table.AddCell(GetDataCell(fuelEntry.DriverLastName, rowFillColor));

                isEvenRow = !isEvenRow;
            }

            // Add the table to the document
            document.Add(table);

            // Close the document
            document.Close();

            // Return the PDF file for download
            return File(output.ToArray(), "application/pdf", $"FuelTableReport_{reportType}.pdf");
        }

        private PdfPCell GetHeaderCell(string text, Font font, BaseColor backgroundColor)
        {
            var cell = new PdfPCell(new Phrase(text, font));
            cell.BackgroundColor = backgroundColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            return cell;
        }

        private PdfPCell GetDataCell(string text, BaseColor backgroundColor)
        {
            var cell = new PdfPCell(new Phrase(text));
            cell.BackgroundColor = backgroundColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            return cell;
        }


        private IEnumerable<FuelEntry> GetFuelEntriesFromDatabase(string reportType)
        {
            DateTime startDate;
            DateTime endDate;

            // Set the start and end dates based on the report type
            if (reportType == "day")
            {
                startDate = DateTime.Today;
                endDate = startDate.AddDays(1);
            }
            else if (reportType == "week")
            {
                startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                endDate = startDate.AddDays(7);
            }
            else if (reportType == "month")
            {
                startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                endDate = startDate.AddMonths(1);
            }
            else
            {
                // Invalid report type, return an empty list
                return Enumerable.Empty<FuelEntry>();
            }

            // Retrieve the fuel data from the database and join with the Vehicles and Drivers tables
            var fuelEntries = _db.Fuel
                .Join(_db.Vehicles, fe => fe.VehicleId, v => v.Id, (fe, v) => new { FuelEntry = fe, Vehicle = v })
                .Join(_db.Drivers, entry => entry.FuelEntry.DriverId, d => d.Id, (entry, d) => new { entry.FuelEntry, entry.Vehicle, Driver = d })
                .Where(entry => entry.FuelEntry.Status == "Completed" && entry.FuelEntry.DateTime >= startDate && entry.FuelEntry.DateTime < endDate)
                .ToList();

            // Create FuelEntry objects with the required properties
            var fuelEntryList = fuelEntries.Select(entry => new FuelEntry
            {
                LicensePlateNumber = entry.Vehicle.LicensePlateNumber,
                FuelType = entry.Vehicle.FuelType,
                Date = entry.FuelEntry.DateTime,
                FuelQuantity = (decimal)entry.FuelEntry.Volume,
                Cost = entry.FuelEntry.Cost,
                DriverFirstName = entry.Driver.FirstName, // Add the DriverFirstName property
                DriverLastName = entry.Driver.LastName // Add the DriverLastName property
            });

            return fuelEntryList;
        }

        public class FuelEntry
        {
            public string LicensePlateNumber { get; set; }
            public string FuelType { get; set; }
            public decimal FuelQuantity { get; set; }
            public DateTime Date { get; set; }
            public decimal Cost { get; set; }
            public string DriverFirstName { get; set; } // New property: Driver's first name
            public string DriverLastName { get; set; } // New property: Driver's last name
            public int VehicleId { get; set; }
        }




        [HttpGet("generateallocationreport")]
        public IActionResult GenerateAllocationReport(string reportType)
        {
            // Retrieve the allocation data from the database based on the report type
            var allocationEntries = GetAllocationEntriesFromDatabase(reportType);

            // Create a new PDF document
            Document document = new Document();

            // Set up the PDF writer
            var output = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, output);

            // Open the document
            document.Open();


            // Add a title to the document
            document.Add(new Paragraph($"Allocation Report ({reportType})", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD)));

            // Create a table with the appropriate number of columns
            PdfPTable table = new PdfPTable(5); // Increase the number of columns to 5
            table.WidthPercentage = 100;

            // Add table headers
            table.AddCell(new PdfPCell(new Phrase("User Name", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));
            table.AddCell(new PdfPCell(new Phrase("Vehicle", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));
            table.AddCell(new PdfPCell(new Phrase("Driver", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));
            table.AddCell(new PdfPCell(new Phrase("Departure Date", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));
            table.AddCell(new PdfPCell(new Phrase("End Date", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));

            // Add allocation data to the table
            foreach (var allocationEntry in allocationEntries)
            {
                table.AddCell(new PdfPCell(new Phrase(allocationEntry.UserName))); // Add user name cell
                table.AddCell(new PdfPCell(new Phrase(allocationEntry.Vehicle)));
                table.AddCell(new PdfPCell(new Phrase(allocationEntry.Driver)));
                table.AddCell(new PdfPCell(new Phrase(allocationEntry.DepartureDate.ToString())));
                table.AddCell(new PdfPCell(new Phrase(allocationEntry.EndDate.ToString())));
            }

            // Add the table to the document
            document.Add(table);

            // Close the document
            document.Close();

            // Return the PDF file for download
            return File(output.ToArray(), "application/pdf", $"AllocationReport_{reportType}.pdf");
        }

        private IEnumerable<AllocationEntry> GetAllocationEntriesFromDatabase(string reportType)
        {
            DateTime startDate;
            DateTime endDate;

            // Set the start and end dates based on the report type
            if (reportType == "day")
            {
                startDate = DateTime.Today;
                endDate = startDate.AddDays(1);
            }
            else if (reportType == "week")
            {
                startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                endDate = startDate.AddDays(7);
            }
            else if (reportType == "month")
            {
                startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                endDate = startDate.AddMonths(1);
            }
            else
            {
                // Invalid report type, return an empty list
                return Enumerable.Empty<AllocationEntry>();
            }

            var allocationEntries = _db.Allocations
                .Join(_db.Vehicles, a => a.VehicleId, v => v.Id, (a, v) => new { Allocation = a, Vehicle = v })
                .Join(_db.Drivers, entry => entry.Allocation.DriverId, d => d.Id, (entry, d) => new { entry.Allocation, entry.Vehicle, Driver = d })
                .Join(_db.Users, entry => entry.Allocation.UserId, u => u.Id, (entry, u) => new { entry.Allocation, entry.Vehicle, entry.Driver, User = u })
                .Where(entry => entry.Allocation.Status == "Completed" && entry.Allocation.StartTime >= startDate && entry.Allocation.EndTime < endDate)
                .ToList();

            var allocationEntryList = allocationEntries.Select(entry => new AllocationEntry
            {
                AllocationId = entry.Allocation.Id,
                Vehicle = entry.Vehicle.LicensePlateNumber,
                Driver = entry.Driver.FirstName,
                DepartureDate = entry.Allocation.StartTime,
                EndDate = entry.Allocation.EndTime,
                UserName = entry.User.Username
            });

            return allocationEntryList;
        }

        public class AllocationEntry
        {
            public int AllocationId { get; set; }
            public string Vehicle { get; set; }
            public string Driver { get; set; }
            public DateTime DepartureDate { get; set; }
            public DateTime EndDate { get; set; }
            public string UserName { get; set; }
        }


        [HttpGet("generatedriverlistreport")]
        public IActionResult GenerateDriverListReport()
        {
            // Retrieve the driver data from the database
            var drivers = GetDriverListFromDatabase();

            // Create a new PDF document
            Document document = new Document();

            // Set up the PDF writer
            var output = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, output);

            // Open the document
            document.Open();

            // Add a title to the document
            document.Add(new Paragraph("Driver List Report", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD)));

            // Create a table with the appropriate number of columns
            PdfPTable table = new PdfPTable(5);
            table.WidthPercentage = 100;

            // Add table headers
            table.AddCell(new PdfPCell(new Phrase("First Name", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));
            table.AddCell(new PdfPCell(new Phrase("Last Name", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));
            table.AddCell(new PdfPCell(new Phrase("License Number", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));
            table.AddCell(new PdfPCell(new Phrase("Date Created", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));
            table.AddCell(new PdfPCell(new Phrase("Contact Number", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD))));

            // Add driver data to the table
            foreach (var driver in drivers)
            {
                table.AddCell(new PdfPCell(new Phrase(driver.FirstName)));
                table.AddCell(new PdfPCell(new Phrase(driver.LastName)));
                table.AddCell(new PdfPCell(new Phrase(driver.LicenseNumber)));
                table.AddCell(new PdfPCell(new Phrase(driver.DateCreated.ToString())));
                table.AddCell(new PdfPCell(new Phrase(driver.ContactNumber)));
            }

            // Add the table to the document
            document.Add(table);

            // Close the document
            document.Close();

            // Return the PDF file for download
            return File(output.ToArray(), "application/pdf", "DriverListReport.pdf");
        }


        private List<nolimit.Models.Driver> GetDriverListFromDatabase()
        {
            // Retrieve the driver data from the database
            List<nolimit.Models.Driver> drivers = _db.Drivers.ToList();

            return drivers;
        }





        private class Driver
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string LicenseNumber { get; set; }
            public DateTime DateCreated { get; set; }
            public string ContactNumber { get; set; }
        }


    }
}
