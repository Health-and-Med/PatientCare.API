using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientCare.Domain.Entities
{
    public class PacientesModel
    {
        public int? Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string DataNascimento { get; set; }
        public UsuariosPacientesModel Usuario { get; set; }
    }
}


