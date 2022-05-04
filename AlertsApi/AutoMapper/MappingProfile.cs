using AlertsApi.Api.Models.Responses;
using AlertsApi.Domain.Entities;
using AutoMapper;

namespace AlertsApi.Api.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Alert, AlertResponse>()
                .ForMember(dest => dest.LocationTitle, opt => opt.MapFrom(src => src.LocationName))
                .ForMember(dest => dest.StartedAt, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndedAt, opt => opt.MapFrom(src => src.EndTime))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(srs => DurationMappingFunction(srs)))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active));


            CreateMap<IEnumerable<Alert>, AlertsResponse>()
                .ForMember(dest => dest.Alerts, opt => opt.MapFrom(src => src.ToList()));
        }

        private static TimeSpan? DurationMappingFunction(Alert alert)
        {
            if (alert.StartTime is null)
                return null;

            if (alert.EndTime is null)
                return DateTime.UtcNow - alert.StartTime;

            return alert.EndTime - alert.StartTime;
        }
    }
}
