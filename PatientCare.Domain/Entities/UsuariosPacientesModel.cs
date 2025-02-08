using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientCare.Domain.Entities
{
    public class UsuariosPacientesModel
    {
        public int? Id { get; set; }
        public int? PacienteId { get; set; }
        public string SenhaHash;
    }
}


