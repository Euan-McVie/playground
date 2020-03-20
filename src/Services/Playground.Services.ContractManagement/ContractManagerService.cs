using System;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.FSharp.Core;
using Playground.Domain.Models;
using Playground.Domain.Services;
using Playground.Infrastructure.Domain.Models;
using Playground.Infrastructure.Extensions.Akka;
using Playground.Infrastructure.Extensions.ServiceDiscovery.Attributes;
using Playground.Intrastructure.Extensions.FSharp;
using Playground.Services.ContractManagement.Actors;

namespace Playground.Services.ContractManagement
{
    /// <summary>
    /// Service implementation for contract management.
    /// </summary>
    [DiscoverableGrpcService("Playground.ContractManager", "Playground", "Contract Management")]
    public class ContractManagerService : IContractManager
    {
        private readonly IActorRef _contractManagerActor;

        /// <summary>
        /// Constructor for the contract manager Grpc service.
        /// </summary>
        /// <param name="contractManagerActorProvider">The provider that returns the contract manager actor. This should normally be injected via DI to ensure a single instance.</param>
        public ContractManagerService(ActorProvider<ContractManagerService> contractManagerActorProvider)
        {
            if (contractManagerActorProvider is null)
                throw new ArgumentNullException(nameof(contractManagerActorProvider));
            _contractManagerActor = contractManagerActorProvider();
        }

        /// <inheritdoc/>
        public async Task<ResultDTO<int, string>> CreateContractAsync(ContractDetailsDTO contractDetails)
        {
            if (contractDetails is null)
                throw new ArgumentNullException(nameof(contractDetails));

            object result = await _contractManagerActor
                .Ask(ContractManagerActor.ContractManagerCommand.NewCreateContract(
                    new Contract(
                        default,
                        contractDetails.Price,
                        contractDetails.Volume,
                        contractDetails.TradeTimestamp.ToFSharpOption())))
                .ConfigureAwait(false);

            FSharpResult<int, string> typedResult = result.ResultFromFSharp<int>();

            if (typedResult.IsOk)
                return ResultDTO.NewSuccessResult(typedResult.ResultValue);
            else
                return ResultDTO.NewErrorResult<int>(typedResult.ErrorValue);
        }

        /// <inheritdoc/>
        public async Task<ResultDTO<ContractDTO, string>> GetContractAsync(int id)
        {
            object result = await _contractManagerActor
                .Ask(ContractManagerActor.ContractManagerCommand.NewGetContract(id))
                .ConfigureAwait(false);

            FSharpResult<Contract, string> typedResult = result.ResultFromFSharp<Contract>();

            if (typedResult.IsOk)
                return ResultDTO.NewSuccessResult(
                    new ContractDTO
                    {
                        Id = typedResult.ResultValue.id,
                        Details = new ContractDetailsDTO
                        {
                            Price = typedResult.ResultValue.price,
                            Volume = typedResult.ResultValue.volume,
                            TradeTimestamp = typedResult.ResultValue.tradeTimestamp.ToNullable()
                        }
                    });
            else
                return ResultDTO.NewErrorResult<ContractDTO>(typedResult.ErrorValue);
        }
    }
}
