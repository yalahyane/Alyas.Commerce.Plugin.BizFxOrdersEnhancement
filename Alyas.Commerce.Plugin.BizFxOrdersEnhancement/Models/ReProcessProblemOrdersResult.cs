namespace Alyas.Commerce.Plugin.BizFxOrdersEnhancement.Models
{
    using System.Collections.Generic;
    using Sitecore.Commerce.Core;

    public class ReProcessProblemOrdersResult : Model
    {
        public string Status { get; set; } = "OK";
        public string ErrorMessage { get; set; }
        public List<string> SuccessfulOrders { get; set; } = new List<string>();
        public List<FailedOrder> FailedOrders { get; set; } = new List<FailedOrder>();
    }
}
