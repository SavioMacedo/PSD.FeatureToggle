using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PSD.FeatureToggle.Entities
{
    public class Usuario
    {
        public string Login { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public int EscolaId { get; set; }
    }
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> GetAllUsersAsync();
        Task<Usuario> GetUserAsync(string login, string password);
    }

    public class UsuarioService : IUsuarioService
    {
        private readonly List<Usuario> _users = new()
        {
            new Usuario
            {
                Login = "raimundo",
                Name = "Professor Raimundo",
                Password = "123456",
                Role = "COORDENADOR",
                EscolaId = 1
            },
            new Usuario
            {
                Login = "girafales",
                Name = "Professor Girafales",
                Password = "123456",
                Role = "PROFESSOR",
                EscolaId = 2
            },
            new Usuario
            {
                Login = "chaves",
                Name = "Chaves do 8",
                Password = "123456",
                Role = "ALUNO",
                EscolaId = 2
            },
            new Usuario
            {
                Login = "cacilda",
                Name = "Dona Cacilda",
                Password = "123456",
                Role = "ALUNO",
                EscolaId = 1
            }
        };

        public Task<IEnumerable<Usuario>> GetAllUsersAsync() => Task.FromResult(_users.AsEnumerable());
        public Task<Usuario> GetUserAsync(string login, string password)
            => Task.FromResult(_users.FirstOrDefault(x => x.Login.Equals(login, StringComparison.OrdinalIgnoreCase) &&
             x.Password == password));
    }

    public static class TokenService
    {
        public static string GenerateToken(this Usuario user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes("5621F303-290E-4798-9C92-C07B02995EA4");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new (ClaimTypes.Name, user.Name),
                    new (ClaimTypes.Role, user.Role),
                    new ("EscolaId", user.EscolaId.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
