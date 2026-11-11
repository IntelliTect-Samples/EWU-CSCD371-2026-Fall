# Assignment

The purpose of this assignment is to solidify your learning of:

- Lambda expressions
- LINQ with Standard Query Operators
  - Selecting (projection)
  - Filtering
  - Aggregation
  - Sorting
  - Unit testing collections.
- Implementing IEnumerable

## Instructions

**Throughout, consider using the `System.Linq.Enumerable` methods `Zip`, `Count`, `Sort` and `Contains` methods for testing collections.**. (Preferably avoid using `Microsoft.VisualStudio.TestTools.UnitTesting.CollectionAssert` although that might be easier, to get a firmer grasp on additional LINQ API.)

1. Implement the `ISampleData.CsvRows` property, loading the data from the `People.csv` file and returning each line as a single string. ✔

   - Change the "Copy to" property on People.csv to "Copy if newer" so that the file is deployed along with your test project. ✔
   - Using LINQ, skip the first row in the `People.csv`. ✔
   - Be sure to appropriately handle resource (`IDisposable`) items correctly if applicable (and it may not be depending on how you implement it). ✔

2. Implement `IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()` to return a **sorted**, **unique** list of states. ❌✔

   - Use `ISampleData.CsvRows` for your data source. ✔
   - Don't forget the list should be unique. ✔
   - Sort the list alphabetically. ✔
   - Include a test that leverages a hardcoded list of addresses. ❌✔
   - Include a test that uses LINQ to verify the data is sorted correctly (do not use a hardcoded list). ❌✔

3. Implement `ISampleData.GetAggregateSortedListOfStatesUsingCsvRows()` to return a `string` that contains a **unique**, comma separated list of states. ❌✔

   - Use `ISampleData.GetUniqueSortedListOfStatesGivenCsvRows()` for your data source. ✔
   - Consider "selecting" only the states and calling `ToArray()` to retrieve an array of all the state names. ✔

4. Implement the `ISampleData.People` property to return all the items in `People.csv` as `Person` objects ❌✔

   - Use `ISampleData.CsvRows` as the source of the data. ✔
   - Sort the list by State, City, and Zip. ✔
   - Be sure that `Person.Address` is also populated. ✔
   - Adding null validation to all the `Person` and `Address` properties.❌
   - Consider using `ISampleData.CsvRows` in your test to verify your results.❌ ✔

5. Implement `ISampleDate.FilterByEmailAddress(Predicate<string> filter)` to return a list of names where the email address matches the `filter`. ❌✔

   - Use `ISampleData.People` for your data source. ❌✔

6. Implement `ISampleData.GetAggregateListOfStatesGivenPeopleCollection(IEnumerable<IPerson> people)` to return a `string` that contains a **unique**, comma-separated list of states. ✔

   - Use the `people` parameter from `ISampleData.People` property for your data source. ✔
   - At a minimum, use the `System.Linq.Enumerable.`Aggregate` LINQ method to create your result. ✔
   - Don't forget the list should be unique. ✔
   - It is recommended that, at a minimum, you use `ISampleData.GetUniqueSortedListOfStatesGivenCsvRows` to validate your result.

## Extra Credit

- Implement the homework using async/await and multi-threading by defining a new `SampleDataAsync` class that implements `IAsyncSampleData`). Refactor your `SampleData` and `SampleDataAsync` classes with minimal duplication. Be sure to refactor your tests to re-use a significant amount of the test code for both implementations. ❌✔

## Fundamentals

- Place all shared project properties into a `Directory.Build.Props` file.
- Place all shared project items into a `Directory.Build.targets` file.
- Ensure nullable reference types is enabled  ❌✔
- Ensure that you turn on code analysis for all projects(EnableNETAnalyzers)  ✔
- Set `LangVersion` and the `TargetFramework` to the latest released versions available (preview versions optional)   ✔
- and enabled .NET analyzers for both projects ✔
- For this assignment, consider using `Assert.AreEqual<T>()` (the generic version)  ❌✔
- All of the above should be unit tested ❌✔
- Choose simplicity over complexity ❌✔
- Make sure all exceptions are appropriately handled ❌✔

## See [Docs](Docs)
