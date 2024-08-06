using ClosedXML.Excel;
using Ecommerce_Backend.Models;

namespace Ecommerce_Backend.Services
{
    public class ExcelService
    {
        IXLWorkbook workbook;
        IXLWorksheet worksheet;

        public ExcelService() { }

        public void CreateWorkSheet(string sheetName)
        {
            // Create a new Excel file.
            this.workbook = new XLWorkbook();
            this.worksheet = workbook.Worksheets.Add(sheetName);
        }

        public void CreateHeaders(string[] headers)
        {
            int count = 0, start = 1;

            for (int i = 1; i < headers.Length+1; i++)
            {
                this.worksheet.Cell(1, start++).Value = headers[count];

                count++;
            }
        }

        public void CreateRows(ClientModel[] clients)
        {
            int count = 0, start = 1;

            for (int i = 2; i < clients.Length + 2; i++)
            {

                this.worksheet.Cell(i, start).Value = clients[count].Id;
                this.worksheet.Cell(i, start + 1).Value = clients[count].Name;

                count++;
            }
        }

        public void SaveBook(string filePath)
        {
            //string filePath = "C:\\Users\\julio\\OneDrive\\Desktop\\file.xlsx";
            workbook.SaveAs(filePath);
            Console.WriteLine("Excel file created successfully!");

        }
    }
}
