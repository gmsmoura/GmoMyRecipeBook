using Microsoft.Extensions.Options;
using MyRecipeBook.Communication.Requests;
using MyRecipeBook.Domain.Extensions;
using MyRecipeBook.Domain.Repositories;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Services.LoggedUser;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Exceptions.ExceptionsBase;

namespace MyRecipeBook.Application.UseCases.User.Update
{
    public class UpdateUserUseCase : IUpdateUserUseCase
    {
        private readonly ILoggedUser _loggedUser;
        private readonly IUserUpdateOnlyRepository _repository;
        private readonly IUserReadOnlyRepository _userReadOnlyRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserUseCase(
            ILoggedUser loggedUser
            , IUserUpdateOnlyRepository repository
            , IUserReadOnlyRepository userReadOnlyRepository
            , IUnitOfWork unitOfWork
        )
        {
            _loggedUser = loggedUser;
            _repository = repository;
            _userReadOnlyRepository = userReadOnlyRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Execute(RequestUpdateUserJson request)
        {
            //recuperando o user logado
            var loggedUser = await _loggedUser.User();

            //validando o email do user que está logado, o user que está executando a chamada da requisição
            await Validate(request, loggedUser.Email);

            //recuperando o user pelo id através do repositório
            var user = await _repository.GetById(loggedUser.Id);

            user.Name = request.Name;
            user.Email = request.Email;

            //aplica update no repositório
            _repository.Update(user);

            //aplica a persistência dos dados com commit
            await _unitOfWork.Commit();
        }

        private async Task Validate(RequestUpdateUserJson request, string currentEmail)
        {
            var validator = new UpdateUserValidator();

            var result = validator.Validate(request);

            //validando se currentEmail (email capturado da base de dados) é diferente do email enviado na request
            if (currentEmail.Equals(request.Email).IsFalse())
            {
                //verificando se user existe com o email da request e está ativo, se sim envia uma exception de email já registrado
                var userExist = await _userReadOnlyRepository.ExistActiveUserWithEmail(request.Email);
                if (userExist)
                    result.Errors.Add(new FluentValidation.Results.ValidationFailure("email", ResourceMessagesExceptions.EMAIL_ALREADY_REGISTERED));
            }

            //se result for false, lança uma exceção de bad request (400)
            if (result.IsValid.IsFalse())
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();

                throw new ErrorOnValidationException(errorMessages);
            }
        }
    }
}
