using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dev.IO.API.ViewModels;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dev.IO.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : MainController
    {
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;

        public AuthController(INotificador notificador, SignInManager<IdentityUser> signInManager, 
            UserManager<IdentityUser> userManager) : base(notificador)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
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
                return CustomResponse(registerUser);
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
                return CustomResponse(loginUser);
            }
            if(result.IsLockedOut)
            {
                NotificarErro("Usuário temporariamente bloqueado por tentativas inválidas");
                return CustomResponse(loginUser);
            }

            NotificarErro("Usuário ou Senha incorretos");
            return CustomResponse(loginUser);
        }
    }
}