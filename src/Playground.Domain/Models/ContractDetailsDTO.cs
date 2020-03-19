using System;
using System.Runtime.Serialization;

namespace Playground.Domain.Models
{
    /// <summary>
    /// The details of a contract.
    /// </summary>
    [DataContract]
    public class ContractDetailsDTO
    {
        /// <summary>
        /// The price of the contract.
        /// </summary>
        [DataMember(Order = 1)]
        public decimal Price { get; set; }

        /// <summary>
        /// The volume of the contract.
        /// </summary>
        [DataMember(Order = 2)]
        public decimal Volume { get; set; }

        /// <summary>
        /// The timestamp for when the contract was traded.
        /// </summary>
        [DataMember(Order = 3)]
        public DateTimeOffset? TradeTimestamp { get; set; }
    }
}
