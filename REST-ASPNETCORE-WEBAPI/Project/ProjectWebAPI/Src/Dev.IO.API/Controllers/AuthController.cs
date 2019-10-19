using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dev.IO.API.Extensions;
using Dev.IO.API.ViewModels;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Dev.IO.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly AppSettings appSettings;

        public AuthController(INotificador notificador, SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager, IOptions<AppSettings> appSettings) : base(notificador)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.appSettings = appSettings.Value;
        }

        [HttpPost("nova-conta")]
        public async Task<ActionResult> Registrar(RegisterUserViewModel registerUser)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var user = new IdentityUser
            {
                UserName = registerUser.Email,
                Email = registerUser.Email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, registerUser.Password);
            if(result.Succeeded)
            {
                await signInManager.SignInAsync(user, false); //realiza login do usuário
                return CustomResponse(GerarJWT());
            }

            foreach (var error in result.Errors)
            {
                NotificarErro(error.Description);
            }

            return CustomResponse(registerUser);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Entrar(LoginUserViewModel loginUser)
        {
            if (!ModelState.IsValid)
                return CustomResponse(ModelState);

            var result = await signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true); //PasswordSignInAsync => Login com usuario e senha7

            if (result.Succeeded)
            {
                return CustomResponse(GerarJWT());
            }
            if(result.IsLockedOut)
            {
                NotificarErro("Usuário temporariamente bloqueado por tentativas inválidas");
                return CustomResponse(loginUser);
            }

            NotificarErro("Usuário ou Senha incorretos");
            return CustomResponse(loginUser);
        }

        private string GerarJWT()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = appSettings.Emissor,
                Audience = appSettings.ValidoEm,
                Expires = DateTime.UtcNow.AddHours(appSettings.ExpiracaoHoras),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

            var encodedToken = tokenHandler.WriteToken(token);
            return encodedToken;
        }
    }
}