using CriticalShop.Data;
using CriticalShop.Models;
using Microsoft.EntityFrameworkCore;

namespace CriticalShop.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Admin?> LoginAdminAsync(string email, string senha)
        {
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == email && a.Senha == senha && a.Ativo);

            if (admin != null)
            {
                // Armazenar na sessão
                _httpContextAccessor.HttpContext?.Session.SetString("AdminId", admin.Id.ToString());
                _httpContextAccessor.HttpContext?.Session.SetString("AdminEmail", admin.Email);
                _httpContextAccessor.HttpContext?.Session.SetString("AdminNome", admin.Nome ?? "");
                _httpContextAccessor.HttpContext?.Session.SetString("IsSuperAdmin", admin.IsSuperAdmin.ToString());
            }

            return admin;
        }

        public async Task<Usuario?> LoginUsuarioAsync(string email, string senha)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.Senha == senha && u.Ativo);

            if (usuario != null)
            {
                // Armazenar na sessão
                _httpContextAccessor.HttpContext?.Session.SetString("UsuarioId", usuario.Id.ToString());
                _httpContextAccessor.HttpContext?.Session.SetString("UsuarioEmail", usuario.Email);
                _httpContextAccessor.HttpContext?.Session.SetString("UsuarioNome", usuario.Nome);
            }

            return usuario;
        }

        public async Task<bool> CadastrarUsuarioAsync(Usuario usuario)
        {
            // Verificar se email já existe
            var emailExiste = await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email);
            if (emailExiste)
                return false;

            // Verificar se CPF já existe
            var cpfExiste = await _context.Usuarios.AnyAsync(u => u.Cpf == usuario.Cpf);
            if (cpfExiste)
                return false;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CadastrarAdminAsync(Admin admin)
        {
            // Verificar se email já existe
            var emailExiste = await _context.Admins.AnyAsync(a => a.Email == admin.Email);
            if (emailExiste)
                return false;

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
            return true;
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext?.Session.Clear();
        }

        public bool IsAdminLoggedIn()
        {
            return !string.IsNullOrEmpty(_httpContextAccessor.HttpContext?.Session.GetString("AdminId"));
        }

        public bool IsUsuarioLoggedIn()
        {
            return !string.IsNullOrEmpty(_httpContextAccessor.HttpContext?.Session.GetString("UsuarioId"));
        }

        public int? GetAdminId()
        {
            var adminIdStr = _httpContextAccessor.HttpContext?.Session.GetString("AdminId");
            return int.TryParse(adminIdStr, out int adminId) ? adminId : null;
        }

        public int? GetUsuarioId()
        {
            var usuarioIdStr = _httpContextAccessor.HttpContext?.Session.GetString("UsuarioId");
            return int.TryParse(usuarioIdStr, out int usuarioId) ? usuarioId : null;
        }

        public string? GetAdminEmail()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("AdminEmail");
        }

        public string? GetUsuarioEmail()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("UsuarioEmail");
        }

        public string? GetAdminNome()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("AdminNome");
        }

        public string? GetUsuarioNome()
        {
            return _httpContextAccessor.HttpContext?.Session.GetString("UsuarioNome");
        }

        public bool IsSuperAdmin()
        {
            var isSuperAdminStr = _httpContextAccessor.HttpContext?.Session.GetString("IsSuperAdmin");
            return bool.TryParse(isSuperAdminStr, out bool isSuperAdmin) && isSuperAdmin;
        }
    }
}
