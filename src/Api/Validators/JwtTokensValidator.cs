using Core.Models;
using FastEndpoints;
using FluentValidation;


namespace Api.Validators;


public sealed class JwtTokensValidator : Validator<JwtTokens>
{
    public JwtTokensValidator()
    {
        RuleFor(x => x.Access).NotEmpty();
        RuleFor(x => x.Refresh).NotEmpty();
    }
}