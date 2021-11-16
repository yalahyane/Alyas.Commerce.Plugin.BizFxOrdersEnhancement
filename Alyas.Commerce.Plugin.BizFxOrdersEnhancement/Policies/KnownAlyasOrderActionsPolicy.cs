namespace Alyas.Commerce.Plugin.BizFxOrdersEnhancement.Policies
{
    using Sitecore.Commerce.Plugin.Orders;

    public class KnownAlyasOrderActionsPolicy : KnownOrderActionsPolicy
    {
        public KnownAlyasOrderActionsPolicy()
        {
            this.RetryAllProblemOrders = nameof(RetryAllProblemOrders);
            this.CompleteOrder = nameof(CompleteOrder);
        }
        public string RetryAllProblemOrders { get; set; }
        public string CompleteOrder { get; set; }
    }
}
