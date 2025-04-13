namespace Contracts.IServices;

public interface IServiceManager
{
    IAuthenticationService AuthenticationService { get; }
    IUserService UserService { get; }
    ITemplateService TemplateService { get; }
    IFormService FormService { get; }
    ICommentService CommentService { get; }
    ILikeService LikeService { get; }
    ITagService TagService { get; }
    IRoleService RoleService { get; }
}