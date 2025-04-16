using AutoMapper;
using CustomizableForms.Domain.DTOs;
using CustomizableForms.Domain.Entities;

namespace CustomizableForms.API;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //User
        CreateMap<UserForRegistrationDto, User>();
        CreateMap<UserForAuthenticationDto, User>();
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsActive ? "Active" : "Blocked"));
        
        //Template
        CreateMap<Template, TemplateDto>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => 
                src.TemplateTags != null 
                    ? src.TemplateTags.Select(tt => tt.Tag != null ? tt.Tag.Name : null)
                        .Where(name => !string.IsNullOrEmpty(name))
                        .ToList() 
                    : new List<string>()))
            .ForMember(dest => dest.AllowedUsers, opt => opt.MapFrom(src =>
                src.AllowedUsers != null
                    ? src.AllowedUsers.Select(ta => ta.User != null ? ta.User.Email : null)
                        .Where(name => !string.IsNullOrEmpty(name))
                        .ToList()
                    : new List<string>()))
            .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => 
                src.Likes != null ? src.Likes.Count : 0))
            .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => 
                src.Comments != null ? src.Comments.Count : 0))
            .ForMember(dest => dest.FormsCount, opt => opt.MapFrom(src => 
                src.Forms != null ? src.Forms.Count : 0));
        
        //Question
        CreateMap<Question, QuestionDto>();
        
        //Form
        CreateMap<Form, FormDto>();
        
        //Answer
        CreateMap<Answer, AnswerDto>();
        
        //Comment
        CreateMap<TemplateComment, CommentDto>();
        
        //Tag
        CreateMap<Tag, TagDto>()
            .ForMember(dest => dest.TemplatesCount, opt => opt.Ignore());
    }
}