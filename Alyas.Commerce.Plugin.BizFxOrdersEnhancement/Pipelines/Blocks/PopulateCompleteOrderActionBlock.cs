namespace Alyas.Commerce.Plugin.BizFxOrdersEnhancement.Pipelines.Blocks
{
    using System;
    using System.Threading.Tasks;
    using Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Framework.Pipelines;

    public class PopulateCompleteOrderActionBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            if (arg == null || !string.Equals(arg.Name, context.GetPolicy<KnownOrderViewsPolicy>().Master, StringComparison.Ordinal))
                return Task.FromResult(arg);
            if (!(context.CommerceContext.GetObject<EntityViewArgument>()?.Entity is Order entity))
                return Task.FromResult(arg);
            var orderStatusPolicy = context.GetPolicy<KnownOrderStatusPolicy>();
            var actionsPolicy = arg.GetPolicy<ActionsPolicy>();
            var entityActionView1 = new EntityActionView
            {
                Name = context.GetPolicy<KnownAlyasOrderActionsPolicy>().CompleteOrder,
                DisplayName = "Complete Order",
                Description = "Completes a problem Order that was processed manually",
                IsEnabled = entity.Status.Equals(orderStatusPolicy.Problem, StringComparison.OrdinalIgnoreCase), 
                RequiresConfirmation = true,
                EntityView = string.Empty,
                Icon = "navigate_check"
            };
            actionsPolicy.Actions.Add(entityActionView1);
            
            return Task.FromResult(arg);
        }
    }
}
