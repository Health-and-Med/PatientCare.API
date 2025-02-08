using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using PatientCare.Domain.Entities;


namespace MedControl.Infrastructure.Repositories
{
    public class UsuariosPacientesRepository : IUsuariosPacientesRepository
    {
        private string _connectionString;
        public UsuariosPacientesRepository(IDbConnection dbConnection, IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<UsuariosPacientesModel>> GetAllAsync()
        {

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    return await connection.QueryAsync<UsuariosPacientesModel>(
                         "SELECT * FROM UsuariosPacientes");
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task<UsuariosPacientesModel> GetByIdAsync(int pacienteId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    return await connection.QueryFirstOrDefaultAsync<UsuariosPacientesModel>(
                         "SELECT * FROM UsuariosPacientes WHERE pacienteId = @pacienteId", new { pacienteId = pacienteId });
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public async Task CreateAsync(RequestCreateUsuarioPacienteModel user, int pacienteId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    await connection.ExecuteAsync(
                        "INSERT INTO UsuariosPacientes (pacienteId, SenhaHash) VALUES (@pacienteId, @SenhaHash)",
                        new { user.SenhaHash, pacienteId });
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public async Task UpdateAsync(RequestUpdateUsuarioPaciente user, int pacienteId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    await connection.ExecuteAsync(
                        "UPDATE UsuariosPacientes SET SenhaHash = @SenhaHash WHERE pacienteId = @pacienteId", new { SenhaHash = user.SenhaHash, pacienteId });
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public async Task DeleteAsync(int pacienteId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    await connection.ExecuteAsync(
                        "DELETE FROM UsuariosPacientes WHERE PacienteId = @pacienteId", new { pacienteId });
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}


