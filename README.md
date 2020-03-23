# Playground

This repository contains a POC "playground" for investigating new technologies.

- [Concepts](#concepts)
- [Structure](#structure)
- [Pre-requisites](#pre-requisites)
- [Build Steps](#build-steps)
- [Try It](#try-it)
  - [Startup](#startup)
  - [Usage](#usage)
    - [gRPC Client - Console Application](#grpc-client---console-application)
    - [HTTP Client - Swagger UI](#http-client---swagger-ui)
    - [Service Discovery - Consul](#service-discovery---consul)
  - [Shutdown](#shutdown)

The [Developer Guide](docs/Developer's%20Guide.md) gives details on how to use the playground infrastructure to add your own services.

## Concepts

The following core concepts and technologies have been investigated and have at least a partial example of use:

- gRPC code first services using [protobuf-net.Grpc](https://protobuf-net.github.io/protobuf-net.Grpc/gettingstarted)
- [Web API apps](https://docs.microsoft.com/en-gb/aspnet/core/web-api/)
- Service discovery mechanism using [Consul](https://www.consul.io/discovery.html)
- [Swagger UI](https://swagger.io/tools/swagger-ui/) documented WebAPI endpoints
- [Akka.Net](https://getakka.net/) actors in F# called by C# service wrappers.

## Structure

The [src/Infrastructure](src/Infrastructure) folder contains the shareable lightweight infrastructure components. The contents of this folder could conceivably be moved into a separate repository to be consumed by individual projects.

It currently consists of:

- [Playground.Infrastructure.Domain](src/Infrastructure/Playground.Infrastructure.Domain) - contains helpers for creating your domain's models.
- [Playground.Infrastructure.Extensions.Akka](src/Infrastructure/Playground.Infrastructure.Extensions.Akka) - contains helpers for using [Akka.Net](https://getakka.net/) actors.
- [Playground.Infrastructure.Extensions.Console](src/Infrastructure/Playground.Infrastructure.Extensions.Console) - contains helpers to enable initialising a console application in a similar way to a web host.
- [Playground.Infrastructure.Extensions.Console.EasyConsole](src/Infrastructure/Playground.Infrastructure.Extensions.Console.EasyConsole) - contains helpers for using the [EasyConsole](https://github.com/jimtsikos/EasyConsole.Core) library to generate a basic menu system for your console application.
- [Playground.Infrastructure.Extensions.FSharp](src/Infrastructure/Playground.Infrastructure.Extensions.FSharp) - contains helpers for consuming F# from C#.
- [Playground.Infrastructure.Extensions.Grpc](src/Infrastructure/Playground.Infrastructure.Extensions.Grpc) - contains helpers for using gRPC.
- [Playground.Infrastructure.Extensions.ServiceDiscovery](src/Infrastructure/Playground.Infrastructure.Extensions.ServiceDiscovery) - contains helpers for enabling service discovery.
- [Playground.Infrastructure.Extensions.ServiceDiscovery.Consul](src/Infrastructure/Playground.Infrastructure.Extensions.ServiceDiscovery.Consul) - contains helpers for using [Consul](https://www.consul.io/discovery.html) as the service discovery provider.
- [Playground.Infrastructure.Extensions.Swagger](src/Infrastructure/Playground.Infrastructure.Extensions.Swagger) - contains helpers for using swagger to document WebAPI services.
- [Playground.Infrastructure.Repository](src/Infrastructure/Playground.Infrastructure.Repository) - contains helpers for interacting with a persistence store. It also contains a basic in-memory implementation for testing purposes.

The remaining projects provide a reference implementation. See the [Developer Guide](docs/Developer's%20Guide.md) for more details:

- [Playground.Domains.ContractManagement](src/Domains/Playground.Domains.ContractManagement) - An example domain defining models and a `Playground.ContractManager` service for working with trade contracts.
- [Playground.Services.ContractManagement](src/Services/Playground.Services.ContractManagement) - An example implementation of the `Playground.ContractManager` service using gRPC and WebAPI.
- [Playground.Services.ContractManagement.Actors](src/Services/Playground.Services.ContractManagement.Actors) - An example [Akka.Net](https://getakka.net/) actor system for working with trade contracts. Written in F#, it is used to implement the business logic for the example `Playground.ContractManager` service.
- [Playground.ServiceHost](src/Playground.ServiceHost) - An example service host configured to expose services over gRPC and WebAPI. It includes enabling Swagger UI as documentation for the WebAPI endpoints.
- [Playground.Domains.ContractManagement](src/Playground.TestClient) - An example of a consuming client for gRPC services. The mechanism used to call the remote services can also be used from within a second service host allowing interconnected calls between services hosted on each.

## Pre-requisites

The following pre-requisites should be installed and configured:

- VS 2019
- Docker [Install Guide](https://docs.docker.com/docker-for-windows/install/)
- Fork or Clone of this repository

## Build Steps

1. Open the solution [src/Playground.sln](/src/Playground.sln) in VS 2019.
2. Build it.

## Try It

These are the steps to try out the POC applications:

### Startup

1. Follow the [Build Steps](#build-steps) to build the applications.
2. Open a command prompt and navigate to the root directory.
3. Run `docker-compose up`
4. Start the `Playground.ServiceHost`

### Usage

#### gRPC Client - Console Application

The `Playground.TestClient` can be used to test the gRPC endpoint for the services.

When run it displays a simple menu structure allowing interaction with the gRPC services consumes.

#### HTTP Client - Swagger UI

You can review the configured HTTP endpoint for the services using the built in Swagger UI hosted by the `Playground.ServiceHost` at [https://localhost:5001/](https://localhost:5001/)

This can be used to both review the service definitions and also to try out calls to it using the built in test harness.

#### Service Discovery - Consul

If you would like to directly review the `Consul` service discovery registry then it can be accessed at [http://localhost:8500/](http://localhost:8500/)

Any services from the running `Playground.ServiceHost` will be listed.

### Shutdown

1. Close the applications.
2. Stop the docker services (`CTRL+c` in the docker-compose command prompt)
3. If uninstalling then run `docker-compose down -v`
