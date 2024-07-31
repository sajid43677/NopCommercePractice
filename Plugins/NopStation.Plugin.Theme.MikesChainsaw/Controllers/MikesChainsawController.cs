using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Security;
using Nop.Plugin.NopStation.Theme.MikesChainsaw.Models;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.NopStation.Theme.MikesChainsaw.Controllers;

public class MikesChainsawController : BasePluginController
{
    private readonly CaptchaSettings _captchaSettings;
    private readonly ILocalizationService _localizationService;
    private readonly IWorkflowMessageService _workflowMessageService;
    private readonly ICustomerActivityService _customerActivityService;
    private readonly IWorkContext _workContext;

    public MikesChainsawController(CaptchaSettings captchaSettings,
        ILocalizationService localizationService,
        IWorkflowMessageService workflowMessageService,
        IWorkContext workContext,
        ICustomerActivityService customerActivityService)
    {
        _captchaSettings = captchaSettings;
        _localizationService = localizationService;
        _workflowMessageService = workflowMessageService;
        _workContext = workContext;
        _customerActivityService = customerActivityService;
    }

    public IActionResult BookService()
    {
        var model = new BookServiceModel
        {
            DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> BookService(BookServiceModel model, bool captchaValid)
    {
        //validate CAPTCHA
        if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
        {
            ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
        }

        model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage;

        if (ModelState.IsValid)
        {
            var subject = "Servicing Request From - " + model.FullName;
            var body = await _localizationService.GetResourceAsync("NopStation.Theme.MikesChainsaw.bookservice.PhoneNumber") + " : " + model.PhoneNumber + "\n" +
                await _localizationService.GetResourceAsync("NopStation.Theme.MikesChainsaw.bookservice.PickupRequired") + " : " + model.PickupRequired + "\n" +
                await _localizationService.GetResourceAsync("NopStation.Theme.MikesChainsaw.bookservice.MachineMakeModel") + " : " + model.MachineMakeModel + "\n" +
                await _localizationService.GetResourceAsync("NopStation.Theme.MikesChainsaw.bookservice.BriefSummary") + " : " + model.BriefSummary;

            await _workflowMessageService.SendContactUsMessageAsync((await _workContext.GetWorkingLanguageAsync()).Id,
                model.Email.Trim(), model.FullName, subject, body);

            model.SuccessfullySent = true;
            model.Result = await _localizationService.GetResourceAsync("BookAService.YourEnquiryHasBeenSent");

            //activity log
            await _customerActivityService.InsertActivityAsync("PublicStore.ContactUs",
                await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ContactUs"));

            return View(model);
        }

        return View(model);
    }
}
