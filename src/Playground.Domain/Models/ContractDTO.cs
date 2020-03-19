using System;
using System.Runtime.Serialization;

namespace Playground.Domain.Models
{
    /// <summary>
    /// Represents an instance of a contract.
    /// </summary>
    [DataContract]
    public class ContractDTO
    {
        /// <summary>
        /// The unique identifier for the contract.
        /// </summary>
        [DataMember(Order = 1)]
        public int Id { get; set; }

        /// <summary>
        /// The details of the contract.
        /// </summary>
        [DataMember(Order = 2)]
        public ContractDetailsDTO Details { get; set; }

        /// <summary>
        /// The timestamp for when the contract was created.
        /// </summary>
        [DataMember(Order = 3)]
        public DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// The username of of the user that created the contract.
        /// </summary>
        [DataMember(Order = 3)]
        public DateTimeOffset? CreatedBy { get; set; }

        /// <summary>
        /// The calculated book value for the contract.
        /// </summary>
        [DataMember(Order = 4)]
        public decimal BookValue { get; set; }
    }
}
