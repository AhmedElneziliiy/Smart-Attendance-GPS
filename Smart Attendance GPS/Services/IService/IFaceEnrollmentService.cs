using Smart_Attendance_GPS.DTOs;

namespace Smart_Attendance_GPS.Services.IService
{
    /// <summary>
    /// Service interface for managing face enrollment operations
    /// </summary>
    public interface IFaceEnrollmentService
    {
        /// <summary>
        /// Enrolls a user's face for verification
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="base64Image">Base64 encoded face image</param>
        /// <returns>Enrollment result</returns>
        Task<FaceEnrollmentResponseDto> EnrollUserFaceAsync(string userId, string base64Image);

        /// <summary>
        /// Deletes a user's enrolled face data
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> DeleteUserFaceAsync(string userId);

        /// <summary>
        /// Checks if a user has enrolled their face
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>True if face is enrolled, false otherwise</returns>
        Task<bool> HasFaceEnrollmentAsync(string userId);

        /// <summary>
        /// Enables or disables face verification for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="isEnabled">True to enable, false to disable</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> ToggleFaceVerificationAsync(string userId, bool isEnabled);
    }
}
