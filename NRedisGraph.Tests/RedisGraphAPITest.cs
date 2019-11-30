// .NET port of https://github.com/RedisGraph/JRedisGraph
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
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

            Assert.Equal(0, resultSet.Count());
        }

        [Fact]
        public void TestCreateLabeledNode()
        {
            // Create a node with a label
            ResultSet resultSet = _api.Query("social", "CREATE (:human{name:'danny',age:12})");
            Assert.Equal(0, resultSet.Count());
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

            Assert.Equal(0, resultSet.Count());
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

            Assert.Equal(0, deleteResult.Count());
            Assert.Null(deleteResult.Statistics.GetStringValue(Label.NodesCreated));
            Assert.Null(deleteResult.Statistics.GetStringValue(Label.PropertiesSet));
            Assert.Null(deleteResult.Statistics.GetStringValue(Label.RelationshipsCreated));
            Assert.Null(deleteResult.Statistics.GetStringValue(Label.RelationshipsDeleted));
            Assert.Equal(1, deleteResult.Statistics.NodesDeleted); 
            Assert.NotNull(deleteResult.Statistics.GetStringValue(Label.QueryInternalExecutionTime));

            Assert.NotNull(_api.Query("social", "CREATE (:person{name:'roi',age:32})")); Assert.NotNull(_api.Query("social", "MATCH (a:person), (b:person) WHERE (a.name = 'roi' AND b.name='amit')  CREATE (a)-[:knows]->(a)")); deleteResult = _api.Query("social", "MATCH (a:person) WHERE (a.name = 'roi') DELETE a");

            Assert.Equal(0, deleteResult.Count()); Assert.Null(deleteResult.Statistics.GetStringValue(Label.NodesCreated)); Assert.Null(deleteResult.Statistics.GetStringValue(Label.PropertiesSet)); Assert.Null(deleteResult.Statistics.GetStringValue(Label.NodesCreated)); Assert.Null(deleteResult.Statistics.GetStringValue(Label.RelationshipsCreated)); Assert.Equal(1, deleteResult.Statistics.RelationshipsDeleted); Assert.Equal(1, deleteResult.Statistics.NodesDeleted);

            Assert.NotNull(deleteResult.Statistics.GetStringValue(Label.QueryInternalExecutionTime));
        }

        [Fact]
        public void TestDeleteRelationship()
        {
            Assert.NotNull(_api.Query("social", "CREATE (:person{name:'roi',age:32})"));
            Assert.NotNull(_api.Query("social", "CREATE (:person{name:'amit',age:30})"));
            Assert.NotNull(_api.Query("social", "MATCH (a:person), (b:person) WHERE (a.name = 'roi' AND b.name='amit')  CREATE (a)-[:knows]->(a)"));
            ResultSet deleteResult = _api.Query("social", "MATCH (a:person)-[e]->() WHERE (a.name = 'roi') DELETE e");

            Assert.Equal(0, deleteResult.Count());
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
            Assert.Equal(0, createIndexResult.Count());
            Assert.Equal(1, createIndexResult.Statistics.IndicesAdded);

            ResultSet failCreateIndexResult = _api.Query("social", "CREATE INDEX ON :person(age1)");
            Assert.Equal(0, failCreateIndexResult.Count());
            Assert.Null(failCreateIndexResult.Statistics.GetStringValue(Label.IndicesAdded));
            Assert.Equal(0, failCreateIndexResult.Statistics.IndicesAdded);
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

            Property nameProperty = new Property("name", ResultSet.ResultSetScalarType.PROPERTY_STRING, name);
            Property ageProperty = new Property("age", ResultSet.ResultSetScalarType.PROPERTY_INTEGER, age);
            Property doubleProperty = new Property("doubleValue", ResultSet.ResultSetScalarType.PROPERTY_DOUBLE, doubleValue);
            Property trueBooleanProperty = new Property("boolValue", ResultSet.ResultSetScalarType.PROPERTY_BOOLEAN, true);
            Property falseBooleanProperty = new Property("boolValue", ResultSet.ResultSetScalarType.PROPERTY_BOOLEAN, false);
            Property nullProperty = new Property("nullValue", ResultSet.ResultSetScalarType.PROPERTY_NULL, null);

            Property placeProperty = new Property("place", ResultSet.ResultSetScalarType.PROPERTY_STRING, place);
            Property sinceProperty = new Property("since", ResultSet.ResultSetScalarType.PROPERTY_INTEGER, since);

            Node expectedNode = new Node();
            expectedNode.Id = 0;
            expectedNode.AddLabel("person");
            expectedNode.AddProperty(nameProperty);
            expectedNode.AddProperty(ageProperty);
            expectedNode.AddProperty(doubleProperty);
            expectedNode.AddProperty(trueBooleanProperty);
            expectedNode.AddProperty(nullProperty);

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

            Assert.NotNull(_api.Query("social", "CREATE (:person{name:%s',age:%d, doubleValue:%f, boolValue:%b, nullValue:null})", name, age, doubleValue, boolValue));
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

            Assert.Equal(1, resultSet.Count());

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

            Assert.Equal(new [] { "a", "r", "a.name", "a.age", "a.doubleValue", "a.boolValue", "a.nullValue", "r.place", "r.since", "r.doubleValue", "r.boolValue", "r.nullValue" }, record.Keys);

            Assert.Equal(new object[] { expectedNode, expectedEdge, name, age, doubleValue, true, null, place, since, doubleValue, false, null }, record.Values);

            Assert.Equal("roi", record.GetString(2));
            Assert.Equal("32", record.GetString(3));
            Assert.Equal(32L, record.GetValue<long>(3));
            Assert.Equal(32L, record.GetValue<long>("a.age"));
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

            Property nameProperty = new Property("name", ResultSet.ResultSetScalarType.PROPERTY_STRING, "roi");
            Property ageProperty = new Property("age", ResultSet.ResultSetScalarType.PROPERTY_INTEGER, 32);
            Property lastNameProperty = new Property("lastName", ResultSet.ResultSetScalarType.PROPERTY_STRING, "a");

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
                Assert.Equal(1, resultSet.Count);
                Record record = resultSet.First();
                Assert.Equal(new [] { "a", "r", "a.age" }, record.Keys);
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
                Assert.Equal(1, resultSet.Count);
                Record record = resultSet.First();
                Assert.Equal(new [] { "a", "r" }, record.Keys);
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
            Property nameProperty = new Property("name", ResultSet.ResultSetScalarType.PROPERTY_STRING, "roi");
            Property ageProperty = new Property("age", ResultSet.ResultSetScalarType.PROPERTY_INTEGER, 32);
            Property lastNameProperty = new Property("lastName", ResultSet.ResultSetScalarType.PROPERTY_STRING, "a");

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
            Assert.Equal(1, resultSet.Count);
            Record record = resultSet.First();
            Assert.Equal(new [] { "a", "r" }, record.Keys);
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
            Assert.Equal(1, resultSet.Count);
            record = resultSet.First();
            Assert.Equal(new [] { "a", "r" }, record.Keys);
            Assert.Equal(new object[] { expectedNode, expectedEdge }, record.Values);
        }

        [Fact]
        public void TestEscapedQuery()
        {
            Assert.NotNull(_api.Query("social", "CREATE (:escaped{s1:%s,s2:%s})", "S\"\'", "S\\'\\\""));
            Assert.NotNull(_api.Query("social", "MATCH (n) where n.s1=%s and n.s2=%s RETURN n", "S\"\'", "S\\'\\\""));
            Assert.NotNull(_api.Query("social", "MATCH (n) where n.s1='S\"\\'' RETURN n"));
        }
    }
}