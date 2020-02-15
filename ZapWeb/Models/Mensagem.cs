using System;

namespace ZapWeb.Models
{
    public class Mensagem
    {
        public int Id { get; set; }
        public string Grupo { get; set; }
        public string Usuario { get; set; }
        public string Texto { get; set; }
        public DateTime? DataCriacao { get; set; }
    }
}
