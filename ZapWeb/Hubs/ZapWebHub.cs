using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Collections.Generic;
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
                usuarios.IsOnline = true;
                _context.Usuarios.Update(usuarios);
                _context.SaveChanges();

                await Clients.All.SendAsync("ReceberListUsuarios", _context.Usuarios.Where(u => u.Nome != null).ToList());
            }

        }
        public async Task Logout(Usuarios usuarios)
        {
            var result = _context.Usuarios.Find(usuarios.Id);
            result.IsOnline = false;
            _context.Usuarios.Update(result);
            _context.SaveChanges();

            await DelConnectionIdDoUsuario(result);

            await Clients.All.SendAsync("ReceberListUsuarios", _context.Usuarios.Where(u=>u.Nome != null).ToList());
        }

        public async Task AddConnectionIdDoUsuario (Usuarios usuarios)
        {
            var connectioIdCurrent = Context.ConnectionId;
            var result = _context.Usuarios.Find(usuarios.Id);
            List<string> listConnectionsId = null;
            if (result.ConnectionId == null)
            {
                listConnectionsId = new List<string>();
                listConnectionsId.Add(connectioIdCurrent);
               
            }
            else
            {
                
                 listConnectionsId = JsonConvert.DeserializeObject<List<string>>(result.ConnectionId);
                if (!listConnectionsId.Contains(connectioIdCurrent))
                {
                    listConnectionsId.Add(connectioIdCurrent);
                }
                

            }
            result.ConnectionId = JsonConvert.SerializeObject(listConnectionsId);
            _context.Usuarios.Update(result);
            _context.SaveChanges();
        }
        public async Task DelConnectionIdDoUsuario(Usuarios usuarios)
        {
            var result = _context.Usuarios.Find(usuarios.Id);
            if (result.ConnectionId.Length > 0)
            {
                var connectioIdCurrent = Context.ConnectionId;
                var listConnectionsId = JsonConvert.DeserializeObject<List<string>>(result.ConnectionId);
                if (listConnectionsId.Contains(connectioIdCurrent))
                {
                    listConnectionsId.Remove(connectioIdCurrent);
                }
                result.ConnectionId = JsonConvert.SerializeObject(listConnectionsId);
                _context.Usuarios.Update(result);
                _context.SaveChanges();

            }
        }

        public async Task ObterListUsuarios()
        {
            var usuarios = _context.Usuarios.ToList();
            await Clients.Caller.SendAsync("ReceberListUsuarios",usuarios.Where(u => u.Nome != null));
        }
    }

}
