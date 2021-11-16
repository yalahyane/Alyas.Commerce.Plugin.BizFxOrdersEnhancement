namespace Alyas.Commerce.Plugin.BizFxOrdersEnhancement.Pipelines.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Commands;
    using Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    public class DoActionCompleteOrderBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commander;
        public DoActionCompleteOrderBlock(CommerceCommander commander)
        {
            this._commander = commander;
        }
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");
            if (string.IsNullOrEmpty(entityView.Action) || !entityView.Action.Equals(context.GetPolicy<KnownAlyasOrderActionsPolicy>().CompleteOrder, StringComparison.OrdinalIgnoreCase))
                return entityView;

            var order = context.CommerceContext.GetObject((Func<Order, bool>)(o => o.Id.Equals(entityView.EntityId, StringComparison.OrdinalIgnoreCase)));
            if (order == null)
            {
                order = await this._commander.Pipeline<IFindEntityPipeline>().Run(new FindEntityArgument(typeof(Order), entityView.EntityId), context) as Order;
            }

            if (order == null)
            {
                return entityView;
            }

            await this._commander.Command<SetBulkOrderStatusCommand>().Process(context.CommerceContext,
                new List<string>() {entityView.EntityId}, "Completed");

            return entityView;
        }
    }
}
