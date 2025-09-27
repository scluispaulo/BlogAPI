using FluentValidation;

public class CreatePostDtoValidator : AbstractValidator<CreatePostDto>
{
    public CreatePostDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(250).WithMessage("Title must be at most 250 characters long");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required");
    }
}
