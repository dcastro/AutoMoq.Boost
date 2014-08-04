# AutoMoq.Boost

AutoMoq.Boost is an extension of [AutoMoq][1] for [AutoFixture][2].

AutoMoq.Boost sets up mocks created by AutoMoq and instructs them to retrieve their dependencies from the fixture (i.e., the mocking container).
This makes it really easy to setup large complex dependency trees.

Take this example, written using AutoMoq for AutoFixture and xUnit:

```csharp
[Fact]
public void SelectAll_ReadsDataFromDatabase()
{
    var fixture = new Fixture().Customize(new AutoMoqCustomization());

    //setup reader
    var readerMock = fixture.Freeze<Mock<IDataReader>>();
    readerMock.Setup(r => r.Read())
              .Returns(new Queue<bool>(true, true, true, false).Dequeue);

    readerMock.Setup(r => r["Id"])
              .Returns(new Queue<int>(1, 2, 3).Dequeue);

    //setup command
    var commandMock = fixture.Freeze<Mock<IDbCommand>>();
    commandMock.Setup(cmd => cmd.ExecuteReader())
               .Returns(readerMock.Object);

    //setup connection
    var connectionMock = fixture.Freeze<Mock<IDbConnection>>();
    connectionMock.Setup(conn => conn.CreateCommand())
                  .Returns(commandMock.Object);

    //act 
    var repo = fixture.Create<Repository<Employee>>();
    var people = repo.SelectAll();

    //assert
    Assert.Equal(3, people.Count);
    Assert.Equal(new[]{1,2,3}, people.Select(p => p.Id));
}
```

Using AutoMoq.Boost, you no longer have to tell mocks to return other mocks. The setup happens automatically. Thus, you can focus on setting up the members relevant to the test, e.g., the data reader.

```csharp
[Fact]
public void SelectAll_ReadsDataFromDatabase()
{
    var fixture = new Fixture().Customize(new AutoMoqBoostCustomization());

    //setup reader
    var readerMock = fixture.Freeze<Mock<IDataReader>>();
    readerMock.Setup(r => r.Read())
              .Returns(new Queue<bool>(true, true, true, false).Dequeue);

    readerMock.Setup(r => r["Id"])
              .Returns(new Queue<int>(1, 2, 3).Dequeue);

    //act 
    var repo = fixture.Create<Repository<Employee>>();
    var people = repo.SelectAll();

    //assert
    Assert.Equal(3, people.Count);
    Assert.Equal(new[]{1,2,3}, people.Select(p => p.Id));
}
```

Better yet, using [xUnit integration](#user-content-nunit-and-xunit-integration):

```csharp
[Theory, AutoMoqBoostData]
public void SelectAll_ReadsDataFromDatabase([Frozen] Mock<IDataReader> readerMock, Repository<Employee> repo)
{
    readerMock.Setup(r => r.Read())
              .Returns(new Queue<bool>(true, true, true, false).Dequeue);

    readerMock.Setup(r => r["Id"])
              .Returns(new Queue<int>(1, 2, 3).Dequeue);

    //act 
    var people = repo.SelectAll();

    //assert
    Assert.Equal(3, people.Count);
    Assert.Equal(new[]{1,2,3}, people.Select(p => p.Id));    
}
```

[Easy as pie][3].

## Behind the curtains

Specifically:

- If you're mocking an interface, AutoMoq.Boost will setup all methods and properties with getters [(*)](#user-content-limitations)
  ```csharp
    mock.Setup(m => m.Member())
          .Returns(() => {
              var result = fixture.Create<TMember>();       //retrieve value from the fixture (lazily)
              mock.Setup(m => m.Member().Returns(result);   //reuse this value the next time the member is invoked
              return result;
          });
    ```
- If you're mocking a concrete/abstract class:
    - Abstract, virtual, non-sealed methods/properties with getters will be setup in a fashion similar to the above example
    - Sealed properties with setters will be set eagerly, like this:
      ```csharp
      mock.Object.Member = fixture.Create<TMember>();
      ```

As you can see, the dependency resolution is done lazily to allow circular dependencies, such as:

```csharp
public interface IPizza
{
    IPizza Clone();
}

```




## NUnit and xUnit integration

In order to have your data injected into a test method, instead of having to retrieve them from a fixture, you can use the `AutoMoqBoostData` attribute found in either AutoMoq.Boost.NUnit or AutoMoq.Boost.xUnit.

## Limitations

Currently, AutoMoq.Boost is not able to automatically setup generic methods(*).

However, the extension method `ReturnsUsingFixture` makes it really easy to set those up.

```csharp
converter.Setup(x => x.Convert<double>("10.0"))
         .ReturnsUsingFixture(fixture);
```




AutoMoq.Boost is also not able to mock `ref` and `out` parameters.


(*) Note: A method belonging to a generic type isn't necessarily a "generic method". A method is only considered generic if it introduces generic type parameters of its own. For example, for the interface `IConverter<T>`, AutoMoq.Boost would be able to setup `string ConvertToString(T item)`, but not `U Convert<U>(T item)`


 [1]: http://blog.ploeh.dk/2010/08/19/AutoFixtureasanauto-mockingcontainer/
 [2]: https://github.com/AutoFixture/AutoFixture
 [3]: http://i.imgur.com/V8UVhWI.jpg
 
