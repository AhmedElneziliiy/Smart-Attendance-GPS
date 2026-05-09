using Smart_Attendance_GPS.DTOs;

namespace Smart_Attendance_GPS.Services.IService
{
    /// <summary>
    /// Service interface for face verification operations
    /// </summary>
    public interface IFaceVerificationAppService
    {
        /// <summary>
        /// Verifies a user's face against their enrolled face
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="base64Image">Base64 encoded face image to verify</param>
        /// <returns>Verification result with similarity score</returns>
        Task<FaceVerificationResponseDto> VerifyUserFaceAsync(string userId, string base64Image);
    }
}
