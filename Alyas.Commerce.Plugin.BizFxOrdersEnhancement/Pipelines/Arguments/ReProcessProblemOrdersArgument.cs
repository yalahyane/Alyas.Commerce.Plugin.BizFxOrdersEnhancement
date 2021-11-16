namespace Alyas.Commerce.Plugin.BizFxOrdersEnhancement.Pipelines.Arguments
{
    using System.Collections.Generic;
    using Sitecore.Commerce.Core;

    public class ReProcessProblemOrdersArgument : PipelineArgument
    {
        public List<string> OrderIds { get; set; } = new List<string>();
    }
}
