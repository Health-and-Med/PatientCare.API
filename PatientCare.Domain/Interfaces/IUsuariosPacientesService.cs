using PatientCare.Domain.Entities;

namespace MedControl.Infrastructure.Repositories
{
    public interface IUsuariosPacientesService
    {
        Task CreateAsync(RequestCreateUsuarioPacienteModel user, int medicoId);
        Task DeleteAsync(int pacienteId);
        Task<IEnumerable<UsuariosPacientesModel>> GetAllAsync();
        Task<UsuariosPacientesModel> GetByIdAsync(int pacienteId);
        Task UpdateAsync(RequestUpdateUsuarioPaciente user, int medicoId);
    }
}