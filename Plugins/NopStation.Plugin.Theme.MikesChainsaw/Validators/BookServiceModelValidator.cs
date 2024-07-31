using FluentValidation;
using Nop.Plugin.NopStation.Theme.MikesChainsaw.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.NopStation.Theme.MikesChainsaw.Validators;

public class BookServiceModelValidator : BaseNopValidator<BookServiceModel>
{
    public BookServiceModelValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("NopStation.Theme.MikesChainsaw.bookservice.Name.Required"));

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("NopStation.Theme.MikesChainsaw.bookservice.Email.Required"));

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("NopStation.Theme.MikesChainsaw.bookservice.PhoneNumber.Required"));

        RuleFor(x => x.PickupRequired)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("NopStation.Theme.MikesChainsaw.bookservice.PickupRequired.Required"));

        RuleFor(x => x.MachineMakeModel)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("NopStation.Theme.MikesChainsaw.bookservice.MachineMakeModel.Required"));

        RuleFor(x => x.BriefSummary)
            .NotEmpty()
            .WithMessageAwait(localizationService.GetResourceAsync("NopStation.Theme.MikesChainsaw.bookservice.BriefSummary.Required"));

    }
}
