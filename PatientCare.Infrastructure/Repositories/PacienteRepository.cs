using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using PatientCare.Domain.Entities;

namespace MedControl.Infrastructure.Repositories
{
    public class PacienteRepository : IPacienteRepository
    {
        private readonly IUsuariosPacientesRepository _usuariosPacientesRepository;
        private string _connectionString;
        public PacienteRepository(IUsuariosPacientesRepository usuariosPacientesRepository, IConfiguration configuration)
        {
            _usuariosPacientesRepository = usuariosPacientesRepository;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<IEnumerable<PacientesModel>> GetAllAsync()
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    return await connection.QueryAsync<PacientesModel>(
                         "SELECT * FROM Pacientes");
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }
        public async Task<PacientesModel> GetByIdAsync(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    return await connection.QueryFirstOrDefaultAsync<PacientesModel>("SELECT * FROM Pacientes WHERE Id = @Id", new { Id = id });
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }
        public async Task<PacientesModel> CreateAsync(RequestCreatePacientesModel paciente)
        {

            try
            {
                var query = @"INSERT INTO Pacientes (Nome, CPF, Email, DataNascimento) 
                  VALUES (@Nome, @Cpf, @Email, @DataNascimento) 
                  RETURNING Id;"; // Retorna apenas o ID gerado

                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    int id = await connection.ExecuteScalarAsync<int>(query, paciente);
                    return await GetByIdAsync(id);
                }


            }
            catch (Exception e)
            {

                throw;
            }

        }
        public async Task<PacientesModel> GetUserByEmailAsync(string email)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    var paciente = await connection.QueryFirstOrDefaultAsync<PacientesModel>("SELECT * FROM Pacientes WHERE Email = @Email", new { email });

                    if (paciente == null)
                        return null;
                    paciente.Usuario = await _usuariosPacientesRepository.GetByIdAsync(paciente.Id.Value);

                    return paciente;
                }
            }
            catch (Exception e)
            {

                throw;
            }



        }
        public async Task<PacientesModel> GetUserByCpfAsync(string cpf)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    var paciente = await connection.QueryFirstOrDefaultAsync<PacientesModel>("SELECT * FROM Pacientes WHERE CPF = @cpf", new { cpf });

                    if (paciente == null)
                        return null;
                    paciente.Usuario = await _usuariosPacientesRepository.GetByIdAsync(paciente.Id.Value);

                    return paciente;
                }
            }
            catch (Exception e)
            {

                throw;
            }



        }
        public async Task UpdateAsync(RequestUpdatepacienteModel paciente, int pacienteId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString)) // Cria uma nova conexão
                {
                    await connection.OpenAsync();

                    await connection.ExecuteAsync(
                        $@"UPDATE 
                            Pacientes 
                          SET 
                                Nome = @Nome, 
                                Cpf = @Cpf, 
                                DataNascimento = @DataNascimento 
                          WHERE 
                            Id = @Id", new { paciente.Nome, paciente.Cpf, paciente.DataNascimento, Id = pacienteId });
                }

                await _usuariosPacientesRepository.UpdateAsync(paciente.Usuario, pacienteId);
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
                        "DELETE FROM Pacientes WHERE Id = @pacienteId", new { pacienteId });
                    await _usuariosPacientesRepository.DeleteAsync(pacienteId);
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        
    }
}


