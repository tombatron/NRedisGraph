// .NET port of https://github.com/RedisGraph/JRedisGraph
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using static NRedisGraph.Header;
using static NRedisGraph.Statistics;

namespace NRedisGraph.Tests
{
    public class RedisGraphAPITest : BaseTest
    {
        private ConnectionMultiplexer _muxr;
        private RedisGraph _api;

        public RedisGraphAPITest() : base() { }

        protected override void BeforeTest()
        {
            _muxr = ConnectionMultiplexer.Connect("localhost");

            _muxr.GetDatabase().Execute("FLUSHDB");

            _api = new RedisGraph(_muxr.GetDatabase(0));
        }

        protected override void AfterTest()
        {
            _api = null;
            _muxr.Dispose();
            _muxr = null;
        }

        [Fact]
        public void TestCreateNode()
        {
            // Create a node    	
            ResultSet resultSet = _api.Query("social", "CREATE ({name:'roi',age:32})");

            Assert.Equal(1, resultSet.Statistics.NodesCreated);
            Assert.Null(resultSet.Statistics.GetStringValue(Label.NodesDeleted));
            Assert.Null(resultSet.Statistics.GetStringValue(Label.RelationshipsCreated));
            Assert.Null(resultSet.Statistics.GetStringValue(Label.RelationshipsDeleted));
            Assert.Equal(2, resultSet.Statistics.PropertiesSet);
            Assert.NotNull(resultSet.Statistics.GetStringValue(Label.QueryInternalExecutionTime));

            Assert.Empty(resultSet);
        }

        [Fact]
        public void TestCreateLabeledNode()
        {
            // Create a node with a label
            ResultSet resultSet = _api.Query("social", "CREATE (:human{name:'danny',age:12})");
            Assert.Empty(resultSet);
            Assert.Equal("1", resultSet.Statistics.GetStringValue(Label.NodesCreated));
            Assert.Equal("2", resultSet.Statistics.GetStringValue(Label.PropertiesSet));
            Assert.NotNull(resultSet.Statistics.GetStringValue(Label.QueryInternalExecutionTime));
        }

        [Fact]
        public void TestConnectNodes()
        {
            // Create both source and destination nodes
            Assert.NotNull(_api.Query("social", "CREATE (:person{name:'roi',age:32})"));
            Assert.NotNull(_api.Query("social", "CREATE (:person{name:'amit',age:30})"));

            // Connect source and destination nodes.
            ResultSet resultSet = _api.Query("social", "MATCH (a:person), (b:person) WHERE (a.name = 'roi' AND b.name='amit')  CREATE (a)-[:knows]->(a)");

            Assert.Empty(resultSet);
            Assert.Null(resultSet.Statistics.GetStringValue(Label.NodesCreated));
            Assert.Null(resultSet.Statistics.GetStringValue(Label.PropertiesSet));
            Assert.Equal(1, resultSet.Statistics.RelationshipsCreated);
            Assert.Equal(0, resultSet.Statistics.RelationshipsDeleted);
            Assert.NotNull(resultSet.Statistics.GetStringValue(Label.QueryInternalExecutionTime));
        }

