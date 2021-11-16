namespace Alyas.Commerce.Plugin.BizFxOrdersEnhancement.Pipelines.Blocks
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.BusinessUsers;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    public class PopulateRetryAllProblemOrdersViewActionsBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull(this.Name + ": The argument cannot be null.");
            if (string.IsNullOrEmpty(arg?.Name) || !arg.Name.Equals(context.GetPolicy<KnownOrderViewsPolicy>().OrdersDashboard, StringComparison.OrdinalIgnoreCase) || !string.IsNullOrEmpty(arg.Action))
                return Task.FromResult(arg);
            var entityViewArgument = context.CommerceContext.GetObjects<EntityViewArgument>().FirstOrDefault();
            if (!string.IsNullOrEmpty(entityViewArgument?.ViewName) && entityViewArgument.ViewName.Equals(context.GetPolicy<KnownBusinessUsersViewsPolicy>().ToolsNavigation, StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(arg);
            var actions = arg.GetPolicy<ActionsPolicy>().Actions;
            actions.Add(new EntityActionView
            {
                Name = context.GetPolicy<KnownAlyasOrderActionsPolicy>().RetryAllProblemOrders,
                DisplayName = "Retry All Problem Orders",
                Description = "Retry All Problem Orders",
                IsEnabled = true,
                EntityView = string.Empty,
                RequiresConfirmation = true,
                Icon = "nav_refresh"
            });
            return Task.FromResult(arg);
        }
    }
}
