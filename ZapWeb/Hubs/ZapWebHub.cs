using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;
using ZapWeb.BancoDados;
using ZapWeb.Models;

namespace ZapWeb.Hubs
{
    public class ZapWebHub : Hub
    {
        private ContextDB _context;
        public ZapWebHub(ContextDB context)
        {
            _context = context;
        }
        public async Task Cadastrar(Usuarios usuarios)
        {

            var IsExist = _context.Usuarios.Where(u => u.Email == usuarios.Email).Count() > 0;
            if (IsExist)
            {
                await Clients.Caller.SendAsync("ReceberCadastro", false, null, "E-mail já cadastrado.");

            }
            else
            {
                _context.Usuarios.Add(usuarios);
                _context.SaveChanges();

                await Clients.Caller.SendAsync("ReceberCadastro", true, usuarios, "Usuário cadastrado com sucesso!");

            }

        }
        public async Task Login(Usuarios usuarios)
        {
            var result = _context.Usuarios.FirstOrDefault(u => u.Email == usuarios.Email && u.Senha == usuarios.Senha);

            if (result == null)
            {
                await Clients.Caller.SendAsync("ReceberLogin", false, null, "Login inválido!");
            }
            else
            {
                await Clients.Caller.SendAsync("ReceberLogin", true, result, null);
            }

        }
    }

}
