namespace Test_Assessment.Interfaces
{
    /// <summary>
    /// Interface for processing trip data.
    /// </summary>
    public interface IDataProcessor
    {
        /// <summary>
        /// Executes the full data processing workflow.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ProcessDataAsync();
    }
}