        [Fact]
        public void TestDeleteNodes()
        {
            Assert.NotNull(_api.Query("social", "CREATE (:person{name:'roi',age:32})"));
            Assert.NotNull(_api.Query("social", "CREATE (:person{name:'amit',age:30})"));
            ResultSet deleteResult = _api.Query("social", "MATCH (a:person) WHERE (a.name = 'roi') DELETE a");

            Assert.Empty(deleteResult);
            Assert.Null(deleteResult.Statistics.GetStringValue(Label.NodesCreated));
            Assert.Null(deleteResult.Statistics.GetStringValue(Label.PropertiesSet));
            Assert.Null(deleteResult.Statistics.GetStringValue(Label.RelationshipsCreated));
            Assert.Null(deleteResult.Statistics.GetStringValue(Label.RelationshipsDeleted));
            Assert.Equal(1, deleteResult.Statistics.NodesDeleted);
            Assert.NotNull(deleteResult.Statistics.GetStringValue(Label.QueryInternalExecutionTime));

            Assert.NotNull(_api.Query("social", "CREATE (:person{name:'roi',age:32})"));
            Assert.NotNull(_api.Query("social", "MATCH (a:person), (b:person) WHERE (a.name = 'roi' AND b.name='amit')  CREATE (a)-[:knows]->(a)"));
            deleteResult = _api.Query("social", "MATCH (a:person) WHERE (a.name = 'roi') DELETE a");

            Assert.Empty(deleteResult);
            Assert.Null(deleteResult.Statistics.GetStringValue(Label.NodesCreated));
            Assert.Null(deleteResult.Statistics.GetStringValue(Label.PropertiesSet));
            Assert.Null(deleteResult.Statistics.GetStringValue(Label.NodesCreated));
            Assert.Null(deleteResult.Statistics.GetStringValue(Label.RelationshipsCreated));
            Assert.Equal(1, deleteResult.Statistics.RelationshipsDeleted);
            Assert.Equal(1, deleteResult.Statistics.NodesDeleted);

            Assert.NotNull(deleteResult.Statistics.GetStringValue(Label.QueryInternalExecutionTime));
        }

        [Fact]
        public void TestDeleteRelationship()
        {
            var graphName = $"social_{MethodBase.GetCurrentMethod()}";

            Assert.NotNull(_api.Query(graphName, "CREATE (:person{name:'roi',age:32})"));
            Assert.NotNull(_api.Query(graphName, "CREATE (:person{name:'amit',age:30})"));
            Assert.NotNull(_api.Query(graphName, "MATCH (a:person), (b:person) WHERE (a.name = 'roi' AND b.name='amit')  CREATE (a)-[:knows]->(a)"));
            ResultSet deleteResult = _api.Query(graphName, "MATCH (a:person)-[e]->() WHERE (a.name = 'roi') DELETE e");

            Assert.Empty(deleteResult);
            Assert.Null(deleteResult.Statistics.GetStringValue(Label.NodesCreated));
            Assert.Null(deleteResult.Statistics.GetStringValue(Label.PropertiesSet));
            Assert.Null(deleteResult.Statistics.GetStringValue(Label.NodesCreated));
            Assert.Null(deleteResult.Statistics.GetStringValue(Label.RelationshipsCreated));
            Assert.Null(deleteResult.Statistics.GetStringValue(Label.NodesDeleted));
            Assert.Equal(1, deleteResult.Statistics.RelationshipsDeleted);

            Assert.NotNull(deleteResult.Statistics.GetStringValue(Label.QueryInternalExecutionTime));
        }

        [Fact]
        public void TestIndex()
        {
            // Create both source and destination nodes
            Assert.NotNull(_api.Query("social", "CREATE (:person{name:'roi',age:32})"));

            ResultSet createIndexResult = _api.Query("social", "CREATE INDEX ON :person(age)");
            Assert.Empty(createIndexResult);
            Assert.Equal(1, createIndexResult.Statistics.IndicesCreated);

            // since RediSearch as index, those action are allowed
            ResultSet createNonExistingIndexResult = _api.Query("social", "CREATE INDEX ON :person(age1)");
            Assert.Empty(createNonExistingIndexResult);
            Assert.NotNull(createNonExistingIndexResult.Statistics.GetStringValue(Label.IndicesCreated));
            Assert.Equal(1, createNonExistingIndexResult.Statistics.IndicesCreated);
        }

