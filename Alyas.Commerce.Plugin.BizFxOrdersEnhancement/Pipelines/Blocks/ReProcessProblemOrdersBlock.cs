namespace Alyas.Commerce.Plugin.BizFxOrdersEnhancement.Pipelines.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Arguments;
    using Microsoft.Extensions.Logging;
    using Models;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Framework.Pipelines;

    public class ReProcessProblemOrdersBlock : PipelineBlock<ReProcessProblemOrdersArgument, ReProcessProblemOrdersResult, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        public ReProcessProblemOrdersBlock(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<ReProcessProblemOrdersResult> Run(ReProcessProblemOrdersArgument arg, CommercePipelineExecutionContext context)
        {
            var result = new ReProcessProblemOrdersResult();
            try
            {
                var findEntitiesArgument = new FindEntitiesInListArgument(typeof(Order),
                    context.GetPolicy<KnownOrderListsPolicy>().ProblemOrders, 0, int.MaxValue);
                var problemOrderIds = arg.OrderIds.Any()
                    ? arg.OrderIds
                    : (await this._commerceCommander.Pipeline<IFindEntitiesInListPipeline>()
                        .Run(findEntitiesArgument, context).ConfigureAwait(false)).EntityReferences
                    .Select(x => x.EntityId);
                foreach (var orderId in problemOrderIds)
                {
                    try
                    {
                        var pipelineContextOptions = context.CommerceContext.PipelineContextOptions;
                        var listEntitiesArgument = new ListEntitiesArgument(new List<string>
                        {
                            orderId
                        }, context.GetPolicy<KnownOrderListsPolicy>().ProblemOrders);
                        await this._commerceCommander.Pipeline<IRemoveListEntitiesPipeline>().Run(listEntitiesArgument, pipelineContextOptions);
                        await this._commerceCommander.Pipeline<ISetOrderStatusPipeline>().Run(new SetOrderStatusArgument(orderId, context.GetPolicy<KnownOrderStatusPolicy>().Pending), pipelineContextOptions);
                        
                        result.SuccessfulOrders.Add(orderId);
                    }
                    catch (Exception ex)
                    {
                        result.Status = "Completed with Errors";
                        result.FailedOrders.Add(new FailedOrder{OrderId = orderId, ErrorMessage = ex.Message});
                        context.Logger.LogError(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                result.Status = "Error";
                result.ErrorMessage = ex.Message;
                context.Logger.LogError(ex.ToString());
            }

            return result;
        }
    }
}
