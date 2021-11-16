namespace Alyas.Commerce.Plugin.BizFxOrdersEnhancement
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Pipelines;
    using Pipelines.Blocks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

    /// <summary>
    /// The configure sitecore class.
    /// </summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config

                .AddPipeline<IReProcessProblemOrdersPipeline, ReProcessProblemOrdersPipeline>(
                    configure =>
                    {
                        configure.Add<ReProcessProblemOrdersBlock>();
                    })
                .ConfigurePipeline<IPopulateEntityViewActionsPipeline>(configure =>
                {
                    configure.Add<PopulateRetryAllProblemOrdersViewActionsBlock>().After<PopulateOrdersDashboardViewActionsBlock>();
                    configure.Add<PopulateCompleteOrderActionBlock>().After<PopulateEntityViewActionsMasterBlock>();
                })
                .ConfigurePipeline<IDoActionPipeline>(configure =>
                {
                    configure.Add<DoActionRetryAllProblemOrdersBlock>().After<ValidateEntityVersionBlock>();
                    configure.Add<DoActionCompleteOrderBlock>().After<ValidateEntityVersionBlock>();
                }));
             
            services.RegisterAllCommands(assembly);
        }
    }
}