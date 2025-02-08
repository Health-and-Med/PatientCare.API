using PatientCare.Domain.Entities;

namespace MedControl.Infrastructure.Repositories
{
    public interface IPacientesService
    {
        Task<PacientesModel> AuthenticateAsync(string crm, string password);
        Task CreateAsync(RequestCreatePacientesModel paciente);
        Task DeleteAsync(int pacienteId);
        Task<PacientesModel> GetByIdAsync(int id);
        Task<IEnumerable<PacientesModel>> GetAllAsync();
        Task UpdateAsync(RequestUpdatepacienteModel paciente, int userId);
    }
}