        [Fact]
        public void TestHeader()
        {
            Assert.NotNull(_api.Query("social", "CREATE (:person{name:'roi',age:32})"));
            Assert.NotNull(_api.Query("social", "CREATE (:person{name:'amit',age:30})"));
            Assert.NotNull(_api.Query("social", "MATCH (a:person), (b:person) WHERE (a.name = 'roi' AND b.name='amit')  CREATE (a)-[:knows]->(a)"));

            ResultSet queryResult = _api.Query("social", "MATCH (a:person)-[r:knows]->(b:person) RETURN a,r, a.age");

            Assert.NotNull(queryResult.Header);
            Header header = queryResult.Header;

            List<string> schemaNames = header.SchemaNames;
            List<Header.ResultSetColumnTypes> schemaTypes = header.SchemaTypes;

            Assert.NotNull(schemaNames);
            Assert.NotNull(schemaTypes);

            Assert.Equal(3, schemaNames.Count);
            Assert.Equal(3, schemaTypes.Count);

            Assert.Equal("a", schemaNames[0]);
            Assert.Equal("r", schemaNames[1]);
            Assert.Equal("a.age", schemaNames[2]);

            Assert.Equal(ResultSetColumnTypes.COLUMN_NODE, schemaTypes[0]);
            Assert.Equal(ResultSetColumnTypes.COLUMN_RELATION, schemaTypes[1]);
            Assert.Equal(ResultSetColumnTypes.COLUMN_SCALAR, schemaTypes[2]);
        }

        [Fact]
        public void TestRecord()
        {
            string name = "roi";
            int age = 32;
            double doubleValue = 3.14;
            bool boolValue = true;

            string place = "TLV";
            int since = 2000;

            Property nameProperty = new Property("name", name);
            Property ageProperty = new Property("age", age);
            Property doubleProperty = new Property("doubleValue", doubleValue);
            Property trueBooleanProperty = new Property("boolValue", true);
            Property falseBooleanProperty = new Property("boolValue", false);
            Property nullProperty = new Property("nullValue", null);

            Property placeProperty = new Property("place", place);
            Property sinceProperty = new Property("since", since);

            Node expectedNode = new Node();
            expectedNode.Id = 0;
            expectedNode.AddLabel("person");
            expectedNode.AddProperty(nameProperty);
            expectedNode.AddProperty(ageProperty);
            expectedNode.AddProperty(doubleProperty);
            expectedNode.AddProperty(trueBooleanProperty);
            expectedNode.AddProperty(nullProperty);

            Assert.Equal("Node{labels=[person], id=0, propertyMap={name=Property{name='name', value=roi}, age=Property{name='age', value=32}, doubleValue=Property{name='doubleValue', value=3.14}, boolValue=Property{name='boolValue', value=True}, nullValue=Property{name='nullValue', value=null}}}", expectedNode.ToString());

            Edge expectedEdge = new Edge();
            expectedEdge.Id = 0;
            expectedEdge.Source = 0;
            expectedEdge.Destination = 1;
            expectedEdge.RelationshipType = "knows";
            expectedEdge.AddProperty(placeProperty);
            expectedEdge.AddProperty(sinceProperty);
            expectedEdge.AddProperty(doubleProperty);
            expectedEdge.AddProperty(falseBooleanProperty);
            expectedEdge.AddProperty(nullProperty);

            Assert.Equal("Edge{relationshipType='knows', source=0, destination=1, id=0, propertyMap={place=Property{name='place', value=TLV}, since=Property{name='since', value=2000}, doubleValue=Property{name='doubleValue', value=3.14}, boolValue=Property{name='boolValue', value=False}, nullValue=Property{name='nullValue', value=null}}}", expectedEdge.ToString());

            var parms = new Dictionary<string, object>
            {
                { "name", name },
                { "age", age },
                { "boolValue", boolValue },
                { "doubleValue", doubleValue }
            };

            Assert.NotNull(_api.Query("social", "CREATE (:person{name:$name,age:$age, doubleValue:$doubleValue, boolValue:$boolValue, nullValue:null})", parms));
            Assert.NotNull(_api.Query("social", "CREATE (:person{name:'amit',age:30})"));
            Assert.NotNull(_api.Query("social", "MATCH (a:person), (b:person) WHERE (a.name = 'roi' AND b.name='amit') CREATE (a)-[:knows{place:'TLV', since:2000,doubleValue:3.14, boolValue:false, nullValue:null}]->(b)"));

            ResultSet resultSet = _api.Query("social", "MATCH (a:person)-[r:knows]->(b:person) RETURN a,r, a.name, a.age, a.doubleValue, a.boolValue, a.nullValue, r.place, r.since, r.doubleValue, r.boolValue, r.nullValue");

            Assert.NotNull(resultSet);

            Assert.Equal(0, resultSet.Statistics.NodesCreated);
            Assert.Equal(0, resultSet.Statistics.NodesDeleted);
            Assert.Equal(0, resultSet.Statistics.LabelsAdded);
            Assert.Equal(0, resultSet.Statistics.PropertiesSet);
            Assert.Equal(0, resultSet.Statistics.RelationshipsCreated);
            Assert.Equal(0, resultSet.Statistics.RelationshipsDeleted);
            Assert.NotNull(resultSet.Statistics.GetStringValue(Label.QueryInternalExecutionTime));
            Assert.Single(resultSet);

            Record record = resultSet.First();

            Node node = record.GetValue<Node>(0);
            Assert.NotNull(node);

            Assert.Equal(expectedNode, node);

            node = record.GetValue<Node>("a");

            Assert.Equal(expectedNode, node);

            Edge edge = record.GetValue<Edge>(1);
            Assert.NotNull(edge);
            Assert.Equal(expectedEdge, edge);

            edge = record.GetValue<Edge>("r");
            Assert.Equal(expectedEdge, edge);

            Assert.Equal(new[] { "a", "r", "a.name", "a.age", "a.doubleValue", "a.boolValue", "a.nullValue", "r.place", "r.since", "r.doubleValue", "r.boolValue", "r.nullValue" }, record.Keys);

            Assert.Equal(new object[] { expectedNode, expectedEdge, name, age, doubleValue, true, null, place, since, doubleValue, false, null }, record.Values);

            Assert.Equal("roi", record.GetString(2));
            Assert.Equal("32", record.GetString(3));
            Assert.Equal(32, record.GetValue<int>(3));
            Assert.Equal(32, record.GetValue<int>("a.age"));
            Assert.Equal("roi", record.GetString("a.name"));
            Assert.Equal("32", record.GetString("a.age"));
        }

