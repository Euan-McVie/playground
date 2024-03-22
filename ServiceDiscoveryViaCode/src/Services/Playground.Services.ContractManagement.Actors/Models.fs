namespace Playground.Services.ContractManagement.Actors

open System

/// Represents an instance of a contract.
type Contract = {
    /// The unique identifier for the contract.
    mutable id: int;
    /// The price of the contract.
    price: decimal;
    /// The volume of the contract.
    volume: decimal;
    /// The timestamp for when the contract was traded.
    tradeTimestamp: DateTimeOffset option;
}
