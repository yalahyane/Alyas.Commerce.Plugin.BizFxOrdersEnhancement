namespace Alyas.Commerce.Plugin.BizFxOrdersEnhancement.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.Orders;

    public class SetBulkOrderStatusCommand : CommerceCommand
    {
        private readonly ISetOrderStatusPipeline _setOrderStatusPipeline;
        private readonly IRemoveListEntitiesPipeline _removeListEntitiesPipeline;

        public SetBulkOrderStatusCommand(
            ISetOrderStatusPipeline setOrderStatusPipeline,
            IRemoveListEntitiesPipeline removeListEntitiesPipeline,
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this._setOrderStatusPipeline = setOrderStatusPipeline;
            this._removeListEntitiesPipeline = removeListEntitiesPipeline;
        }

        public virtual async Task<bool> Process(
            CommerceContext commerceContext,
            List<string> orderIds,
            string status)
        {
            var result = false;
            bool flag;
            using (CommandActivity.Start(commerceContext, this))
            {
                await this.PerformTransaction(commerceContext, async () =>
                {
                    foreach (var orderId in orderIds)
                    {
                        var pipelineContextOptions = commerceContext.PipelineContextOptions;
                        var listEntitiesArgument = new ListEntitiesArgument(new List<string>
                        {
                            orderId
                        }, commerceContext.GetPolicy<KnownOrderListsPolicy>().ProblemOrders);
                        await this._removeListEntitiesPipeline.Run(listEntitiesArgument, pipelineContextOptions);
                        result = await this._setOrderStatusPipeline.Run(new SetOrderStatusArgument(orderId, status), pipelineContextOptions);
                    }
                });
                flag = result;
            }
            return flag;
        }
    }
}
