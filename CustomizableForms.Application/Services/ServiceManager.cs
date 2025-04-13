using AutoMapper;
using Contracts.IRepositories;
using Contracts.IServices;
using CustomizableForms.Domain.ConfigurationModels;
using CustomizableForms.LoggerService;
using CustomizableForms.Persistance;
using CustomizableForms.Persistance.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace CustomizableForms.Application.Services;

public class ServiceManager(
    IRepositoryManager repositoryManager,
    ILoggerManager logger,
    IMapper mapper,
    IOptions<JwtConfiguration> configuration,
    IHubContext<CommentsHub> hubContext)
    : IServiceManager
{
    private readonly Lazy<IAuthenticationService> _authenticationService = new(() => 
        new AuthenticationService(logger, mapper, repositoryManager, configuration));
    private readonly Lazy<IUserService> _userService = new(() => 
        new UserService(repositoryManager, mapper));
    private readonly Lazy<ITemplateService> _templateService = new(() => 
        new TemplateService(repositoryManager, logger, mapper));
    private readonly Lazy<IFormService> _formService = new(() => 
        new FormService(repositoryManager, logger, mapper));
    private readonly Lazy<ICommentService> _commentService = new(() => 
        new CommentService(repositoryManager, logger, mapper, hubContext));
    private readonly Lazy<ILikeService> _likeService = new(() => 
        new LikeService(repositoryManager, logger, hubContext));
    private readonly Lazy<ITagService> _tagService = new(() => 
        new TagService(repositoryManager, logger, mapper));
    private readonly Lazy<IRoleService> _roleService = new(() => 
        new RoleService(repositoryManager, logger));
    public IAuthenticationService AuthenticationService => _authenticationService.Value;
    public IUserService UserService => _userService.Value;
    public ITemplateService TemplateService => _templateService.Value;
    public IFormService FormService => _formService.Value;
    public ICommentService CommentService => _commentService.Value;
    public ILikeService LikeService => _likeService.Value;
    public ITagService TagService => _tagService.Value;
    public IRoleService RoleService => _roleService.Value;
}