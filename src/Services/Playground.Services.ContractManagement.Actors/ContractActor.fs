namespace Playground.Services.ContractManagement.Actors

open Akka.FSharp
open System
open Playground.Infrastructure.Repository.Interfaces
open Microsoft.Extensions.DependencyInjection
open Linq

/// Module containing behaviour for an actor that manages a single contract.
module internal ContractActor =    
    /// The commands that a contract actor responds to.
    type ContractCommand =
        /// Command to retrieve and return a contract to the sender.
        | GetContract
    
    /// The implementation of the contract actor.
    let private contractActor (serviceProvider: IServiceProvider) id (mailbox: Actor<_>) =
        let repository = serviceProvider.GetService<IRepository>()

        /// Load a contract from the repository.
        let loadContract (id: int) =
            let contracts = repository.ExecuteLinq<Contract>(fun query -> query.Where(fun c -> c.id = id))
            match Seq.length contracts with
            | 1 -> Some <| Seq.head contracts
            | _ -> None

        /// The main actor control loop.
        let rec loop contract = actor {
            let! message = mailbox.Receive ()
            match message with
            | GetContract -> 
                match contract with
                | Some c -> mailbox.Sender() <! Ok c
                | None -> 
                    match loadContract id with
                    | Some c -> 
                        mailbox.Sender() <! Ok c
                        return! loop <| Some c
                    | None -> mailbox.Sender() <! (Error <| sprintf "No Contract found with id %i" id)
            return! loop contract
        }
        loop None

    /// Creates an instance of a contract actor, under the provided contract manager, to manage a contract with the provided id.
    let CreateActor contractManager serviceProvider id = 
        spawn contractManager
        <| sprintf "Contract-%i" id
        <| contractActor serviceProvider id
