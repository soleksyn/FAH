namespace SportMatrix.AIAssistant.UI.API.Services;

using Grpc.Core;
using SportMatrix.AIAssistant.Application.DTOs;
using SportMatrix.AIAssistant.Application.Interfaces;
using SportMatrix.AIAssistant.Extensions;

public class MotivationGrpcService : Sportmatrix.MotivationService.MotivationServiceBase
{
    private readonly IMotivationCoachService motivationCoachService;
    private readonly ILogger<MotivationGrpcService> logger;

    public MotivationGrpcService(
        IMotivationCoachService motivationCoachService,
        ILogger<MotivationGrpcService> logger)
    {
        this.motivationCoachService = motivationCoachService;
        this.logger = logger;
    }

    public override async Task<Sportmatrix.MotivationResponse> GetMotivation(
        Sportmatrix.MotivationRequest request,
        ServerCallContext context)
    {
        try
        {
            this.logger.LogInformation(
                "gRPC: Received motivation request for athlete: {Name}",
                request.AthleteProfile?.Name ?? "Unknown");

            // Convert gRPC Request to Application DTO
            MotivationRequestDto motivationRequest = request.ToMotivationRequestDto();

            // Call the motivation service
            MotivationResponseDto response = await this.motivationCoachService.GenerateMotivationAsync(
                motivationRequest, context.CancellationToken);

            // Convert back to gRPC Response
            Sportmatrix.MotivationResponse grpcResponse = new Sportmatrix.MotivationResponse
            {
                MotivationalMessage = response.MotivationalMessage ?? string.Empty,
                Quote = response.Quote ?? string.Empty,
                GeneratedAt = response.GeneratedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Source = "Gemini-AI",
            };

            // Add ActionableTips
            if (response.ActionableTips != null)
            {
                grpcResponse.ActionableTips.AddRange(response.ActionableTips);
            }

            this.logger.LogInformation("gRPC: Successfully generated motivation response");
            return grpcResponse;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "gRPC: Error generating motivation");
            throw new RpcException(new Status(StatusCode.Internal, $"Failed to generate motivation: {ex.Message}"));
        }
    }
}
