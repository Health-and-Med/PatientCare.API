using PatientCare.Domain.Entities;

namespace MedControl.Infrastructure.Repositories
{
    public interface IPacienteRepository
    {
        Task<PacientesModel> CreateAsync(RequestCreatePacientesModel paciente);
        Task DeleteAsync(int pacienteId);
        Task<IEnumerable<PacientesModel>> GetAllAsync();
        Task<PacientesModel> GetByIdAsync(int id);
        Task<PacientesModel> GetUserByEmailAsync(string email);
        Task<PacientesModel> GetUserByCpfAsync(string cpf);

        Task UpdateAsync(RequestUpdatepacienteModel paciente, int pacienteId);
    }
}