        [Fact]
        public void TinyTestMultiThread()
        {
            ResultSet resultSet = _api.Query("social", "CREATE ({name:'roi',age:32})");

            _api.Query("social", "MATCH (a:person) RETURN a");

            for (int i = 0; i < 10000; i++)
            {
                var resultSets = Enumerable.Range(0, 16).AsParallel().Select(x => _api.Query("social", "MATCH (a:person) RETURN a"));
            }
        }

        [Fact]
        public void TestMultiThread()
        {

            Assert.NotNull(_api.Query("social", "CREATE (:person {name:'roi', age:32})-[:knows]->(:person {name:'amit',age:30}) "));

            List<ResultSet> resultSets = Enumerable.Range(0, 16).AsParallel().Select(x => _api.Query("social", "MATCH (a:person)-[r:knows]->(b:person) RETURN a,r, a.age")).ToList();

            Property nameProperty = new Property("name", "roi");
            Property ageProperty = new Property("age", 32);
            Property lastNameProperty = new Property("lastName", "a");

            Node expectedNode = new Node();
            expectedNode.Id = 0;
            expectedNode.AddLabel("person");
            expectedNode.AddProperty(nameProperty);
            expectedNode.AddProperty(ageProperty);

            Edge expectedEdge = new Edge();
            expectedEdge.Id = 0;
            expectedEdge.Source = 0;
            expectedEdge.Destination = 1;
            expectedEdge.RelationshipType = "knows";

            foreach (ResultSet resultSet in resultSets)
            {
                Assert.NotNull(resultSet.Header);
                Header header = resultSet.Header;
                List<String> schemaNames = header.SchemaNames;
                List<Header.ResultSetColumnTypes> schemaTypes = header.SchemaTypes;
                Assert.NotNull(schemaNames);
                Assert.NotNull(schemaTypes);
                Assert.Equal(3, schemaNames.Count);
                Assert.Equal(3, schemaTypes.Count);
                Assert.Equal("a", schemaNames[0]);
                Assert.Equal("r", schemaNames[1]);
                Assert.Equal("a.age", schemaNames[2]);
                Assert.Equal(ResultSetColumnTypes.COLUMN_NODE, schemaTypes[0]);
                Assert.Equal(ResultSetColumnTypes.COLUMN_RELATION, schemaTypes[1]);
                Assert.Equal(ResultSetColumnTypes.COLUMN_SCALAR, schemaTypes[2]);
                Assert.Single(resultSet);
                Record record = resultSet.First();
                Assert.Equal(new[] { "a", "r", "a.age" }, record.Keys);
                Assert.Equal(new object[] { expectedNode, expectedEdge, 32 }, record.Values);
            }

            //test for update in local cache
            expectedNode.RemoveProperty("name");
            expectedNode.RemoveProperty("age");
            expectedNode.AddProperty(lastNameProperty);
            expectedNode.RemoveLabel("person");
            expectedNode.AddLabel("worker");
            expectedNode.Id = 2;

            expectedEdge.RelationshipType = "worksWith";
            expectedEdge.Source = 2;
            expectedEdge.Destination = 3;
            expectedEdge.Id = 1;

            Assert.NotNull(_api.Query("social", "CREATE (:worker{lastName:'a'})"));
            Assert.NotNull(_api.Query("social", "CREATE (:worker{lastName:'b'})"));
            Assert.NotNull(_api.Query("social", "MATCH (a:worker), (b:worker) WHERE (a.lastName = 'a' AND b.lastName='b')  CREATE (a)-[:worksWith]->(b)"));

            resultSets = Enumerable.Range(0, 16).AsParallel().Select(x => _api.Query("social", "MATCH (a:worker)-[r:worksWith]->(b:worker) RETURN a,r")).ToList();

            foreach (ResultSet resultSet in resultSets)
            {
                Assert.NotNull(resultSet.Header);
                Header header = resultSet.Header;
                List<String> schemaNames = header.SchemaNames;
                List<Header.ResultSetColumnTypes> schemaTypes = header.SchemaTypes;
                Assert.NotNull(schemaNames);
                Assert.NotNull(schemaTypes);
                Assert.Equal(2, schemaNames.Count);
                Assert.Equal(2, schemaTypes.Count);
                Assert.Equal("a", schemaNames[0]);
                Assert.Equal("r", schemaNames[1]);
                Assert.Equal(ResultSetColumnTypes.COLUMN_NODE, schemaTypes[0]);
                Assert.Equal(ResultSetColumnTypes.COLUMN_RELATION, schemaTypes[1]);
                Assert.Single(resultSet);
                Record record = resultSet.First();
                Assert.Equal(new[] { "a", "r" }, record.Keys);
                Assert.Equal(new object[] { expectedNode, expectedEdge }, record.Values);
            }
        }

