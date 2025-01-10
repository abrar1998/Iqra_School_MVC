namespace SchoolProj.Models.Domain
{
    public class Downloads
    {
        public string? DID { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FilePath { get; set; }
        public string? DType { get; set; }
        public string? DCounter { get; set; }
        public string? ClassID { get; set; }
        public string? DDate { get; set; }
        public string? DownloadTypeName { get; set; } // newly added
        public int ActionType { get; set; }

    }
}
