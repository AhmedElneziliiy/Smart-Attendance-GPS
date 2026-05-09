namespace Smart_Attendance_GPS.DTOs
{
    /// <summary>
    /// Response DTO for face verification operations
    /// </summary>
    public class FaceVerificationResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public FaceVerificationDataDto? Data { get; set; }
    }

    /// <summary>
    /// Data DTO containing face verification results
    /// </summary>
    public class FaceVerificationDataDto
    {
        public bool IsVerified { get; set; }
        public double Similarity { get; set; }
    }
}
