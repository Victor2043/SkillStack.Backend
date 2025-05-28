using FluentValidation.TestHelper;
using SkillStack.Application.Commands.Login;
using SkillStack.Application.Commands.LoginCommands;

namespace Tests.Application.Commands.LoginCommandsTests
{
    public class LoginCommandValidatorTests
    {
        private readonly LoginCommandValidator _validator;

        public LoginCommandValidatorTests()
        {
            _validator = new LoginCommandValidator();
        }

        [Fact]
        public void Should_HaveError_When_EmailIsEmpty()
        {
            var model = new LoginCommand("", "password123");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_HaveError_When_EmailIsInvalid()
        {
            var model = new LoginCommand("invalid-email", "password123");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_HaveError_When_PasswordIsEmpty()
        {
            var model = new LoginCommand("user@example.com", "");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_HaveError_When_PasswordTooShort()
        {
            var model = new LoginCommand("user@example.com", "123");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_NotHaveErrors_When_ValidData()
        {
            var model = new LoginCommand("user@example.com", "password123");
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}