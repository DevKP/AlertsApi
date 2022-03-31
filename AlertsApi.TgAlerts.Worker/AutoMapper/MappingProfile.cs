using AlertsApi.Domain.Entities;
using AlertsApi.TgAlerts.Worker.Models;
using AutoMapper;
using TL;
using DbMessage = AlertsApi.Domain.Entities.DbMessage;

namespace AlertsApi.TgAlerts.Worker.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Message, DbMessage>();
            CreateMap<TgAlert, Alert>()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.LocationTitle))
                .ForMember(dest => dest.UpdateTime, opt => opt.MapFrom(src => src.FetchedAt))
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active));
            //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
            //.ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.message))
            //.ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date));
        }
    }
}
