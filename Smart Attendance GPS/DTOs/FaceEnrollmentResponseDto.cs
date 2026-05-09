namespace Smart_Attendance_GPS.DTOs
{
    /// <summary>
    /// Response DTO for face enrollment operations
    /// </summary>
    public class FaceEnrollmentResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
