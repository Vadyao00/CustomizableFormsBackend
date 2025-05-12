using AutoMapper;
using Contracts.IRepositories;
using Contracts.IServices;
using CustomizableForms.Domain.ConfigurationModels;
using CustomizableForms.LoggerService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CustomizableForms.Application.Services;

public class ServiceManager : IServiceManager
{
    private readonly IRepositoryManager _repositoryManager;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly IOptions<JwtConfiguration> _configuration;
    private readonly IConfiguration _appConfiguration;

    private readonly Lazy<IAuthenticationService> _authenticationService;
    private readonly Lazy<IDropboxService> _dropBoxService;
    private readonly Lazy<ISupportService> _supportService;

    public ServiceManager(
        IRepositoryManager repositoryManager,
        ILoggerManager logger,
        IMapper mapper,
        IOptions<JwtConfiguration> configuration,
        IConfiguration appConfiguration)
    {
        _repositoryManager = repositoryManager;
        _logger = logger;
        _mapper = mapper;
        _configuration = configuration;
        _appConfiguration = appConfiguration;

        _authenticationService = new Lazy<IAuthenticationService>(() => 
            new AuthenticationService(logger, mapper, repositoryManager, configuration));

        _dropBoxService = new Lazy<IDropboxService>(() => 
            new DropboxService(logger, appConfiguration));

        _supportService = new Lazy<ISupportService>(() => 
            new SupportService(logger, _dropBoxService.Value));
    }

    public IAuthenticationService AuthenticationService => _authenticationService.Value;
    public IDropboxService DropboxService => _dropBoxService.Value;
    public ISupportService SupportService => _supportService.Value;
}