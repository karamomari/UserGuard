using ClosedXML.Excel;
using OfficeOpenXml;
using System.ComponentModel;
using ClosedXML.Excel;

namespace UserGuard_API.Service
{
    public class ExcelExportService : IExcelExportService
    {

        public byte[] ExportBranchDetailsToExcel(BranchDetailsDTO branchDetails)
        {
            using var workbook = new XLWorkbook();

            var branchSheet = workbook.Worksheets.Add("Branch Info");
            branchSheet.Cell("A1").Value = "Name";
            branchSheet.Cell("B1").Value = "Location";
            branchSheet.Cell("C1").Value = "Manager";
            branchSheet.Cell("A2").Value = branchDetails.Name;
            branchSheet.Cell("B2").Value = branchDetails.Location;
            branchSheet.Cell("C2").Value = branchDetails.ManagerName;

            var courseSheet = workbook.Worksheets.Add("Courses");
            courseSheet.Cell("A1").Value = "Name";
            courseSheet.Cell("B1").Value = "Description";
            courseSheet.Cell("C1").Value = "Credit Hours";
            for (int i = 0; i < branchDetails.Courses.Count; i++)
            {
                var course = branchDetails.Courses[i];
                courseSheet.Cell(i + 2, 1).Value = course.Name;
                courseSheet.Cell(i + 2, 2).Value = course.Description;
                courseSheet.Cell(i + 2, 3).Value = course.CreditHours;
            }

            var empSheet = workbook.Worksheets.Add("Employees");
            empSheet.Cell("A1").Value = "Role";
            empSheet.Cell("B1").Value = "Specialization";
            for (int i = 0; i < branchDetails.Employees.Count; i++)
            {
                var emp = branchDetails.Employees[i];
                empSheet.Cell(i + 2, 1).Value = emp.Role;
                empSheet.Cell(i + 2, 2).Value = emp.Specialization;
            }

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

    }
}