        [Fact]
        public void TestAdditionToProcedures()
        {
            Assert.NotNull(_api.Query("social", "CREATE (:person{name:'roi',age:32})"));
            Assert.NotNull(_api.Query("social", "CREATE (:person{name:'amit',age:30})"));
            Assert.NotNull(_api.Query("social", "MATCH (a:person), (b:person) WHERE (a.name = 'roi' AND b.name='amit')  CREATE (a)-[:knows]->(b)"));

            List<ResultSet> resultSets = Enumerable.Range(0, 16).AsParallel().Select(x => _api.Query("social", "MATCH (a:person)-[r:knows]->(b:person) RETURN a,r")).ToList();

            //expected objects init
            Property nameProperty = new Property("name", "roi");
            Property ageProperty = new Property("age", 32);
            Property lastNameProperty = new Property("lastName", "a");

            Node expectedNode = new Node();
            expectedNode.Id = 0;
            expectedNode.AddLabel("person");
            expectedNode.AddProperty(nameProperty);
            expectedNode.AddProperty(ageProperty);

            Edge expectedEdge = new Edge();
            expectedEdge.Id = 0;
            expectedEdge.Source = 0;
            expectedEdge.Destination = 1;
            expectedEdge.RelationshipType = "knows";

            ResultSet resultSet = _api.Query("social", "MATCH (a:person)-[r:knows]->(b:person) RETURN a,r");
            Assert.NotNull(resultSet.Header);
            Header header = resultSet.Header;
            List<String> schemaNames = header.SchemaNames;
            List<Header.ResultSetColumnTypes> schemaTypes = header.SchemaTypes;
            Assert.NotNull(schemaNames);
            Assert.NotNull(schemaTypes);
            Assert.Equal(2, schemaNames.Count);
            Assert.Equal(2, schemaTypes.Count);
            Assert.Equal("a", schemaNames[0]);
            Assert.Equal("r", schemaNames[1]);
            Assert.Equal(ResultSetColumnTypes.COLUMN_NODE, schemaTypes[0]);
            Assert.Equal(ResultSetColumnTypes.COLUMN_RELATION, schemaTypes[1]);
            Assert.Single(resultSet);
            Record record = resultSet.First();
            Assert.Equal(new[] { "a", "r" }, record.Keys);
            Assert.Equal(new object[] { expectedNode, expectedEdge }, record.Values);

            //test for local cache updates

            expectedNode.RemoveProperty("name");
            expectedNode.RemoveProperty("age");
            expectedNode.AddProperty(lastNameProperty);
            expectedNode.RemoveLabel("person");
            expectedNode.AddLabel("worker");
            expectedNode.Id = 2;
            expectedEdge.RelationshipType = "worksWith";
            expectedEdge.Source = 2;
            expectedEdge.Destination = 3;
            expectedEdge.Id = 1;
            Assert.NotNull(_api.Query("social", "CREATE (:worker{lastName:'a'})"));
            Assert.NotNull(_api.Query("social", "CREATE (:worker{lastName:'b'})"));
            Assert.NotNull(_api.Query("social", "MATCH (a:worker), (b:worker) WHERE (a.lastName = 'a' AND b.lastName='b')  CREATE (a)-[:worksWith]->(b)"));
            resultSet = _api.Query("social", "MATCH (a:worker)-[r:worksWith]->(b:worker) RETURN a,r");
            Assert.NotNull(resultSet.Header);
            header = resultSet.Header;
            schemaNames = header.SchemaNames;
            schemaTypes = header.SchemaTypes;
            Assert.NotNull(schemaNames);
            Assert.NotNull(schemaTypes);
            Assert.Equal(2, schemaNames.Count);
            Assert.Equal(2, schemaTypes.Count);
            Assert.Equal("a", schemaNames[0]);
            Assert.Equal("r", schemaNames[1]);
            Assert.Equal(ResultSetColumnTypes.COLUMN_NODE, schemaTypes[0]);
            Assert.Equal(ResultSetColumnTypes.COLUMN_RELATION, schemaTypes[1]);
            Assert.Single(resultSet);
            record = resultSet.First();
            Assert.Equal(new[] { "a", "r" }, record.Keys);
            Assert.Equal(new object[] { expectedNode, expectedEdge }, record.Values);
        }

