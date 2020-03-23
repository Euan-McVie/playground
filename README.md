# Playground

This repository contains a POC "playground" for investigating new technologies.

- [Pre-requisites](#pre-requisites)
- [Build Steps](#build-steps)
- [Try It](#try-it)
  - [Startup](#startup)
  - [Usage](#usage)
    - [gRPC Client - Console Application](#grpc-client---console-application)
    - [HTTP Client - Swagger UI](#http-client---swagger-ui)
    - [Service Discovery - Consul](#service-discovery---consul)
  - [Shutdown](#shutdown)

## Pre-requisites

The following pre-requisites should be installed and configured:

- VS 2019
- Docker [Install Guide](https://docs.docker.com/docker-for-windows/install/)
- Fork or Clone this repository

## Build Steps

1. Open the solution [src/Playground.sln](/src/Playground.sln) in VS 2019.
2. Build

## Try It

These are the steps to try out the POC applications

### Startup

1. Follow the [Build Steps](#build-steps) to build the applications.
2. Open a command prompt and navigate to the root directory.
3. Run `docker-compose up`
4. Start the `Playground.ServiceHost`

### Usage

#### gRPC Client - Console Application

The `Playground.TestClient` can be used to test the gRPC endpoint for the services.
When run it will display a simple menu structure allowing interaction with the gRPC services it has can consume.

#### HTTP Client - Swagger UI

You can review the configured HTTP endpoint for the services using the built in Swagger UI hosted by the `Playground.ServiceHost` at [https://localhost:5001/](https://localhost:5001/)

This can be used to both review the service definitions and also to try out calls to it using the built in test harness.

#### Service Discovery - Consul

If you would like to directly review the `Consul` service discovery registry then it can be accessed at [http://localhost:8500/](http://localhost:8500/)

Any services from the running `Playground.ServiceHost` should be listed.

### Shutdown

1. Close the applications.
2. Stop the docker services (`CTRL+c` in the docker-compose command prompt)
3. If uninstalling then run `docker-compose down -v`
