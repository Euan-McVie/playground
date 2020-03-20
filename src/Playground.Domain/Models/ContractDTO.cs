using System;
using ProtoBuf;

namespace Playground.Domain.Models
{
    /// <summary>
    /// Represents an instance of a contract.
    /// </summary>
    [ProtoContract]
    public class ContractDTO
    {
        /// <summary>
        /// The unique identifier for the contract.
        /// </summary>
        [ProtoMember(1)]
        public int Id { get; set; }

        /// <summary>
        /// The details of the contract.
        /// </summary>
        [ProtoMember(2)]
        public ContractDetailsDTO Details { get; set; }

        /// <summary>
        /// The timestamp for when the contract was created.
        /// </summary>
        [ProtoMember(3, DataFormat = DataFormat.WellKnown)]
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// The username of of the user that created the contract.
        /// </summary>
        [ProtoMember(4)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// The calculated book value for the contract.
        /// </summary>
        [ProtoMember(5)]
        public decimal BookValue { get; set; }
    }
}
