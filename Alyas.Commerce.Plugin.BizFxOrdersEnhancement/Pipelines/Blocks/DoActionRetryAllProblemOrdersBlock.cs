namespace Alyas.Commerce.Plugin.BizFxOrdersEnhancement.Pipelines.Blocks
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Arguments;
    using Policies;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    public class DoActionRetryAllProblemOrdersBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commander;
        public DoActionRetryAllProblemOrdersBlock(CommerceCommander commander)
        {
            this._commander = commander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");
            if (string.IsNullOrEmpty(entityView.Action) || !entityView.Action.Equals(context.GetPolicy<KnownAlyasOrderActionsPolicy>().RetryAllProblemOrders, StringComparison.OrdinalIgnoreCase))
                return entityView;

            var problemOrders = await this._commander.Pipeline<IFindEntitiesInListPipeline>().Run(new FindEntitiesInListArgument(typeof(Order), context.GetPolicy<KnownOrderListsPolicy>().ProblemOrders, 0, int.MaxValue)
            {
                LoadEntities = false,
                LoadTotalItemCount = true
            }, context).ConfigureAwait(false);
            if (problemOrders?.List?.TotalItemCount > 0)
            {
                await this._commander.Pipeline<IReProcessProblemOrdersPipeline>().Run(new ReProcessProblemOrdersArgument { OrderIds = problemOrders.EntityReferences.Select(x=>x.EntityId).ToList() }, context.CommerceContext.PipelineContext);
            }

            return entityView;
        }
    }
}
