using PatientCare.Domain.Interfaces;
using Dapper;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using PatientCare.Domain.Entities;
using MedControl.Infrastructure.Repositories;

namespace PatientCare.Infrastructure.Repositories
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IDbConnection _dbConnection;
        private readonly IPacienteRepository  _pacienteRepository;
        private readonly IUsuariosPacientesService  _usuariosPacientesService;

        public DatabaseInitializer(IDbConnection dbConnection, IPacienteRepository pacienteRepository, IUsuariosPacientesService usuariosPacientesService)
        {
            _dbConnection = dbConnection;
            _pacienteRepository = pacienteRepository;
            _usuariosPacientesService = usuariosPacientesService;
        }

        public async Task InitializeAsync()
        {
            var createPacientesTableQuery = @"
                CREATE TABLE IF NOT EXISTS Pacientes (
                    Id SERIAL PRIMARY KEY,
                    Nome VARCHAR(50) NOT NULL,
                    Cpf VARCHAR(50) NOT NULL,
                    DataNascimento DATE NOT NULL,
                    Email VARCHAR(255) NOT NULL
                );
            ";

            var createUsuarioPacientesTableQuery = @"
                CREATE TABLE IF NOT EXISTS UsuariosPacientes (
                    Id SERIAL PRIMARY KEY,
                    PacienteId INT NOT NULL,
                    SenhaHash TEXT
                );
            ";

            _dbConnection.Execute(createPacientesTableQuery);
            _dbConnection.Execute(createUsuarioPacientesTableQuery);
            

            // Adicionar usuário admin se não existir
            await AddAdminUserAsync();
        }

        private async Task AddAdminUserAsync()
        {
            try
            {
                var checkAdminUserQuery = "SELECT COUNT(*) FROM Pacientes WHERE Email = @email";
                var adminUserCount = _dbConnection.ExecuteScalar<int>(checkAdminUserQuery, new { email = "devs@email.com" });
                RequestCreatePacientesModel pacientes = new RequestCreatePacientesModel {  Cpf = "00000000000", DataNascimento = new DateTime(1990,11,24), Email = "devs@email.com", Nome = "devs"};
                if (adminUserCount == 0)
                {
                    var paciente = await _pacienteRepository.CreateAsync(pacientes);
                    await _usuariosPacientesService.CreateAsync(new RequestCreateUsuarioPacienteModel { SenhaHash = CreatePasswordHash("123456") }, paciente.Id.Value);
                    
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }

        private string CreatePasswordHash(string password)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes("a-secure-key-of-your-choice")))
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var hash = hmac.ComputeHash(passwordBytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}

