namespace Contracts.IRepositories;

public interface IRepositoryManager
{
    IUserRepository User { get; }
    ITemplateRepository Template { get; }
    IQuestionRepository Question { get; }
    IFormRepository Form { get; }
    IAnswerRepository Answer { get; }
    ITagRepository Tag { get; }
    ICommentRepository Comment { get; }
    ILikeRepository Like { get; }
    IRoleRepository Role { get; }
    Task SaveAsync();
}