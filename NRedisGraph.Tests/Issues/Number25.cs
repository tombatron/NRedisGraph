using System.Collections.Generic;
using StackExchange.Redis;
using Xunit;

namespace NRedisGraph.Tests.Issues;

public class Number25 : BaseTest
{
    private ConnectionMultiplexer _readWriteMultiplexer;
    private ConnectionMultiplexer _readonlyMultiplexer;
    
    private RedisGraph _readonlyApi;
    private RedisGraph _readWriteApi;
    
    protected override void BeforeTest()
    {
        // Setup the multiplexers.
        _readonlyMultiplexer = ConnectionMultiplexer.Connect("tombaserver.local:10000,tombaserver.local:10001");
        _readWriteMultiplexer = ConnectionMultiplexer.Connect("tombaserver.local:10000,tombaserver.local:10001");
        
        // Setup RedisGraph API...
        _readonlyApi = new RedisGraph(_readonlyMultiplexer.GetDatabase(0));
        _readWriteApi = new RedisGraph(_readWriteMultiplexer.GetDatabase(0));
        
        // Setup the data.
        _readWriteApi.GraphQuery("issue25", "CREATE (:Person {username: 'John', age: 20})");
        _readWriteApi.GraphQuery("issue25", "CREATE (:Person {username: 'Peter', age: 23})");
        _readWriteApi.GraphQuery("issue25", "CREATE (:Person {username: 'Steven', age: 30})");        
    }

    protected override void AfterTest()
    {
        _readWriteApi.DeleteGraph("issue25");
        _readWriteMultiplexer.Dispose();

        _readonlyMultiplexer.Dispose();
    }

    [Fact(Skip="Need a read only node...")]
    public void IssueReproduction()
    {
        // Read from the read-only replica
        string readQuery = "MATCH x=(p:Person) RETURN nodes(x) as nodes";

        ResultSet records = _readonlyApi.GraphReadOnlyQuery("issue25", readQuery);

        var result = new List<Person>();

        foreach (Record record in records) // Here it throws the exception (On the iteration)
        {
            var nodes = record.GetValue<object[]>("nodes");

            foreach (var node in nodes)
            {
                if (node is Node castedNode)
                {
                    Person person = GetPersonInfo(castedNode);
                    
                    result.Add(new Person(person.Name, person.Age));
                }
            }
        }
    }

    private static Person GetPersonInfo(Node node)
    {
        var name = node.PropertyMap["username"];
        var age = node.PropertyMap["age"];

        return new Person(name.Value.ToString(), (long)age.Value);
    }
}

public class Person
{
    public string Name { get; }
    
    public long Age { get; }

    public Person(string name, long age)
    {
        Name = name;
        Age = age;
    }
}