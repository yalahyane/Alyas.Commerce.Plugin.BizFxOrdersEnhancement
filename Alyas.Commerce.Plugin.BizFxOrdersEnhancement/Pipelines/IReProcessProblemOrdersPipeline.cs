namespace Alyas.Commerce.Plugin.BizFxOrdersEnhancement.Pipelines
{
    using Arguments;
    using Models;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;

    public interface IReProcessProblemOrdersPipeline : IPipeline<ReProcessProblemOrdersArgument, ReProcessProblemOrdersResult, CommercePipelineExecutionContext>
    {
    }
}
