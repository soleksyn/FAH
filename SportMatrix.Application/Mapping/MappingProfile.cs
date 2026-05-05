using AutoMapper;
using SportMatrix.Application.DTOs;
using SportMatrix.Domain.Entities;
using SportMatrix.Domain.ValueObjects;

namespace SportMatrix.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Athlete mappings
        this.CreateMap<Athlete, AthleteDto>();
        this.CreateMap<CreateAthleteDto, Athlete>();
        this.CreateMap<UpdateAthleteDto, Athlete>();
        this.CreateMap<Athlete, Athlete>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // Activity mappings
        this.CreateMap<Activity, ActivityDto>()
            .ForMember(dest => dest.MovingTime, opt => opt.MapFrom(src => TimeSpan.FromSeconds(src.MovingTime)))
            .ForMember(dest => dest.ElapsedTime, opt => opt.MapFrom(src => TimeSpan.FromSeconds(src.ElapsedTime)))
            .ForMember(dest => dest.Distance, opt => opt.MapFrom(src => src.Distance / 1000.0)) // Convert meters to kilometers
            .ForMember(dest => dest.AthleteFullName, opt => opt.MapFrom(src =>
            src.Athlete != null ? $"{src.Athlete.FirstName} {src.Athlete.LastName}" : string.Empty));

        this.CreateMap<CreateActivityDto, Activity>()
            .ForMember(dest => dest.MovingTime, opt => opt.MapFrom(src => src.MovingTimeSeconds))
            .ForMember(dest => dest.ElapsedTime, opt => opt.MapFrom(src => src.ElapsedTimeSeconds))
            .ForMember(dest => dest.Distance, opt => opt.MapFrom(src => src.Distance * 1000.0)) // Convert kilometers to meters

            // Pace as calculated value from Distance and MovingTime
            .ForMember(dest => dest.Pace, opt => opt.MapFrom(src =>
                this.CreatePaceFromDistanceAndTime(src.Distance, src.MovingTimeSeconds)));

        this.CreateMap<UpdateActivityDto, Activity>()
            .ForMember(dest => dest.MovingTime, opt => opt.MapFrom(src => src.MovingTimeSeconds))
            .ForMember(dest => dest.ElapsedTime, opt => opt.MapFrom(src => src.ElapsedTimeSeconds))
            .ForMember(dest => dest.Distance, opt => opt.MapFrom(src => src.Distance * 1000.0)); // Convert kilometers to meters

        this.CreateMap<Activity, Activity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AthleteId, opt => opt.Ignore()) // Set separately
            .ForMember(dest => dest.Athlete, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => (string)null)); // Explicitly set null

        // Training plan mappings
        this.CreateMap<TrainingPlan, TrainingPlanDto>()
            .ForMember(dest => dest.AthleteName, opt => opt.MapFrom(src => src.Athlete != null ? $"{src.Athlete.FirstName} {src.Athlete.LastName}" : string.Empty));
        this.CreateMap<CreateTrainingPlanDto, TrainingPlan>();
        this.CreateMap<UpdateTrainingPlanDto, TrainingPlan>();

        // Planned activity mappings
        this.CreateMap<PlannedActivity, PlannedActivityDto>()
            .ForMember(dest => dest.PlannedDuration, opt =>
                opt.MapFrom(src => src.PlannedDuration.HasValue ? TimeSpan.FromMinutes(src.PlannedDuration.Value) : (TimeSpan?)null))
            .ForMember(dest => dest.PlannedDistance, opt => opt.MapFrom(src => src.PlannedDistance.HasValue ? src.PlannedDistance.Value / 1000.0 : (double?)null)); // Convert meters to kilometers

        this.CreateMap<CreatePlannedActivityDto, PlannedActivity>()
            .ForMember(dest => dest.PlannedDuration, opt =>
                opt.MapFrom(src => src.PlannedDurationMinutes))
            .ForMember(dest => dest.PlannedDistance, opt => opt.MapFrom(src => src.PlannedDistance.HasValue ? src.PlannedDistance.Value * 1000.0 : (double?)null)); // Convert kilometers to meters

        this.CreateMap<UpdatePlannedActivityDto, PlannedActivity>()
            .ForMember(dest => dest.PlannedDuration, opt =>
                opt.MapFrom(src => src.PlannedDurationMinutes))
            .ForMember(dest => dest.PlannedDistance, opt => opt.MapFrom(src => src.PlannedDistance.HasValue ? src.PlannedDistance.Value * 1000.0 : (double?)null)); // Convert kilometers to meters
    }

    private Pace CreatePaceFromDistanceAndTime(double distanceInKm, int movingTimeSeconds)
    {
        if (distanceInKm <= 0 || movingTimeSeconds <= 0)
        {
            return null; // or a default pace
        }

        // Calculate seconds per kilometer
        double secondsPerKm = movingTimeSeconds / distanceInKm;

        // Create Pace value object
        return new Pace(TimeSpan.FromSeconds(secondsPerKm));
    }
}
