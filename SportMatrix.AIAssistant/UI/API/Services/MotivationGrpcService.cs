namespace SportMatrix.AIAssistant.UI.API.Services;

using SportMatrix;
using global::SportMatrix.AIAssistant.Application.Interfaces;
using global::SportMatrix.AIAssistant.Extensions;
using Grpc.Core;
using Sportmatrix;

public class MotivationGrpcService : MotivationService.MotivationServiceBase
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

    public override async Task<Sportmatrix.MotivationResponse> GetMotivation(Sportmatrix.MotivationRequest request, ServerCallContext context)
    {
        try
        {
            this.logger.LogInformation(
                "gRPC: Received motivation request for athlete: {Name}",
                request.AthleteProfile?.Name ?? "Unknown");

            // Konvertiere gRPC Request zu Application DTO
            SportMatrix.AIAssistant.Application.DTOs.MotivationRequestDto motivationRequest = request.ToMotivationRequestDto();

            // Rufe den HuggingFace Service auf!
            global::SportMatrix.AIAssistant.Application.DTOs.MotivationResponseDto response = await this.motivationCoachService.GetHuggingFaceMotivationalMessageAsync(motivationRequest, context.CancellationToken);

            // Konvertiere zur?ck zu gRPC Response
            Sportmatrix.MotivationResponse grpcResponse = new Sportmatrix.MotivationResponse
            {
                MotivationalMessage = response.MotivationalMessage ?? string.Empty,
                Quote = response.Quote ?? string.Empty,
                GeneratedAt = response.GeneratedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            };

            // ActionableTips hinzuf?gen
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

            // gRPC Exception werfen
            throw new RpcException(new Status(
                StatusCode.Internal,
                $"Failed to generate motivation: {ex.Message}"));
        }
    }
}
