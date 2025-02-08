using PatientCare.Domain.Entities;
using PatientCare.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MedControl.Infrastructure.Repositories;

namespace PatientCare.API.Controllers
{
    [ApiController]
    [Route("api/patients")]
    public class PatientsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IPacientesService _pacienteService;

        public PatientsController(IConfiguration configuration, IPacientesService medicoService, IUsuariosPacientesService usuariosMedicosService)
        {
            _configuration = configuration;
            _pacienteService = medicoService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel request)
        {
            try
            {
                if (request.Email == null && request.Cpf == null)
                {
                    return BadRequest("Preencha E-mail ou Cpf para autenticar!");
                }
                var user = await _pacienteService.AuthenticateAsync(request.Email,request.Cpf, request.Password);
                if (user == null)
                    return Unauthorized();


                var token = GenerateJwtToken(user.Id.Value, user.Email);
                return Ok(new { token });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] RequestCreatePacientesModel register)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var role = User.FindFirst(ClaimTypes.Role).Value;

                if (role != "patient")
                    return Forbid("Exclusivo para perfil Paciente.");


                await _pacienteService.CreateAsync(register);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] RequestUpdatepacienteModel register)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var role = User.FindFirst(ClaimTypes.Role).Value;

                if (role != "patient")
                    return Forbid("Exclusivo para perfil Paciente.");


                await _pacienteService.UpdateAsync(register, userId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("delete")]
        [Authorize]
        public async Task<IActionResult> Delete()
        {
            try
            {

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var role = User.FindFirst(ClaimTypes.Role).Value;


                if (role != "patient")
                    return Forbid("Exclusivo para perfil Paciente.");

                await _pacienteService.DeleteAsync(userId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("getAll")]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var role = User.FindFirst(ClaimTypes.Role).Value;

                if (role != "doctor" && role != "admin" )
                    return Forbid("Exclusivo para perfil Doctor.");

                var medicos = await _pacienteService.GetAllAsync();
                return Ok(medicos);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private string GenerateJwtToken(int pacienteId, string email)
        {
            var jwtConfig = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtConfig["Secret"]!);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, pacienteId.ToString()), //  ID do usuário
                new Claim(ClaimTypes.Role, "patient"),
                new Claim(ClaimTypes.Name, email) //  E-mail do usuário
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtConfig["Issuer"],
                Audience = jwtConfig["Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}

