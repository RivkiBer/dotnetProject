using UserNamespace.Models;
using Microsoft.AspNetCore.Http;

namespace UserService.interfaces;

public interface IActiveUser
{
    User ActiveUser { get; }
}