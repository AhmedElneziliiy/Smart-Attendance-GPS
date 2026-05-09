namespace Smart_Attendance_GPS.DTOs
{
    /// <summary>
    /// Request DTO for verifying a user's face
    /// </summary>
    public class VerifyFaceRequestDto
    {
        /// <summary>
        /// User ID to verify (string)
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Base64 encoded face image to verify
        /// </summary>
        public string FaceImage { get; set; } = string.Empty;
    }
}
