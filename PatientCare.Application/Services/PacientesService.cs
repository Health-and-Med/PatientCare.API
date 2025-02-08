using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MedControl.Infrastructure.Repositories;
using PatientCare.Domain.Entities;

namespace PatientCare.Application.Services
{
    public class PacientesService : IPacientesService
    {
        private readonly IPacienteRepository _pacienteRepository;
        private readonly IUsuariosPacientesRepository _usuariosPacientesRepository;
        private readonly byte[] _key;
        public PacientesService(IPacienteRepository pacienteRepository, IUsuariosPacientesRepository usuariosPacientesRepository)
        {
            _pacienteRepository = pacienteRepository;
            _usuariosPacientesRepository = usuariosPacientesRepository;
            _key = Encoding.UTF8.GetBytes("a-secure-key-of-your-choice");
        }

        public async Task<IEnumerable<PacientesModel>> GetAllAsync()
        {
            try
            {
                return await _pacienteRepository.GetAllAsync();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<PacientesModel> GetByIdAsync(int id)
        {
            try
            {
                return await _pacienteRepository.GetByIdAsync(id);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task CreateAsync(RequestCreatePacientesModel paciente)
        {
            try
            {

                List<string> erros = paciente.ValidateModel();

                if (erros.Count > 0)
                    throw new Exception(string.Join("\n", erros));

                var pacienteExist = await _pacienteRepository.GetUserByEmailAsync(paciente.Email);

                if (pacienteExist != null)
                    throw new Exception("Paciente já registrado.");

                paciente.Usuario.SenhaHash = CreatePasswordHash(paciente.Usuario.SenhaHash);

                var newpaciente = await _pacienteRepository.CreateAsync(paciente);

                if (newpaciente != null)
                {
                    newpaciente.Usuario = new UsuariosPacientesModel { PacienteId = newpaciente.Id, SenhaHash = paciente.Usuario.SenhaHash };

                    if (newpaciente.Id == 0)
                        throw new ArgumentNullException("Erro ao Criar paciente.");
                    await _usuariosPacientesRepository.CreateAsync( paciente.Usuario, newpaciente.Id.Value);
                }
                else
                {
                    throw new Exception("Erro ao Cadastrar paciente");
                }
            }
            catch (Exception e)
            {

                throw;
            }


        }


        private string CreatePasswordHash(string password)
        {
            try
            {

                using (var hmac = new HMACSHA512(_key))
                {
                    var passwordBytes = Encoding.UTF8.GetBytes(password);
                    var hash = hmac.ComputeHash(passwordBytes);
                    return Convert.ToBase64String(hash);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            try
            {
                using (var hmac = new HMACSHA512(_key))
                {
                    var passwordBytes = Encoding.UTF8.GetBytes(password);
                    var hash = hmac.ComputeHash(passwordBytes);
                    var hashString = Convert.ToBase64String(hash);
                    return hashString == storedHash;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<PacientesModel> AuthenticateAsync(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    throw new ArgumentNullException("Email");

                if (string.IsNullOrEmpty(password))
                    throw new ArgumentNullException("password");


                var paciente = await _pacienteRepository.GetUserByEmailAsync(email);

                if (paciente == null)
                    return null;

                paciente.Usuario = await _usuariosPacientesRepository.GetByIdAsync(paciente.Id.Value);


                if (!VerifyPasswordHash(password, paciente.Usuario.SenhaHash))
                    return null;

                return paciente;
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public async Task UpdateAsync(RequestUpdatepacienteModel paciente, int pacienteId)
        {
            List<string> erros = paciente.ValidateModel();

            if (erros.Count > 0)
                throw new Exception(string.Join("\n", erros));

            var pacienteExist = await _pacienteRepository.GetByIdAsync(pacienteId);
            if (pacienteExist == null)
                throw new Exception("paciente não encontrado.");

            paciente.Usuario.SenhaHash = CreatePasswordHash(paciente.Usuario.SenhaHash);

            await _pacienteRepository.UpdateAsync(paciente, pacienteId);
        }

        public async Task DeleteAsync(int pacienteId)
        {
            PacientesModel paciente = await _pacienteRepository.GetByIdAsync(pacienteId);
            if (paciente == null)
                throw new Exception($"Não existe na base.");

            if (paciente.Email == "devs@email.com")
                throw new Exception($"Não é possível Deletar esse perfil");

            await _pacienteRepository.DeleteAsync(pacienteId);
        }


    }
}