        [Fact]
        public void TestEscapedQuery()
        {
            Assert.NotNull(_api.Query("social", "MATCH (n) where n.s1='S\"\\'' RETURN n"));
        }

        [Fact]
        public void TestMultiExec()
        {
            var transaction = _api.Multi();

            // transaction.SetAsync("x", "1");
            transaction.QueryAsync("social", "CREATE (:Person {name:'a'})");
            transaction.QueryAsync("g", "CREATE (:Person {name:'a'})");
            // transaction.IncrAsync("x");
            // transaction.GetAsync("x");
            transaction.QueryAsync("social", "MATCH (n:Person) RETURN n");
            transaction.DeleteGraphAsync("g");
            transaction.CallProcedureAsync("social", "db.labels");

            var results = transaction.Exec();

            // Skipping Redis SET command assetions...

            // Redis Graph command
            var resultSet = results[0];
            Assert.Equal(1, resultSet.Statistics.NodesCreated);
            Assert.Equal(1, resultSet.Statistics.PropertiesSet);

            resultSet = results[1];
            Assert.Equal(1, resultSet.Statistics.NodesCreated);
            Assert.Equal(1, resultSet.Statistics.PropertiesSet);

            // Skipping Redis INCR command assertions...

            // Skipping Redis GET command assertions...

            // Graph Query Result
            resultSet = results[2];
            Assert.NotNull(resultSet.Header);

            var header = resultSet.Header;

            var schemaNames = header.SchemaNames;
            var schemaTypes = header.SchemaTypes;

            Assert.NotNull(schemaNames);
            Assert.NotNull(schemaTypes);

            Assert.Single(schemaNames);
            Assert.Single(schemaTypes);

            Assert.Equal("n", schemaNames[0]);
            Assert.Equal(ResultSetColumnTypes.COLUMN_NODE, schemaTypes[0]);

            var nameProperty = new Property("name", "a");

            var expectedNode = new Node();
            expectedNode.Id = 0;
            expectedNode.AddLabel("Person");
            expectedNode.AddProperty(nameProperty);

            // See that the result were pulled from the right graph.

            Assert.Single(resultSet);

            var record = resultSet.First();
            Assert.Equal(new List<string> { "n" }, record.Keys);
            Assert.Equal(expectedNode, record.GetValue<Node>("n"));

            resultSet = results[4];

            Assert.NotNull(resultSet.Header);

            schemaNames = header.SchemaNames;
            schemaTypes = header.SchemaTypes;

            Assert.NotNull(schemaNames);
            Assert.NotNull(schemaTypes);

            Assert.Single(schemaNames);
            Assert.Single(schemaTypes);

            Assert.Equal("n", schemaNames[0]);

            Assert.Equal(ResultSetColumnTypes.COLUMN_NODE, schemaTypes[0]);

            Assert.Single(resultSet);

            record = resultSet.First();

            Assert.Equal(new List<string> { "label" }, record.Keys);
            Assert.Equal("Person", record.GetValue<string>("label"));
        }

