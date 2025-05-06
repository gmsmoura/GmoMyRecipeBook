using CommonTestUtilities.Requests;
using FluentAssertions;
using MyRecipeBook.Application.UseCases.User.Register;
using MyRecipeBook.Exceptions;

namespace Validators.Test.User.Register
{
    public class RegisterUserValidatorTest
    {
        
        [Fact]
        public void Success()
        {
            
            var validator = new RegisterUserValidator();

            
            var request = RequestRegisterUserJsonBuilder.Build();

            
            var result = validator.Validate(request);

            
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
