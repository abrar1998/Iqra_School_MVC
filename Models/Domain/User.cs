namespace SchoolProj.Models.Domain
{
    public class User
    {
        public long? UserID { get; set; }
        public string? UserName { get; set; }

        public string? userFullName { get; set; }
        public string? userEmail { get; set; }
        public string? OldPassword { get; set; }
        public string? UserPassword { get; set; }
        public string? userAddress { get; set; }

        public string? userPhoneNo { get; set; }
        public string? UserType { get; set; }
        public long? userTypeID { get; set; }// Determines the role of a particular user
        public string? controlId { get; set; }
        public string? userRemarks { get; set; }
        public string? userLogoPath { get; set; }
        public string? current_Session { get; set; }
        public string? sessionID { get; set; }
        public string? activation { get; set; }
        public string? dashboard { get; set; }
        public string? control1Id { get; set; }
        // For New UI
        public string? UserTypeName { get; set; }
        public string? MasterIDs { get; set; }
        public string? PageIDs { get; set; }

        public string LabIDFK { get; set; }

        public string? DoctorIDFK { get; set; }
        public string? Token { get; set; }
        public object Url { get; set; }
    }
}
