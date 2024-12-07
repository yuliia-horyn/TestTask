namespace Test_Assessment.Helpers
{
    public class PathService
    {
        public string CsvFilePath { get; private set; }
        public string DuplicatesCsvPath { get; private set; }
        public string ErrorsCsvPath { get; private set; }

        public PathService(FilePathSettings settings)
        {
            var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));

            CsvFilePath = GetAbsolutePath(basePath, settings.CsvFilePath);
            DuplicatesCsvPath = GetAbsolutePath(basePath, settings.DuplicatesCsvPath);
            ErrorsCsvPath = GetAbsolutePath(basePath, settings.ErrorCsvPath);
        }

        private string GetAbsolutePath(string basePath, string relativePath)
        {
            return Path.Combine(basePath, relativePath);
        }
    }
}
