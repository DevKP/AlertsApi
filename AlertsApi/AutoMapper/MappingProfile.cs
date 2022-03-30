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
                .ForMember(dest => dest.StartedAt, opt => opt.MapFrom(src => src.UpdateTime))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active));

            CreateMap<IEnumerable<Alert>, AlertsResponse>()
                .ForMember(dest => dest.Alerts, opt => opt.MapFrom(src => src.ToList()));
        }
    }
}
