using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZayirAlkhayr.Entities.Auth;

namespace ZayirAlkhayr.Interfaces.Auth
{
    public interface IJwtProvider
    {
        (string token, int expiresIn) GenerateToken(AdminUser user);
        string? ValidateToken(string token);
    }
}
