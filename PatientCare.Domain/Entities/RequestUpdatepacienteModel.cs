using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientCare.Domain.Entities
{
    public class RequestUpdatepacienteModel
    {
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public RequestUpdateUsuarioPaciente Usuario { get; set; }

        public DateTime DataNascimento { get; set; }

        public List<string> ValidateModel()
        {
            try
            {
                List<string> erros = new List<string>();
                if (string.IsNullOrEmpty(Nome))
                    erros.Add("Nome é obrigatório.");

                if (Cpf == null)
                    erros.Add("Cpf é obrigatório.");

                if (Usuario == null)
                    erros.Add("Usuario é obrigatório.");

                if (string.IsNullOrEmpty(Usuario.SenhaHash))
                    erros.Add("SenhaHash é obrigatória.");

                return erros;
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}