        /*

        Since by default all commands executed by StackExchange.Redis travel through the same connection
        we're going to skip the following "contexted" tests:

        - testContextedAPI
        - testWriteTransactionWatch
        - testReadTransactionWatch

        */

        [Fact]
        public void TestArraySupport()
        {
            var expectedANode = new Node();
            expectedANode.Id = 0;
            expectedANode.AddLabel("person");
            var aNameProperty = new Property("name", "a");
            var aAgeProperty = new Property("age", 32);
            var aListProperty = new Property("array", new object[] { 0, 1, 2 });
            expectedANode.AddProperty(aNameProperty);
            expectedANode.AddProperty(aAgeProperty);
            expectedANode.AddProperty(aListProperty);

            var expectedBNode = new Node();
            expectedBNode.Id = 1;
            expectedBNode.AddLabel("person");
            var bNameProperty = new Property("name", "b");
            var bAgeProperty = new Property("age", 30);
            var bListProperty = new Property("array", new object[] { 3, 4, 5 });
            expectedBNode.AddProperty(bNameProperty);
            expectedBNode.AddProperty(bAgeProperty);
            expectedBNode.AddProperty(bListProperty);

            Assert.NotNull(_api.Query("social", "CREATE (:person{name:'a',age:32,array:[0,1,2]})"));
            Assert.NotNull(_api.Query("social", "CREATE (:person{name:'b',age:30,array:[3,4,5]})"));

            // test array

            var resultSet = _api.Query("social", "WITH [0,1,2] as x return x");

            // check header
            Assert.NotNull(resultSet.Header);
            var header = resultSet.Header;

            var schemaNames = header.SchemaNames;
            var schemaTypes = header.SchemaTypes;

            Assert.NotNull(schemaNames);
            Assert.NotNull(schemaTypes);

            Assert.Single(schemaNames);
            Assert.Single(schemaTypes);

            Assert.Equal("x", schemaNames[0]);
            Assert.Equal(ResultSetColumnTypes.COLUMN_SCALAR, schemaTypes[0]);

            // check record
            Assert.Single(resultSet);
            var record = resultSet.First();
            Assert.Equal(new[] { "x" }, record.Keys);

            var x = record.GetValue<object[]>("x");
            Assert.Equal(new object[] { 0, 1, 2 }, x);

            // test collect
            resultSet = _api.Query("social", "MATCH(n) return collect(n) as x");

            Assert.NotNull(resultSet.Header);
            header = resultSet.Header;

            schemaNames = header.SchemaNames;
            schemaTypes = header.SchemaTypes;

            Assert.NotNull(schemaNames);
            Assert.NotNull(schemaTypes);

            Assert.Single(schemaNames);
            Assert.Single(schemaTypes);

            Assert.Equal("x", schemaNames[0]);
            Assert.Equal(ResultSetColumnTypes.COLUMN_SCALAR, schemaTypes[0]);

            // check record
            Assert.Single(resultSet);
            record = resultSet.First();
            Assert.Equal(new[] { "x" }, record.Keys);
            x = record.GetValue<object[]>("x");

            Assert.Contains(expectedANode, x);
            Assert.Contains(expectedBNode, x);

            // test unwind
            resultSet = _api.Query("social", "unwind([0,1,2]) as x return x");

            Assert.NotNull(resultSet.Header);
            header = resultSet.Header;

            schemaNames = header.SchemaNames;
            schemaTypes = header.SchemaTypes;

            Assert.NotNull(schemaNames);
            Assert.NotNull(schemaTypes);

            Assert.Single(schemaNames);
            Assert.Single(schemaTypes);

            Assert.Equal("x", schemaNames[0]);
            Assert.Equal(ResultSetColumnTypes.COLUMN_SCALAR, schemaTypes[0]);

            // check record
            Assert.Equal(3, resultSet.Count);

            for (var i = 0; i < 3; i++)
            {
                record = resultSet.ElementAt(i);

                Assert.Equal(new[] { "x" }, record.Keys);
                Assert.Equal(i, record.GetValue<int>("x"));
            }
        }

