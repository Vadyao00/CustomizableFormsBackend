namespace Contracts.IServices;

public interface IServiceManager
{
    IAuthenticationService AuthenticationService { get; }
    IDropboxService DropboxService { get;  }
    ISupportService SupportService { get;  }
}