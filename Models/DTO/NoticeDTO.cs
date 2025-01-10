namespace SchoolProj.Models.DTO
{
    public class NoticeDTO
    {
        public string? Nid { get; set; }
        //public string? NDate { get; set; }
        public string? NDate { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FilePath { get; set; }
        public string? Url { get; set; }
        public string Ncid { get; set; }
        public string NcName { get; set; }
        public string? UserName { get; set; }
        public string? ThumbNail { get; set; }
        public string? IsFile { get; set; }
        public string? ActionType { get; set; }
    }
}
