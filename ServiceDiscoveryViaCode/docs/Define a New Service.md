# Define a New Service

Follow these steps to define your new service.

A reference implementation can be found at [Playground.Domains.ContractManagement](../src/Domains/Playground.Domains.ContractManagement)

- [Create a new Domain project](#create-a-new-domain-project)
- [Create DTO request and response models](#create-dto-request-and-response-models)
- [Create a Service Definition](#create-a-service-definition)

## Create a new Domain project

If your service does not fit into an existing domain then follow these steps to create the new project:

1. Add a new `Class Library (.NET Core)` project to the `Domains` folder. It is recommended that the name should be `Playground.Domains.{New Domain}`. Note that the domain name should not match the service name you intend to use.

   e.g. If your service will be called `IExampleManager` then your domain could be `Playground.Domains.IExampleManagement`.
2. Add project references to:
   - `Playground.Infrastructure.Extensions.ServiceDiscovery` - contains extensions to enable your service to be discovered without knowledge of its location.
   - `Playground.Infrastructure.Domain` - contains helpers for creating your domain's models.
3. If the services will use gRPC then add nuget package references to:
   - `protobuf-net.Grpc` - contains the code first gRPC implementation.
4. Create `Models` and `Services` subfolders.

## Create DTO request and response models

For each operation create the appropriate DTO models for the payloads.

It is recommended to use the `ResultDTO<TResult,TError>` from the `Playground.Infrastructure.Domain` library as the response wrapper and so the result DTO need only represent a successful result.

Even if the request parameter will be a single primitive it must be wrapped in a request DTO containing a single property.

1. Add a new `public class {New Request/Response}DTO`.
2. Apply the `ProtoBuf.ProtoContract` attribute to designate the model as protocol buffer serializable.

   ```cs
   [ProtoContract]
   public class ExampleDTO
   { ... }
   ```

3. Create standard C# properties for the DTO. Properties may be read only `{ get; }`. If the read only properties are set using a parameterised constructor then a private parameterless constructor must also be defined.
4. Apply the `ProtoBuf.ProtoMember` attribute to each property giving them a unique `int` tag. For backwards compatibility this tag number should never be changed or reused once given to a property and does not need to be consecutive.

   Other DTOs can be used as child properties and should be defined in the same manner.

   ```cs
   [ProtoMember(2)]
   public ExampleDetailsDTO Details { get; set; }
   ```

   If defining a `DateTime` or `TimeSpan` add a `DataFormat = DataFormat.WellKnown` flag to the attribute. This specifies that the "well known" standardized representation defined by Google should be used.

   ```cs
   [ProtoMember(3, DataFormat = DataFormat.WellKnown)]
   public DateTime CreatedAtUTC { get; set; }
   ```

## Create a Service Definition

1. Add a new `public interface I{New Service}` to the `Services` folder.
2. For a gRPC service then:
   - Apply the `ProtoBuf.Grpc.Configuration.Service` attribute to your `I{New Service}` interface giving the service it's gRPC service name. e.g. `Playground.{New Service}`
   - Apply the `Playground.Infrastructure.Extensions.ServiceDiscovery.Attributes.DiscoverableGrpcService` attribute to your `I{New Service}` giving the service its discoverable name and any additional tags to apply to the service to describe it. It is recommended that the discoverable name is set to match the name given in the `Service` attribute.

   ```cs
   [Service("Playground.ExampleManager")]
   [DiscoverableGrpcService("Playground.ExampleManager", "Playground", "Management")]
   public interface IExampleManager
   { ... }
   ```

3. For a WebAPI service then:
   - Apply the `Playground.Infrastructure.Extensions.ServiceDiscovery.Attributes.DiscoverableWebAPIService` attribute to your `I{New Service}` giving the service its discoverable name and any additional tags to apply to the service to describe it. It is recommended that if the service is being exposed using both WebAPI and gRPC then the names defined in the attributes should match.

   ```cs
   [DiscoverableWebAPIService("Playground.ExampleManager", "Playground", "Management")]
   public interface IExampleManager
   { ... }
   ```

4. Define the operations available on your service as standard C# methods. It is recommended that the methods should return a `Task<ResultDTO<TResult, TError>>`.
   - Returning a `Task` enables asynchronous calling.
   - Using the `ResultDTO<TResult,TError>` from the `Playground.Infrastructure.Domain` library provides a consistent, shared mechanism for returning a successful `TResult` DTO model separately from any `TError` model.
5. Apply the `ProtoBuf.Grpc.Configuration.Operation` attribute, optionally setting the gRPC name for the operation.

   ```cs
   [Operation]
   Task<ResultDTO<ExampleDTO, string>> GetExampleAsync(SingleExampleRequestDTO request);
   ```
