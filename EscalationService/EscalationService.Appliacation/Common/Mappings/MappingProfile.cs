using AutoMapper;
using EscalationService.Appliacation.DTOs;
using EscalationService.Appliacation.DTOs.Criteria;
using EscalationService.Domain.Entities;
using Models.DTOs;

namespace EscalationService.Appliacation.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CommentDto, Comment>().ReverseMap();
        
        CreateMap<EscalationDto, Escalation>().ReverseMap();
        
        CreateMap<CreateCriteriaDto, Criteria>();
        CreateMap<UpdateCriteriaDto, Criteria>();
        CreateMap<EscalationDtoMessage, Escalation>().ReverseMap();
        
        CreateMap<Escalation, EscalationReminderDto>()
            .ForMember(dest => dest.ResponsibleUserIds, 
                opt => opt.MapFrom(src => src.EscalationUsers.Select(eu => eu.UserId).ToList()));
    }
}