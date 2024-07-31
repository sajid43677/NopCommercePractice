using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.NopStation.Theme.MikesChainsaw.Models;

public record BookServiceModel : BaseNopModel
{
    [DataType(DataType.EmailAddress)]
    [NopResourceDisplayName("NopStation.Theme.MikesChainsaw.bookservice.Email")]
    public string Email { get; set; }

    [NopResourceDisplayName("NopStation.Theme.MikesChainsaw.bookservice.Subject")]
    public string Subject { get; set; }
    public bool SubjectEnabled { get; set; }

    [NopResourceDisplayName("NopStation.Theme.MikesChainsaw.bookservice.PhoneNumber")]
    public string PhoneNumber { get; set; }

    [NopResourceDisplayName("NopStation.Theme.MikesChainsaw.bookservice.PickupRequired")]
    public string PickupRequired { get; set; }


    [NopResourceDisplayName("NopStation.Theme.MikesChainsaw.bookservice.MachineMakeModel")]
    public string MachineMakeModel { get; set; }

    [NopResourceDisplayName("NopStation.Theme.MikesChainsaw.bookservice.BriefSummary")]
    public string BriefSummary { get; set; }

    [NopResourceDisplayName("NopStation.Theme.MikesChainsaw.bookservice.FullName")]
    public string FullName { get; set; }

    public bool SuccessfullySent { get; set; }
    public string Result { get; set; }

    public bool DisplayCaptcha { get; set; }
}