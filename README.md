# Migrated

This project [has been merged][5] with [AutoFixture][4].
It is now bundled with the [AutoFixture.AutoMoq][6] NuGet package as of version 3.20.0.

`AutoMoqBoostCustomization` has been renamed to `AutoConfiguredMoqCustomization`.

As part of the migration, a few bugs were fixed and features added:

* Fix: Crash when mocking a class with public non-overridable indexers.
* Fix: Crash when mocking a class with public non-overridable properties with private set accessors.
* Fix: Crash when mocking a class with public static members.
* Add: Mocks of concrete types will now have their public fields configured.


**Table of Contents**

- [AutoMoq.Boost](#user-content-automoqboost)
    - [Behind the curtains](#user-content-behind-the-curtains)
    - [NUnit and xUnit integration](#user-content-nunit-and-xunit-integration)
    - [Limitations](#user-content-limitations)

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

    //setup command
    var commandMock = fixture.Freeze<Mock<IDbCommand>>();
    commandMock.Setup(cmd => cmd.ExecuteReader())
               .Returns(readerMock.Object);

    //setup connection
    var connectionMock = fixture.Freeze<Mock<IDbConnection>>();
    connectionMock.Setup(conn => conn.CreateCommand())
                  .Returns(commandMock.Object);

    //act 
    var repo = fixture.Create<Repository<Person>>();
    var people = repo.SelectAll();

    //assert
    Assert.Equal(3, people.Count);
}
```

Using AutoMoq.Boost, you no longer have to tell mocks to return other mocks. The setup happens automatically. Thus, you can focus on setting up the members relevant to the test, e.g., the data reader.

```csharp
[Fact]
public void SelectAll_ReadsDataFromDatabase()
{
    var fixture = new Fixture().Customize(new AutoMoqBoostCustomization());

    //setup reader
    fixture.Freeze<Mock<IDataReader>>()
           .Setup(r => r.Read())
           .Returns(new Queue<bool>(true, true, true, false).Dequeue);

    //act 
    var repo = fixture.Create<Repository<Person>>();
    var people = repo.SelectAll();

    //assert
    Assert.Equal(3, people.Count);
}
```

Better yet, using [xUnit integration](#user-content-nunit-and-xunit-integration):

```csharp
[Theory, AutoMoqBoostData]
public void SelectAll_ReadsDataFromDatabase([Frozen] Mock<IDataReader> readerMock, Repository<Person> repo)
{
    readerMock.Setup(r => r.Read())
              .Returns(new Queue<bool>(true, true, true, false).Dequeue);

    //act 
    var people = repo.SelectAll();

    //assert
    Assert.Equal(3, people.Count);  
}
```

[Easy as pie][3].

## Behind the curtains

Specifically:

- If you're mocking an interface:
    - AutoMoq.Boost will setup all methods, indexers and properties with getters [(*)](#user-content-limitations)

    ```csharp
    mock.Setup(m => m.Method())
        .Returns(() => {
              var result = fixture.Create<TMember>();        //retrieve value from the fixture (lazily)
              mock.Setup(m => m.Method()).Returns(result);   //reuse this value the next time the member is invoked
              return result;
        });
    ```
    
    - Methods with "out" parameters will be set up like this:
    
    ```csharp
    var outParameter = fixture.Create<TOut>();
    mock.Setup(m => m.Method(out outParameter));
    ```
- If you're mocking a concrete/abstract class:
    - Abstract, virtual, non-sealed methods/indexers/properties with getters will be setup in a fashion similar to the above examples;
    - Sealed properties with setters will be set eagerly, like this:
    
    ```csharp
    mock.Object.Member = fixture.Create<TMember>()
    ```

As you can see, whenever possible, the dependency resolution is done lazily to allow circular dependencies, such as:

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

Also, due to a limitation of Moq, AutoMoq.Boost cannot setup methods with "ref" parameters.
If any such method is encountered, AutoMoq.Boost will quietly skip it.



(*) Note: A method belonging to a generic type isn't necessarily a "generic method". A method is only considered generic if it introduces generic type parameters of its own. For example, for the interface `IConverter<T>`, AutoMoq.Boost would be able to setup `string ConvertToString(T item)`, but not `U Convert<U>(T item)`


 [1]: http://blog.ploeh.dk/2010/08/19/AutoFixtureasanauto-mockingcontainer/
 [2]: https://github.com/AutoFixture/AutoFixture
 [3]: http://i.imgur.com/V8UVhWI.jpg
 [4]: https://github.com/AutoFixture/AutoFixture
 [5]: https://github.com/AutoFixture/AutoFixture/pull/302
 [6]: https://www.nuget.org/packages/AutoFixture.AutoMoq