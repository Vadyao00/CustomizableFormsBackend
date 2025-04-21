using AutoMapper;
using Contracts.IRepositories;
using Contracts.IServices;
using CustomizableForms.Domain.ConfigurationModels;
using CustomizableForms.LoggerService;
using Microsoft.Extensions.Options;

namespace CustomizableForms.Application.Services;

public class ServiceManager(
    IRepositoryManager repositoryManager,
    ILoggerManager logger,
    IMapper mapper,
    IOptions<JwtConfiguration> configuration)
    : IServiceManager
{
    private readonly Lazy<IAuthenticationService> _authenticationService = new(() => 
        new AuthenticationService(logger, mapper, repositoryManager, configuration));

    public IAuthenticationService AuthenticationService => _authenticationService.Value;
}