# Value Objects

## Considerations

- `record` vs `readonly record struct`:
  - `record` is passed by reference and stored on the heap.
  - `record struct` is passed by value and stored on the stack.
  - `record` supports inheritance, `record struct` does not.
  - `record struct` must have a primary constructor so can be created empty.
  - other normal struct vs class considerations like null handling, boxing etc.
- When to use a value object:
  - When you have simple domain concepts that are both the same primitive type.
  - When you want to make your code more readable and expressive.
    - Especially generic types e.g. What is the key for `IDictionary<string, IList<Participant>>`?
      <details><summary>Improvement</summary>
      IDictionary&lt;TenantIdentifier, IList&lt;Participant&gt;&gt;
      </details>
- When not to use a value object:
  - For all values everywhere. i.e. You don't need to create a value object for the intermediate values that never escape the method or class.
  - When creating a DTO with an external system. e.g. API, Database, etc.
- What behaviour is required:
  - C# `records` give initial `IEquatable`, `ToString`, `GetHashCode`, and `Deconstruct` for free.
  - Should they allow implicit conversion to the primitive type?
    <details><summary>My Proposal</summary>
    Generally yes, as it makes it easier to use when accessing the primitive for mapping or library functions that are not domain aware, etc.
    </details>
  - Should they allow implicit conversion from the primitive type?
    <details><summary>My Proposal</summary>
    Generally no, as it makes it easier to accidentally pass in an incorrect primitive.
    </details>
  - Should they allow implicit conversion to/from other value objects?
    <details><summary>My Proposal</summary>
    Generally no, as it makes it easier to accidentally pass in an incorrect value object.
    </details>
  - Should they expose their internal value (as a public property)?
    <details><summary>My Proposal</summary>
    Ideally no, as it avoids people adding `.Value` to unpack it.
    However, considerations need to be made for serialization, mapping of composite value objects etc.
    </details>
  - What is the expected `ToString` output?
    <details><summary>My Proposal</summary>
    For most values we should override the default `record` implementation and return the underlying values `ToString`.
    This avoids overly verbose output when debugging etc, but does mean that logs would not automatically show the type.
    </details>
  - Should they implement `IComparable`, `IComparable<T>`, `IFormattable<T>`, `IParsable<T>` etc.
    <details><summary>Proposal</summary>
    Comparable is useful for sorting and helps if using tools like verify that require deterministic ordering.
    Others, possibly not.
    </details>
  - Should they have math operators: `+`, `-`, `*`, `/`, etc.
    <details><summary>Proposal</summary>
    Only where it makes sense.
    </details>
  - Underlying methods on the underlying primitive type.
    <details><summary>Proposal</summary>
    Add as pass through only if required.
    </details>
  - Shareable extension methods. e.g. NodaTime custom operations etc.
    <details><summary>Proposal</summary>
    Leave against the base type and add a pass-through method to the value object.
    </details>

## Code to Demo

- Example of inheritance for shared behaviour.
  - `BaseId` + `ParticipantId`.
- Example of casting operators.
  - Calcs: `AvailableCapacityCalculator.TryCalculateAvailableUpCapacity()`
  - Mapping: `BidTimeSeriesDomainToPersistenceTransform.ToPersistenceBids()`
- Example of `ToString` override use.
  - `TimeSeriesPortfolioRepository.GetGeneratorTimeSeriesAsync()`
  - Simplify the output for debugging.
- Example of Maths implementation.
  - `BidVolume`
  - `MinimumBidVolumeFilter.ReallocateBidVolumes()`
- Example of `IComparable` implementation.
  - `Price`
- Example of `IEquatable` override implementation.
  - `Generator`
