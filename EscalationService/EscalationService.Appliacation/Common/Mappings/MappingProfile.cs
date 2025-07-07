using AutoMapper;
using EscalationService.Appliacation.DTOs;
using EscalationService.Appliacation.DTOs.Criteria;
using EscalationService.Domain.Entities;

namespace EscalationService.Appliacation.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CommentDto, Comment>().ReverseMap();
        
        CreateMap<EscalationDto, Escalation>().ReverseMap();
        
        CreateMap<CreateCriteriaDto, Criteria>();
        CreateMap<UpdateCriteriaDto, Criteria>();
        CreateMap<Escalation, Models.DTOs.EscalationDto>().ReverseMap();
    }
}