using EasyConsoleCore;
using Playground.Domains.ContractManagement.Services;
using Playground.Infrastructure.Extensions.Grpc;

namespace Playground.TestClient
{
    /// <summary>
    /// The root menu for the console application.
    /// </summary>
    public class RootMenu : Program
    {
        /// <summary>
        /// Constructor to create the menu system for the console application.
        /// </summary>
        /// <param name="contractManagerClient">The contract manager gRPC client to use when interacting with contracts.</param>
        public RootMenu(GrpcClient<IContractManager> contractManagerClient)
            : base("Test Client", breadcrumbHeader: true)
        {
            AddPage(new MainMenu(this));
            AddPage(new ContractManagement(this));
            AddPage(new CreateContract(this, contractManagerClient));
            AddPage(new GetContract(this, contractManagerClient));

            SetPage<MainMenu>();
        }
    }

    class MainMenu : MenuPage
    {
        public MainMenu(Program program)
            : base("Test Areas", program,
                  new Option("Contract Management", () => program.NavigateTo<ContractManagement>()))
        {
        }
    }
}
