using FluentValidation.TestHelper;
using SkillStack.Application.Commands.UserCommands;

namespace Tests.Application.Commands.CreateUserCommandsTests
{
    public class CreateUserCommandValidatorTests
    {
        private readonly CreateUserCommandValidator _validator;

        public CreateUserCommandValidatorTests()
        {
            _validator = new CreateUserCommandValidator();
        }

        [Fact]
        public void Should_HaveError_When_UserNameIsEmpty()
        {
            var model = new CreateUserCommand("", "Test Name", "test@example.com", "password123");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.UserName);
        }

        [Fact]
        public void Should_HaveError_When_NameIsTooShort()
        {
            var model = new CreateUserCommand("testuser", "A", "test@example.com", "password123");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_HaveError_When_EmailIsInvalid()
        {
            var model = new CreateUserCommand("testuser", "Test Name", "invalid-email", "password123");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_HaveError_When_PasswordTooShort()
        {
            var model = new CreateUserCommand("testuser", "Test Name", "test@example.com", "123");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.PasswordHash);
        }

        [Fact]
        public void Should_NotHaveErrors_When_ValidData()
        {
            var model = new CreateUserCommand("testuser", "Test Name", "test@example.com", "password123");
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
