namespace Smart_Attendance_GPS.DTOs
{
    /// <summary>
    /// DTO for enabling or disabling face verification for a user
    /// </summary>
    public class ToggleFaceVerificationDto
    {
        /// <summary>
        /// User ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Enable or disable face verification
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
