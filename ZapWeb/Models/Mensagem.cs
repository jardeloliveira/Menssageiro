using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZapWeb.Models
{
    public class Mensagem
    {
        public int Id { get; set; }
        public string Grupo { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioJson { get; set; }
        [NotMapped]
        public Usuarios Usuario { get; set; }
        public string Texto { get; set; }
        public DateTime? DataCriacao { get; set; }
    }
}
