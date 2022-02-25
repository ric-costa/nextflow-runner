using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using NextflowRunner.Models;
using System.Threading.Tasks;

namespace NextflowRunner.Serverless.Functions;

public partial class ContainerManager
{
    private readonly ContainerConfiguration _containerConfig;
    private readonly Microsoft.Azure.Management.Fluent.IAzure _azure;
    private readonly NextflowRunnerContext _context;

    public ContainerManager(ContainerConfiguration containerConfig, NextflowRunnerContext context)
    {
        _context = context;
        _containerConfig = containerConfig;

        var msiLoginInformation = new MSILoginInformation(
                MSIResourceType.AppService,
                _containerConfig.ClientId,
                _containerConfig.ResourceId,
                _containerConfig.ObjectId);

        var credentials = SdkContext.AzureCredentialsFactory.FromMSI(
            msiLoginInformation,
            new AzureEnvironment(),
            _containerConfig.TenantId);

        _azure = Microsoft.Azure.Management.Fluent.Azure.Authenticate(credentials)
            .WithDefaultSubscription();
    }

    [FunctionName("ContainerManager")]
    public async Task RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var containerRunRequest = context.GetInput<ContainerRunRequest>();

        var containerGroupId = await context.CallActivityAsync<string>("ContainerManager_CreateContainer", containerRunRequest);

        await context.WaitForExternalEvent("ContainerManager_WebhookTrigger");

        await context.CallActivityAsync("ContainerManager_DestroyContainer", containerGroupId);
    }
}
