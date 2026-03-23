using BloggingApp.Domain.Entities;
using BloggingAppPlatform.MVC.Models;

namespace BloggingAppPlatform.MVC.Areas.Admin.ViewModels;

public class OpClaimVM
{
    public OpClaimForm opClaim { get; set; } = new();
    public List<OperationClaim> OperationClaims { get; set; } = [];
}
