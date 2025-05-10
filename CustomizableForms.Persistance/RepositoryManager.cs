using Contracts.IRepositories;
using CustomizableForms.Persistance.Repositories;

namespace CustomizableForms.Persistance;

public class RepositoryManager : IRepositoryManager
{
    private readonly CustomizableFormsContext _context;
    private readonly Lazy<IUserRepository> _userRepository;
    private readonly Lazy<ITemplateRepository> _templateRepository;
    private readonly Lazy<IQuestionRepository> _questionRepository;
    private readonly Lazy<IFormRepository> _formRepository;
    private readonly Lazy<IAnswerRepository> _answerRepository;
    private readonly Lazy<ITagRepository> _tagRepository;
    private readonly Lazy<ICommentRepository> _commentRepository;
    private readonly Lazy<ILikeRepository> _likeRepository;
    private readonly Lazy<IRoleRepository> _roleRepository;
    private readonly Lazy<IUserSalesforceProfileRepository> _userSalesforceProfileRepository;
    
    public RepositoryManager(CustomizableFormsContext context)
    {
        _context = context;
        _userRepository = new Lazy<IUserRepository>(() => new UserRepository(context));
        _templateRepository = new Lazy<ITemplateRepository>(() => new TemplateRepository(context));
        _questionRepository = new Lazy<IQuestionRepository>(() => new QuestionRepository(context));
        _formRepository = new Lazy<IFormRepository>(() => new FormRepository(context));
        _answerRepository = new Lazy<IAnswerRepository>(() => new AnswerRepository(context));
        _tagRepository = new Lazy<ITagRepository>(() => new TagRepository(context));
        _commentRepository = new Lazy<ICommentRepository>(() => new CommentRepository(context));
        _likeRepository = new Lazy<ILikeRepository>(() => new LikeRepository(context));
        _roleRepository = new Lazy<IRoleRepository>(() => new RoleRepository(context));
        _userSalesforceProfileRepository = new Lazy<IUserSalesforceProfileRepository>(() => new UserSalesforceProfileRepository(context));
    }

    public IUserRepository User => _userRepository.Value;
    public ITemplateRepository Template => _templateRepository.Value;
    public IQuestionRepository Question => _questionRepository.Value;
    public IFormRepository Form => _formRepository.Value;
    public IAnswerRepository Answer => _answerRepository.Value;
    public ITagRepository Tag => _tagRepository.Value;
    public ICommentRepository Comment => _commentRepository.Value;
    public ILikeRepository Like => _likeRepository.Value;
    public IRoleRepository Role => _roleRepository.Value;
    public IUserSalesforceProfileRepository UserSalesforceProfile => _userSalesforceProfileRepository.Value;
    
    public async Task SaveAsync() => await _context.SaveChangesAsync();
}