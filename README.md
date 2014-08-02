# AutoMoq.Boost

AutoMoq.Boost is an extension of [AutoMoq][1] for [AutoFixture][2].

AutoMoq.Boost sets up mocks created by AutoMoq and instructs them to retrieve their dependencies from the fixture (i.e., the mocking container).
This makes it really easy to setup large complex dependency trees.

## The problem

AutoMoq lets you use AutoFixture as a mocking container. It uses [Moq][3] to resolve dependencies of abstract types (i.e., interfaces and abstract classes) by creating `Mock<T>` instances. This allows for very concise, readable and easy to maintain unit tests. Take a look at the following example using [Mark Seemann's `AutoMoqDataAttribute`][4].

```csharp
[Theory, AutoMoqData]
public void BakePizza_AlsoBakesIngredients([Frozen] IEnumerable<IIngredient> ingredients, Pizza pizza)
{
    pizza.Bake();

    foreach (var ingredient in ingredients)
        Mock.Get(ingredient).Verify(i => i.Bake(), Times.Once());
}
```

This gives us a concrete instance of pizza, composed with a few mocks of the `IIngredient` interface. Pretty neat.

The problem, however, arises when you have a large and complex tree of dependencies, such as the following `Repo` which depends on `IDbConnection`, which in turn creates instances of `IDbCommand`, whose `ExecuteScalar` method returns an `int`.


```csharp`
//System under test
public class PersonRepo
{
    private readonly Func<IDbConnection> _connFactory;
    public Repo(Func<IDbConnection> connFactory)
    {
        _connFactory = connFactory;
    }

    public int Count()
    {
        using (var conn = _connFactory())
        using (var cmd = conn.CreateCommand())
        {
            conn.Open();
            cmd.CommandText = "SELECT count(*) FROM employees";

            return (int)cmd.ExecuteScalar();
        }
    }
}
```

In a fashion similar to the first example, AutoMoq is able to give us a `PersonRepo`, composed with a delegate that returns a mock of `IDbConnection`. Because this mock is setup with `DefaultValue.Mock`, when `CreateCommand` is called, Moq will automatically create a mock of `IDbCommand` for us.

But what if we need to perform any additional setups on `IDbCommand`? What if we need to tell it to return "3" when `ExecuteScalar` is called? Well, we have to go old school and setup `IDbCommand.ExecuteScalar` *aaand* `IDbConnection.CreateCommand`. 

```csharp
[Theory, AutoMoqData]
public void Repo_ReturnsTheNumberOfPeople([Frozen] Mock<IDbCommand> cmdMock,
                                          [Frozen] Mock<IDbConnection> connMock,
                                          PersonRepo repo)
{
    cmdMock.Setup(c => c.ExecuteScalar())
            .Returns(3);

    connMock.Setup(c => c.CreateCommand())
            .Returns(cmdMock.Object);
    //act
    var result = repo.Count();

    //assert
    Assert.Equal(3, result);
}
```

Now imagine you had a chain with 4 mocks depending on each other. You'd have to chain those "Setup().Returns()" for all of them. Ugh!

Enter AutoMoq.Boost!


## The solution

Given a tree of dependencies, AutoMoq will fetch the topmost interfaces (e.g., `IDbConnection` in the example above) from the fixture, aka the mocking container. All further dependencies will now be resolved directly by Moq, and they're out of AutoFixture's control.

AutoMoq.Boost solves this issue by setting up all members of any mock created by AutoMoq so that their return values will be fetched from the fixture.

This way, to unit test the `PersonRepo`, all we need to setup is the behaviour that matters: `IDbCommand.CreateCommand`. Everything else becomes unnecessary, including those boring intermediate mocks.

```csharp
[Theory, AutoMoqBoostData]
public void Test([Frozen] Mock<IDbCommand> cmd, PersonRepo repo)
{
    cmd.Setup(c => c.ExecuteScalar())
        .Returns(3);

    Assert.Equal(3, repo.Count());
}
```
[Easy as pie][5].


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
 [3]: https://github.com/Moq/moq4
 [4]: http://blog.ploeh.dk/2010/10/08/AutoDataTheorieswithAutoFixture/
 [5]: http://i.imgur.com/V8UVhWI.jpg
 
