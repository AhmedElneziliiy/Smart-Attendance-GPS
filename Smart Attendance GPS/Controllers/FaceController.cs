using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Smart_Attendance_GPS.DTOs;
using Smart_Attendance_GPS.Services.IService;

namespace Smart_Attendance_GPS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]  // Only Admins can access face recognition endpoints
    public class FaceController : ControllerBase
    {
        private readonly IFaceEnrollmentService _enrollmentService;
        private readonly IFaceVerificationAppService _verificationService;
        private readonly ILogger<FaceController> _logger;

        public FaceController(
            IFaceEnrollmentService enrollmentService,
            IFaceVerificationAppService verificationService,
            ILogger<FaceController> logger)
        {
            _enrollmentService = enrollmentService;
            _verificationService = verificationService;
            _logger = logger;
        }

        /// <summary>
        /// Enroll a user's face for face verification
        /// </summary>
        /// <param name="request">Enrollment request with user ID and base64 image</param>
        /// <returns>Enrollment result</returns>
        [HttpPost("enroll")]
        [ProducesResponseType(typeof(FaceEnrollmentResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EnrollFace([FromBody] EnrollFaceRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return BadRequest(new { success = false, message = "Invalid user ID" });
            }

            if (string.IsNullOrWhiteSpace(request.FaceImage))
            {
                return BadRequest(new { success = false, message = "Face image is required" });
            }

            var result = await _enrollmentService.EnrollUserFaceAsync(request.UserId, request.FaceImage);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Verify a user's face against their enrolled face
        /// </summary>
        /// <param name="request">Verification request with user ID and base64 image</param>
        /// <returns>Verification result with similarity score</returns>
        [HttpPost("verify")]
        [ProducesResponseType(typeof(FaceVerificationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyFace([FromBody] VerifyFaceRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return BadRequest(new { success = false, message = "Invalid user ID" });
            }

            if (string.IsNullOrWhiteSpace(request.FaceImage))
            {
                return BadRequest(new { success = false, message = "Face image is required" });
            }

            var result = await _verificationService.VerifyUserFaceAsync(request.UserId, request.FaceImage);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Delete a user's enrolled face
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("enroll/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFace(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(new { success = false, message = "Invalid user ID" });
            }

            var result = await _enrollmentService.DeleteUserFaceAsync(userId);

            if (!result)
            {
                return NotFound(new { success = false, message = "User not found or no face enrolled" });
            }

            return Ok(new { success = true, message = "Face enrollment deleted successfully" });
        }

        /// <summary>
        /// Check if a user has enrolled their face
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Boolean indicating if face is enrolled</returns>
        [HttpGet("enroll/status/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckEnrollmentStatus(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(new { success = false, message = "Invalid user ID" });
            }

            var hasEnrollment = await _enrollmentService.HasFaceEnrollmentAsync(userId);

            return Ok(new
            {
                success = true,
                userId = userId,
                hasEnrollment = hasEnrollment
            });
        }

        /// <summary>
        /// Enable or disable face verification for a specific user
        /// </summary>
        /// <param name="request">Toggle request with user ID and enabled status</param>
        /// <returns>Result of toggle operation</returns>
        [HttpPut("verification/toggle")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ToggleFaceVerification([FromBody] ToggleFaceVerificationDto request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return BadRequest(new { success = false, message = "Invalid user ID" });
            }

            var result = await _enrollmentService.ToggleFaceVerificationAsync(request.UserId, request.IsEnabled);

            if (!result)
            {
                return BadRequest(new
                {
                    success = false,
                    message = request.IsEnabled
                        ? "Cannot enable face verification. User not found or no face enrolled."
                        : "Cannot disable face verification. User not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = $"Face verification {(request.IsEnabled ? "enabled" : "disabled")} successfully",
                userId = request.UserId,
                isEnabled = request.IsEnabled
            });
        }
    }
}
