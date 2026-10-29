# Assignment 5 - Directed Acyclic Graph

## Overview

Create a generic `Dag<T>` class that represents a **directed acyclic graph** (DAG). A DAG stores nodes and directed dependencies without allowing cycles. DAGs are useful for representing course prerequisites, build steps, and workflow ordering.

For an edge `dependsOn → dependent`, `dependent` cannot occur until `dependsOn` occurs first. Your implementation must reject an edge that would introduce a cycle.

## Reading

Reference the material on generics, collections, and graph traversal covered in class.

## Tasks

### Project setup

- Create a class library project named `Dag` in the `Dag` directory. ✔
- Add the `Dag` project to `Dag.slnx`. ✔
- Ensure the `Dag.Tests` project references the `Dag` project. ✔

### `Dag<T>`

- Create a generic `Dag<T>` class. ✔
- Implement `AddNode(T value)`. ✔
  - Add a node to the graph. ✔
  - Adding an existing node must not throw or add a duplicate. ✔
- Implement `AddDependency(T dependsOn, T dependent)`. ✔
  - Both nodes must already exist in the graph. ✔
  - Add the directed edge `dependsOn → dependent`. ✔
  - Do not add duplicate edges. ✔
  - Throw an `InvalidOperationException` if the edge would create a cycle. Use the message `"Adding this dependency would create a cycle."` ✔
- Implement `GetDependencies(T value)` to return the nodes that `value` **directly** depends on. ✔
- Implement `GetDependents(T value)` to return the nodes that **directly** depend on `value`. ✔
- For an edge `dependsOn → dependent`, `GetDependencies(dependent)` includes `dependsOn`, and `GetDependents(dependsOn)` includes `dependent`. ✔
- Choose and consistently apply an equality strategy for values of type `T`. ✔
- Keep the graph's internal collections encapsulated. ✔

### Demonstration and tests

- Demonstrate a fan-out/fan-in dependency graph using these course prerequisites: `CSCD 211 → CSCD 300`, `CSCD 211 → CSCD 320`, `CSCD 300 → CSCD 371`, and `CSCD 320 → CSCD 371`. CSCD 211 is a prerequisite for both intermediate courses, and both intermediate courses are prerequisites for CSCD 371. ❌✔
- Attempt to add `CSCD 371 → CSCD 211` and verify that an `InvalidOperationException` is thrown. ❌✔
- Unit test node and dependency addition, duplicate nodes and edges, cycle detection, and the results of `GetDependencies` and `GetDependents`. ❌✔
- Include tests for relevant boundary cases, such as querying or adding dependencies for nodes that are not in the graph. ❌✔

## Fundamentals

- Target all projects at .NET 10 (`net10.0`) and use the latest `LangVersion`.
- Enable nullable reference types for all projects. ❌✔
- Enable .NET analyzers and enforce code style during builds for all projects. ❌✔
- Ensure there are no build errors or warnings, including code analysis warnings. ❌✔
- Unit test the behavior required above. ❌✔
- Choose simplicity over complexity. ❌✔

## Extra Credit
- **Remove a node:** Add `RemoveNode(T value)` and remove all incoming and outgoing edges associated with that node. Verify that unrelated nodes and edges remain unchanged. ❌✔
- **Render as a flow chart:**: Add a method that can output the graph in [mermaid flow chart](https://mermaid.ai/open-source/syntax/flowchart.html) syntax. ❌✔
