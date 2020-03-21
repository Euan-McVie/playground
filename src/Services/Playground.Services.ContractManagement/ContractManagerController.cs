using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Playground.Domain.Models;
using Playground.Domain.Services;
using Playground.Infrastructure.Domain.Models;
using Playground.Infrastructure.Extensions.ServiceDiscovery.Attributes;

namespace Playground.Services.ContractManagement
{
    /// <summary>
    /// Controller to manage contracts.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [DiscoverableWebAPIService("Playground.ContractManager", "Playground", "Contract Management")]
    public class ContractManagerController : ControllerBase
    {
        private readonly IContractManager _contractManager;

        /// <summary>
        /// Construct the .Net API Controller for managing contracts.
        /// </summary>
        /// <param name="contractManager">The contract manager service.</param>
        public ContractManagerController(IContractManager contractManager)
        {
            if (contractManager is null)
                throw new ArgumentNullException(nameof(contractManager));
            _contractManager = contractManager;
        }

        /// <summary>
        /// Gets a contract with the provided <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The id of the contract to retrieve.</param>
        /// <returns>The contract with the provided id.</returns>
        /// <response code="200">OK with the <see cref="ContractDTO"/> contract.</response>
        /// <response code="404">Contract not found with a <see cref="string"/> error message.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ContractDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ContractDTO>> GetContract(int id)
        {
            ResultDTO<ContractDTO, string> result = await _contractManager.GetContractAsync(id).ConfigureAwait(false);

            return result switch
            {
                (true, ContractDTO contract, _) => Ok(contract),
                (false, _, string error) => NotFound(error)
            };
        }
    }
}
