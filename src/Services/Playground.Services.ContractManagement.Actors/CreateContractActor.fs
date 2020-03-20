namespace Playground.Services.ContractManagement.Actors

open Akka.FSharp
open System
open Playground.Infrastructure.Repository.Interfaces
open Microsoft.Extensions.DependencyInjection

/// Module containing behaviour for an actor that creates a new contract.
module internal CreateContractActor = 
    /// The commands that a create contract actor responds to.
    type CreateContractCommand =
        /// Command to create a new contract from the provided contract record.
        | CreateContract of Contract 

    /// The implementation of the create contract actor.
    let private createContractActor (serviceProvider: IServiceProvider) (mailbox: Actor<_>) =
        let repository = serviceProvider.GetService<IRepository>()

        // The main actor control loop.
        let rec loop contract = actor {
            let! message = mailbox.Receive ()
            match message with
            | CreateContract c -> 
                match repository.TryCreate(c, fun r -> r.id ) with
                | (true, id) ->
                    mailbox.Sender() <! Ok id
                | (false, _) ->
                    mailbox.Sender() <! Failure "Failed to save contract"
            return! loop contract
        }
        loop None

    /// Creates an instance of a create contract actor under the provided contract manager.
    let CreateActor contractManager serviceProvider = 
        spawn contractManager 
        <| "CreateContract"
        <| createContractActor serviceProvider
