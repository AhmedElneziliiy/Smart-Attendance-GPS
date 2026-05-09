using FaceRecognition.Core.Services;
using Microsoft.EntityFrameworkCore;
using Smart_Attendance_GPS.DTOs;
using Smart_Attendance_GPS.Models.Context;
using Smart_Attendance_GPS.Services.IService;

namespace Smart_Attendance_GPS.Services
{
    public class FaceEnrollmentService : IFaceEnrollmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFaceVerificationService _faceService;
        private readonly ILogger<FaceEnrollmentService> _logger;

        public FaceEnrollmentService(
            ApplicationDbContext context,
            IFaceVerificationService faceService,
            ILogger<FaceEnrollmentService> logger)
        {
            _context = context;
            _faceService = faceService;
            _logger = logger;
        }

        public async Task<FaceEnrollmentResponseDto> EnrollUserFaceAsync(string userId, string base64Image)
        {
            try
            {
                _logger.LogInformation("Starting face enrollment for user {UserId}", userId);

                // Validate input
                if (string.IsNullOrWhiteSpace(base64Image))
                {
                    return new FaceEnrollmentResponseDto
                    {
                        Success = false,
                        Message = "No face image provided"
                    };
                }

                // Convert base64 to byte array
                byte[] photoBytes;
                try
                {
                    photoBytes = Convert.FromBase64String(base64Image);
                }
                catch (FormatException)
                {
                    return new FaceEnrollmentResponseDto
                    {
                        Success = false,
                        Message = "Invalid base64 image format"
                    };
                }

                // Validate file size (max 5MB)
                if (photoBytes.Length > 5 * 1024 * 1024)
                {
                    return new FaceEnrollmentResponseDto
                    {
                        Success = false,
                        Message = "Photo size exceeds 5MB limit"
                    };
                }

                // Extract embedding from photo using FaceRecognition.Core
                var extractResult = await _faceService.ExtractEmbeddingAsync(photoBytes);

                if (!extractResult.Success)
                {
                    _logger.LogWarning("Face extraction failed for user {UserId}: {Error}",
                        userId, extractResult.ErrorMessage);
                    return new FaceEnrollmentResponseDto
                    {
                        Success = false,
                        Message = extractResult.ErrorMessage ?? "Failed to extract face embedding"
                    };
                }

                // Validate exactly one face is detected
                if (extractResult.FaceCount != 1)
                {
                    var message = extractResult.FaceCount == 0
                        ? "No face detected in the photo. Please upload a clear frontal face photo."
                        : $"{extractResult.FaceCount} faces detected. Please upload a photo with exactly one face.";

                    _logger.LogWarning("Invalid face count for user {UserId}: {FaceCount}",
                        userId, extractResult.FaceCount);
                    return new FaceEnrollmentResponseDto
                    {
                        Success = false,
                        Message = message
                    };
                }

                // Validate embedding
                if (extractResult.Embedding == null || extractResult.Embedding.Length == 0)
                {
                    _logger.LogError("Embedding is null or empty for user {UserId}", userId);
                    return new FaceEnrollmentResponseDto
                    {
                        Success = false,
                        Message = "Failed to generate face embedding"
                    };
                }

                // Convert float[] to byte[] for storage (512 floats = 2048 bytes)
                byte[] embeddingBytes = new byte[extractResult.Embedding.Length * sizeof(float)];
                Buffer.BlockCopy(extractResult.Embedding, 0, embeddingBytes, 0, embeddingBytes.Length);

                _logger.LogInformation("Successfully extracted face embedding for user {UserId}. Embedding size: {Size} bytes",
                    userId, embeddingBytes.Length);

                // Find user and update face data
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found", userId);
                    return new FaceEnrollmentResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                // Store embedding and enrollment timestamp
                user.FaceEmbedding = embeddingBytes;
                user.FaceEnrolledAt = DateTime.UtcNow;
                user.IsFaceVerificationEnabled = true; // Enable face verification

                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully enrolled face for user {UserId}", userId);

                return new FaceEnrollmentResponseDto
                {
                    Success = true,
                    Message = "Face enrolled successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enrolling face for user {UserId}", userId);
                return new FaceEnrollmentResponseDto
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }

        public async Task<bool> DeleteUserFaceAsync(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                user.FaceEmbedding = null;
                user.FaceEnrolledAt = null;
                user.IsFaceVerificationEnabled = false;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting face for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> HasFaceEnrollmentAsync(string userId)
        {
            try
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.Id == userId)
                    .Select(u => new { u.FaceEmbedding })
                    .FirstOrDefaultAsync();

                return user != null && user.FaceEmbedding != null && user.FaceEmbedding.Length > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking face enrollment for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> ToggleFaceVerificationAsync(string userId, bool isEnabled)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found when toggling face verification", userId);
                    return false;
                }

                // If enabling, check if face is enrolled
                if (isEnabled && (user.FaceEmbedding == null || user.FaceEmbedding.Length == 0))
                {
                    _logger.LogWarning("Cannot enable face verification for user {UserId}: No face enrolled", userId);
                    return false;
                }

                user.IsFaceVerificationEnabled = isEnabled;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Face verification {Status} for user {UserId}",
                    isEnabled ? "enabled" : "disabled", userId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling face verification for user {UserId}", userId);
                return false;
            }
        }
    }
}
