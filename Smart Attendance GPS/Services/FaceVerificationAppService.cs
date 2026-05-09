using FaceRecognition.Core.Services;
using Microsoft.EntityFrameworkCore;
using Smart_Attendance_GPS.DTOs;
using Smart_Attendance_GPS.Models.Context;
using Smart_Attendance_GPS.Services.IService;

namespace Smart_Attendance_GPS.Services
{
    public class FaceVerificationAppService : IFaceVerificationAppService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFaceVerificationService _faceService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FaceVerificationAppService> _logger;

        public FaceVerificationAppService(
            ApplicationDbContext context,
            IFaceVerificationService faceService,
            IConfiguration configuration,
            ILogger<FaceVerificationAppService> logger)
        {
            _context = context;
            _faceService = faceService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<FaceVerificationResponseDto> VerifyUserFaceAsync(string userId, string base64Image)
        {
            try
            {
                // Find user with face enrollment
                var user = await _context.Users
                    .AsNoTracking()
                    .Where(u => u.Id == userId)
                    .Select(u => new { u.FaceEmbedding, u.IsFaceVerificationEnabled })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return new FaceVerificationResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                // Check if face verification is enabled
                if (!user.IsFaceVerificationEnabled)
                {
                    return new FaceVerificationResponseDto
                    {
                        Success = false,
                        Message = "Face verification is not enabled for this user"
                    };
                }

                // Check if user has enrolled face
                if (user.FaceEmbedding == null || user.FaceEmbedding.Length == 0)
                {
                    return new FaceVerificationResponseDto
                    {
                        Success = false,
                        Message = "No face enrolled for this user. Please enroll first."
                    };
                }

                // Convert base64 face image to byte array
                byte[] faceImageBytes;
                try
                {
                    faceImageBytes = Convert.FromBase64String(base64Image);
                }
                catch (FormatException)
                {
                    return new FaceVerificationResponseDto
                    {
                        Success = false,
                        Message = "Invalid face image format"
                    };
                }

                _logger.LogInformation("Processing face verification for user {UserId}. Image size: {Size} bytes",
                    userId, faceImageBytes.Length);

                // Extract embedding from the provided face image
                var embeddingResult = await _faceService.ExtractEmbeddingAsync(faceImageBytes);

                if (!embeddingResult.Success)
                {
                    _logger.LogWarning("Face extraction failed for user {UserId}: {Error}",
                        userId, embeddingResult.ErrorMessage);
                    return new FaceVerificationResponseDto
                    {
                        Success = false,
                        Message = $"Face verification failed: {embeddingResult.ErrorMessage ?? "Unable to detect face in image"}"
                    };
                }

                // Validate exactly one face is detected
                if (embeddingResult.FaceCount != 1)
                {
                    var message = embeddingResult.FaceCount == 0
                        ? "No face detected in the photo. Please ensure your face is clearly visible."
                        : $"{embeddingResult.FaceCount} faces detected. Please ensure only your face is visible.";

                    _logger.LogWarning("Invalid face count for user {UserId}: {FaceCount}",
                        userId, embeddingResult.FaceCount);
                    return new FaceVerificationResponseDto
                    {
                        Success = false,
                        Message = message
                    };
                }

                if (embeddingResult.Embedding == null || embeddingResult.Embedding.Length == 0)
                {
                    _logger.LogError("Embedding extraction returned null for user {UserId}", userId);
                    return new FaceVerificationResponseDto
                    {
                        Success = false,
                        Message = "Failed to process face image. Please try again."
                    };
                }

                // Convert stored embedding from byte[] to float[]
                float[] storedEmbedding = new float[user.FaceEmbedding.Length / sizeof(float)];
                Buffer.BlockCopy(user.FaceEmbedding, 0, storedEmbedding, 0, user.FaceEmbedding.Length);

                // Compare embeddings to get similarity score
                var similarityResult = _faceService.CompareSimilarity(embeddingResult.Embedding, storedEmbedding);

                _logger.LogInformation("Face comparison for user {UserId}: Similarity = {Similarity}%",
                    userId, similarityResult.Similarity);

                // Get similarity threshold from configuration (default to 80.0 if not set)
                double threshold = _configuration.GetValue<double>("FaceRecognition:Thresholds:SimilarityMatch", 80.0);

                // Check similarity threshold
                bool isVerified = similarityResult.Similarity >= threshold;

                if (isVerified)
                {
                    _logger.LogInformation("Face verification successful for user {UserId} with similarity {Similarity}%",
                        userId, similarityResult.Similarity);
                }
                else
                {
                    _logger.LogWarning("Face verification failed for user {UserId}: Low similarity ({Similarity}%)",
                        userId, similarityResult.Similarity);
                }

                return new FaceVerificationResponseDto
                {
                    Success = true,
                    Message = isVerified ? "Face verified successfully" : "Face verification failed: Face does not match",
                    Data = new FaceVerificationDataDto
                    {
                        IsVerified = isVerified,
                        Similarity = similarityResult.Similarity
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during face verification for user {UserId}", userId);
                return new FaceVerificationResponseDto
                {
                    Success = false,
                    Message = "An error occurred during face verification"
                };
            }
        }
    }
}
