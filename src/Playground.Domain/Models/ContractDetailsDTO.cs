using System;
using ProtoBuf;

namespace Playground.Domain.Models
{
    /// <summary>
    /// The details of a contract.
    /// </summary>
    [ProtoContract]
    public class ContractDetailsDTO
    {
        /// <summary>
        /// The price of the contract.
        /// </summary>
        [ProtoMember(1)]
        public decimal Price { get; set; }

        /// <summary>
        /// The volume of the contract.
        /// </summary>
        [ProtoMember(2)]
        public decimal Volume { get; set; }

        /// <summary>
        /// The timestamp in UTC for when the contract was traded.
        /// </summary>
        [ProtoMember(3, DataFormat = DataFormat.WellKnown)]
        public DateTime? TradeTimestampUTC { get; set; }
    }
}
