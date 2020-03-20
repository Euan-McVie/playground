namespace Playground.Services.ContractManagement.Actors

open Akka.FSharp
open System.Collections.Generic
open Akka.Actor

/// Module containing behaviour for an actor that manages contracts.
module ContractManagerActor =
    /// The commands that a contract manager actor responds to.
    type ContractManagerCommand =
        /// Command to create a new contract from the provided contract record.
        | CreateContract of Contract
        /// Command to retrieve and return a contract with the provided id to the sender.
        | GetContract of Id: int

    /// The implementation of the contract manager actor.
    let private contractManagerActor serviceProvider (mailbox: Actor<obj>) = 
        let contractIdToActor = Dictionary<int,IActorRef>()
        let actorToContractId = Dictionary<IActorRef,int>()
        let createContractActor = CreateContractActor.CreateActor mailbox serviceProvider

        /// Get a contract actor to manage a contract with the provided id. If no actor currently exists then create one.
        let getOrCreateContractActor id =
            match contractIdToActor.TryGetValue id with
            | (true, actor) -> actor
            | (false, _) ->
                let actor = ContractActor.CreateActor mailbox serviceProvider id
                monitor actor mailbox |> ignore
                contractIdToActor.Add(id, actor)
                actorToContractId.Add(actor, id)
                actor

        // The main actor control loop.
        let rec loop () = actor {
            let! message = mailbox.Receive ()
            match box message with
            | :? ContractManagerCommand as command ->
                match command with 
                | CreateContract c as m -> createContractActor.Forward <| CreateContractActor.CreateContract c
                | GetContract id -> (getOrCreateContractActor id).Forward ContractActor.GetContract
            | :? Terminated as t -> 
                let contractId = actorToContractId.Item t.ActorRef
                actorToContractId.Remove t.ActorRef |> ignore
                contractIdToActor.Remove contractId |> ignore
            | _ -> mailbox.Unhandled()
            return! loop ()
        }
        loop ()

    /// Creates an instance of a contract manager actor under the provided actor system.
    let CreateActor system serviceProvider = spawn system "ContractManager" (contractManagerActor serviceProvider)
