using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            //Adicionar usuario no grupo
            var grupos = _context.Grupos.Where(a=>a.Usuarios.Contains(result.Email));
            foreach (var item in listConnectionsId)
            {
                foreach (var grupo in grupos)
                {
                   await Groups.AddToGroupAsync(item,grupo.Nome);
                }
               
            }
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
               
                //Remoção do grupo
                var grupos = _context.Grupos.Where(a => a.Usuarios.Contains(result.Email));
                foreach (var item in listConnectionsId)
                {
                    foreach (var grupo in grupos)
                    {
                        await Groups.RemoveFromGroupAsync(item, grupo.Nome);
                    }

                }

            }
        }

        public async Task ObterListUsuarios()
        {
            var usuarios = _context.Usuarios.ToList();
            await Clients.Caller.SendAsync("ReceberListUsuarios",usuarios.Where(u => u.Nome != null));
        }

        public async Task CriarOuAbrirGrupo(string emailUsuarioUm, string emailUsuarioDois)
        {
            var nomeGrupo = CriarNomeGrupo(emailUsuarioUm, emailUsuarioDois);
            var grupo = _context.Grupos.FirstOrDefault(a => a.Nome == nomeGrupo);

            if (grupo == null)
            {
                grupo = new Grupos
                {
                    Nome = nomeGrupo,
                    Usuarios = JsonConvert.SerializeObject(new List<string>()
                    {
                        emailUsuarioUm,
                        emailUsuarioDois

                    })
                };
                _context.Grupos.Add(grupo);
                _context.SaveChanges();


            }
            var emails = JsonConvert.DeserializeObject<List<string>>(grupo.Usuarios);
            var usuarios = new List<Usuarios>
            {
                _context.Usuarios.First(a => a.Email == emails[0]),
                _context.Usuarios.First(a => a.Email == emails[1])

            };
            foreach (var user in usuarios)
            {
                var connectionId = JsonConvert.DeserializeObject<List<Usuarios>>(user.ConnectionId);
                foreach (var item in connectionId)
                {
                    await Groups.AddToGroupAsync(item.ConnectionId, nomeGrupo);
                }
            }
            await Clients.Caller.SendAsync("AbrirGrupo", nomeGrupo);
        }
        public async Task EnviarMessagem(Usuarios usuarios, string mensagem,string nomeGrupo)
        {
            Grupos grupos = _context.Grupos.FirstOrDefault(g => g.Nome == nomeGrupo);
            if (!grupos.Usuarios.Contains(usuarios.Email))
            {
                throw new Exception("Usuário não pertence ao grupo!");
            }

            var msg = new Mensagem
            {
                 Grupo = nomeGrupo,
                 Texto = mensagem,
                 UsuarioId = usuarios.Id,
                 UsuarioJson = JsonConvert.SerializeObject(usuarios),
                 Usuario = usuarios,
                 DataCriacao = DateTime.Now
                 
            };

            _context.Mensagem.Add(msg);
            _context.SaveChanges();


            await Clients.Group(nomeGrupo).SendAsync("ReceberMensagem", msg);

        }
        private string CriarNomeGrupo(string emailUsuarioUm, string emailUsuarioDois)
        {
            var list = new List<string> {emailUsuarioUm, emailUsuarioDois }.OrderBy(a=>a).ToList();
            var sb = new StringBuilder();
            foreach (var item in list)
            {
                sb.Append(item);
            }
            return sb.ToString();
        }
    }

}
