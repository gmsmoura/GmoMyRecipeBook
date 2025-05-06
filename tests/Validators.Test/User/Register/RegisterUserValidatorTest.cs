using CommonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exceptions;

namespace Validators.Test.User.Register
{
    public class RegisterUserValidatorTest
    {
        //Fact para sinalizar que o método se trata de um método específico de testes
        [Fact]
        public void Success()
        {
            //instanciando a classe RegisterUserValidator
            var validator = new RegisterUserValidator();

            //instanciando a classe que trás as instâncias de RequestRegisterUserJson para ser utilizado nos testes sem ter que chamar manualmente para cada teste
            var request = RequestRegisterUserJsonBuilder.Build();

            //retornando o resultado validado
            var result = validator.Validate(request);

            //assert pra verificar se o resultado retornado é o resultado de success, o formato abaixo é utilizando a biblioteca FluentAssertions
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Error_Name_Empty()
        {
            var validator = new RegisterUserValidator();
            
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Name = String.Empty;
            
            var result = validator.Validate(request);         
            
            result.IsValid.Should().BeFalse();

            result.Errors.Should().ContainSingle()
                .And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.NAME_EMPTY));
        }

        [Fact]
        public void Error_Email_Empty()
        {
            var validator = new RegisterUserValidator();
            
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Email = String.Empty;
            
            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            
            result.Errors.Should().ContainSingle()
                .And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.EMAIL_EMPTY));
        }

        [Fact]
        public void Error_Email_Invalid()
        {
            var validator = new RegisterUserValidator();
            
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Email = "email.com";
            
            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            
            result.Errors.Should().ContainSingle()
                .And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.EMAIL_NOT_VALID));
        }

        //Theory para permitir criar testes para cenários de repetição e o InlineData, cada um deles se equivale a uma repetição, neste caso,
        //executará o método de password 5x informando o passwordLength (tamanho do password) como parâmetro que foi configurado no Build do RequestRegisterUserJsonBuilder
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void Error_Password_Invalid(int passwordLength)
        {
            var validator = new RegisterUserValidator();
            
            var request = RequestRegisterUserJsonBuilder.Build(passwordLength);
            
            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
           
            result.Errors.Should().ContainSingle()
                .And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.INVALID_PASSWORD));
        }

        [Theory]
        [InlineData(8)]
        public void Error_Password_Valid(int passwordLength)
        {
            var validator = new RegisterUserValidator();
            
            var request = RequestRegisterUserJsonBuilder.Build(passwordLength);
            
            var result = validator.Validate(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Error_Password_Empty()
        {
            var validator = new RegisterUserValidator();
            
            var request = RequestRegisterUserJsonBuilder.Build();
            request.Password = String.Empty;
            
            var result = validator.Validate(request);

            result.IsValid.Should().BeFalse();
            
            result.Errors.Should().ContainSingle()
                .And.Contain(e => e.ErrorMessage.Equals(ResourceMessagesExceptions.PASSWORD_EMPTY));
        }
    }
}
