namespace UserGuard_API.Service
{
    public interface IExcelExportService
    {

        byte[] ExportBranchDetailsToExcel(BranchDetailsDTO branchDetails);
    }
}