        [Fact]
        public void TestPath()
        {
            List<Node> nodes = new List<Node>(3);

            for (int i = 0; i < 3; i++)
            {
                var node = new Node();
                node.Id = i;
                node.AddLabel("L1");
                nodes.Add(node);
            }

            List<Edge> edges = new List<Edge>(2);

            for (int i = 0; i < 2; i++)
            {
                var edge = new Edge();
                edge.Id = i;
                edge.RelationshipType = "R1";
                edge.Source = i;
                edge.Destination = i + 1;

                edges.Add(edge);
            }

            var expectedPaths = new HashSet<Path>();

            var path01 = new PathBuilder(2).Append(nodes[0]).Append(edges[0]).Append(nodes[1]).Build();
            var path12 = new PathBuilder(2).Append(nodes[1]).Append(edges[1]).Append(nodes[2]).Build();
            var path02 = new PathBuilder(3).Append(nodes[0]).Append(edges[0]).Append(nodes[1]).Append(edges[1]).Append(nodes[2]).Build();

            expectedPaths.Add(path01);
            expectedPaths.Add(path12);
            expectedPaths.Add(path02);

            _api.Query("social", "CREATE (:L1)-[:R1]->(:L1)-[:R1]->(:L1)");

            var resultSet = _api.Query("social", "MATCH p = (:L1)-[:R1*]->(:L1) RETURN p");

            Assert.Equal(expectedPaths.Count, resultSet.Count);

            for (int i = 0; i < resultSet.Count; i++)
            {
                Path p = resultSet.ElementAt(i).GetValue<Path>("p");
                Assert.Contains(p, expectedPaths);
                expectedPaths.Remove(p);
            }
        }

        [Fact]
        public void TestParameters()
        {
            Object[] parameters = { 1, 2.3, true, false, null, "str", new List<int> { 1, 2, 3 }, new[] { 1, 2, 3 } };

            var param = new Dictionary<string, object>();

            for (int i = 0; i < parameters.Length; i++)
            {
                Object expected = parameters[i];
                param.Put("param", expected);
                ResultSet resultSet = _api.Query("social", "RETURN $param", param);
                Assert.Single(resultSet);
                Record r = resultSet.First();
                Object o = r.GetValue<object>(0);
                if (i == parameters.Length - 1)
                {
                    expected = Array.ConvertAll((int[])expected, x => (object)x);
                }
                Assert.Equal(expected, o);
            }
        }

    }
}