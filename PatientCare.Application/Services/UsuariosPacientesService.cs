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
    public class UsuariosPacientesService : IUsuariosPacientesService
    {
        private readonly IUsuariosPacientesRepository  _usuariosPacientesRepository;
        public UsuariosPacientesService(IUsuariosPacientesRepository usuariosPacientesRepository)
        {
            _usuariosPacientesRepository = usuariosPacientesRepository;
        }

        public UsuariosPacientesService()
        {
        }

        public async Task<IEnumerable<UsuariosPacientesModel>> GetAllAsync()
        {
            return await _usuariosPacientesRepository.GetAllAsync();
        }

        public async Task<UsuariosPacientesModel> GetByIdAsync(int pacienteId)
        {
            return await _usuariosPacientesRepository.GetByIdAsync(pacienteId);
        }

        public async Task CreateAsync(RequestCreateUsuarioPacienteModel paciente, int pacienteId)
        {
            await _usuariosPacientesRepository.CreateAsync(paciente, pacienteId);
        }

        public async Task UpdateAsync(RequestUpdateUsuarioPaciente paciente, int pacienteId)
        {
            paciente.SenhaHash = CreatePasswordHash(paciente.SenhaHash);
            await _usuariosPacientesRepository.UpdateAsync(paciente, pacienteId);
        }

        public async Task DeleteAsync(int pacienteId)
        {
            await _usuariosPacientesRepository.DeleteAsync(pacienteId);
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
