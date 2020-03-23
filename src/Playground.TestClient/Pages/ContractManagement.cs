using System;
using EasyConsoleCore;
using Playground.Domains.ContractManagement.Models;
using Playground.Domains.ContractManagement.Services;
using Playground.Infrastructure.Extensions.Grpc;

namespace Playground.TestClient
{
    class ContractManagement : MenuPage
    {
        public ContractManagement(RootMenu program)
            : base("Contract Management", program,
                  new Option("Create Contract", () => program.NavigateTo<CreateContract>()),
                  new Option("Get Contract", () => program.NavigateTo<GetContract>()))
        {

        }
    }

    class CreateContract : MenuPage
    {
        private readonly GrpcClient<IContractManager> _contractManagerClient;

        public CreateContract(RootMenu program, GrpcClient<IContractManager> contractManagerClient)
            : base("Create Contract", program,
                  new Option("Create Another", () => program.NavigateTo<CreateContract>()))
        {
            _contractManagerClient = contractManagerClient;
        }

        public override void Display()
        {
            string priceString = Input.ReadString("Enter Price: ");
            string volumeString = Input.ReadString("Enter Volume: ");

            if (!decimal.TryParse(priceString, out decimal price))
                Output.WriteLine(ConsoleColor.Red, $"Invalid price '{priceString}'");
            if (!decimal.TryParse(volumeString, out decimal volume))
                Output.WriteLine(ConsoleColor.Red, $"Invalid volume '{volumeString}'");

            (bool success, int id, string message) = _contractManagerClient
                .CallAsync(service =>
                    service.CreateContractAsync(
                        new ContractDetailsDTO
                        {
                            Price = price,
                            Volume = volume
                        }))
                .Result;

            if (success)
                Output.WriteLine(ConsoleColor.Green, $"Successfully saved new contract with id: {id}");
            else
                Output.WriteLine(ConsoleColor.Red, $"Failed to save new contract with error: {message}");

            base.Display();
        }
    }

    class GetContract : MenuPage
    {
        private readonly GrpcClient<IContractManager> _contractManagerClient;

        public GetContract(RootMenu program, GrpcClient<IContractManager> contractManagerClient)
            : base("Get Contract", program,
                  new Option("Get Another", () => program.NavigateTo<GetContract>()))
        {
            _contractManagerClient = contractManagerClient;
        }

        public override void Display()
        {
            string idString = Input.ReadString("Enter Id: ");

            if (!int.TryParse(idString, out int id))
                Output.WriteLine(ConsoleColor.Red, $"Invalid id '{idString}'");

            (bool success, ContractDTO contract, string message) = _contractManagerClient
                .CallAsync(service => service.GetContractAsync(new SingleContractRequestDTO { Id = id }))
                .Result;
            if (success)
                Output.WriteLine(ConsoleColor.Green, $"Successfully found contract:{Environment.NewLine}{contract}");
            else
                Output.WriteLine(ConsoleColor.Red, $"Failed to find contract with error: {message}");

            base.Display();
        }
    }
}
