namespace Smart_Attendance_GPS.DTOs
{
    /// <summary>
    /// Request DTO for enrolling a user's face
    /// </summary>
    public class EnrollFaceRequestDto
    {
        /// <summary>
        /// User ID to enroll face for (string since ApplicationUser.Id is string)
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Base64 encoded face image (JPEG/PNG)
        /// </summary>
        public string FaceImage { get; set; } = string.Empty;
    }
}
