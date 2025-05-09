﻿using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.API.Attributes;
using MyRecipeBook.Application.UseCases.User.ChangePassword;
using MyRecipeBook.Application.UseCases.User.Delete.Request;
using MyRecipeBook.Application.UseCases.User.Profile;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Application.UseCases.User.Update;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.API.Controllers
{
    public class UserController : MyRecipeBookBaseController
    {
        [HttpPost]
        
        [ProducesResponseType(typeof(ResponseRegisteredUserJson), StatusCodes.Status201Created)]

        public async Task<IActionResult> Register([FromServices] IRegisterUserUseCase useCase, [FromBody] RequestRegisterUserJson request)
        {
            var result = await useCase.Execute(request);

            return Created(string.Empty, result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResponseUserProfileJson), StatusCodes.Status200OK)]
        [AuthenticatedUser]
        public async Task<IActionResult> GetUserProfile([FromServices] IGetUserProfileUseCase useCase)
        {
            var result = await useCase.Execute();
            return Ok(result);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [AuthenticatedUser]
        public async Task<IActionResult> Update([FromServices] IUpdateUserUseCase useCase, [FromBody] RequestUpdateUserJson request)
        {
            await useCase.Execute(request);

            return Ok("Dados atualizados com sucesso."); 
        }

        [HttpPut("change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [AuthenticatedUser]
        public async Task<IActionResult> ChangePassword([FromServices] IChangePasswordUseCase useCase, [FromBody] RequestChangePasswordJson request)
        {
            await useCase.Execute(request);

            return Ok("Senha atualizada com sucesso.");
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [AuthenticatedUser]
        public async Task<IActionResult> Delete([FromServices] IRequestDeleteUserUseCase useCase)
        {
            await useCase.Execute();

            return NoContent();
        }
    }
}
