using ProtoBuf;

namespace Playground.Domains.ContractManagement.Models
{
    /// <summary>
    /// A request for a single contract.
    /// </summary>
    [ProtoContract]
    public class SingleContractRequestDTO
    {
        /// <summary>
        /// List of <see cref="ContractDTO.Id"/> to filter on. An empty list will match all contracts.
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }
    }
}
