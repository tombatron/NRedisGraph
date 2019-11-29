// .NET port of https://github.com/RedisGraph/JRedisGraph
using System;
using System.Linq;
using StackExchange.Redis;
using Xunit;
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
            Assert.Equal(1, deleteResult.Statistics.NodesDeleted;
            Assert.NotNull(deleteResult.Statistics.GetStringValue(Label.QueryInternalExecutionTime));

            Assert.NotNull(_api.Query("social", "CREATE (:person{name:'roi',age:32})"));
            Assert.NotNull(_api.Query("social", "MATCH (a:person), (b:person) WHERE (a.name = 'roi' AND b.name='amit')  CREATE (a)-[:knows]->(a)"));
            deleteResult = _api.Query("social", "MATCH (a:person) WHERE (a.name = 'roi') DELETE a");

            Assert.Equal(0, deleteResult.Count());
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
            Assert.assertNotNull(api.query("social", "CREATE (:person{name:'roi',age:32})"));
            Assert.assertNotNull(api.query("social", "CREATE (:person{name:'amit',age:30})"));
            Assert.assertNotNull(api.query("social", "MATCH (a:person), (b:person) WHERE (a.name = 'roi' AND b.name='amit')  CREATE (a)-[:knows]->(a)"));

            ResultSet queryResult = api.query("social", "MATCH (a:person)-[r:knows]->(b:person) RETURN a,r, a.age");

            Assert.assertNotNull(queryResult.getHeader());
            Header header = queryResult.getHeader();

            List<String> schemaNames = header.getSchemaNames();
            List<Header.ResultSetColumnTypes> schemaTypes = header.getSchemaTypes();

            Assert.assertNotNull(schemaNames);
            Assert.assertNotNull(schemaTypes);

            Assert.assertEquals(3, schemaNames.size());
            Assert.assertEquals(3, schemaTypes.size());

            Assert.assertEquals("a", schemaNames.get(0));
            Assert.assertEquals("r", schemaNames.get(1));
            Assert.assertEquals("a.age", schemaNames.get(2));

            Assert.assertEquals(COLUMN_NODE, schemaTypes.get(0));
            Assert.assertEquals(COLUMN_RELATION, schemaTypes.get(1));
            Assert.assertEquals(COLUMN_SCALAR, schemaTypes.get(2));
        }

        [Fact]
        public void TestRecord()
        {
            String name = "roi";
            int age = 32;
            double doubleValue = 3.14;
            bool boolValue  = true;

            String place = "TLV";
            int since = 2000;



            Property nameProperty = new Property("name", ResultSet.ResultSetScalarTypes.PROPERTY_STRING, name);
            Property ageProperty = new Property("age", ResultSet.ResultSetScalarTypes.PROPERTY_INTEGER, age);
            Property doubleProperty = new Property("doubleValue", ResultSet.ResultSetScalarTypes.PROPERTY_DOUBLE, doubleValue);
            Property trueBooleanProperty = new Property("boolValue", ResultSet.ResultSetScalarTypes.PROPERTY_BOOLEAN, true);
            Property falseBooleanProperty = new Property("boolValue", ResultSet.ResultSetScalarTypes.PROPERTY_BOOLEAN, false);
            Property nullProperty = new Property("nullValue", ResultSet.ResultSetScalarTypes.PROPERTY_NULL, null);

            Property placeProperty = new Property("place", ResultSet.ResultSetScalarTypes.PROPERTY_STRING, place);
            Property sinceProperty = new Property("since", ResultSet.ResultSetScalarTypes.PROPERTY_INTEGER, since);

            Node expectedNode = new Node();
            expectedNode.setId(0);
            expectedNode.addLabel("person");
            expectedNode.addProperty(nameProperty);
            expectedNode.addProperty(ageProperty);
            expectedNode.addProperty(doubleProperty);
            expectedNode.addProperty(trueBooleanProperty);
            expectedNode.addProperty(nullProperty);

            Edge expectedEdge = new Edge();
            expectedEdge.setId(0);
            expectedEdge.setSource(0);
            expectedEdge.setDestination(1);
            expectedEdge.setRelationshipType("knows");
            expectedEdge.addProperty(placeProperty);
            expectedEdge.addProperty(sinceProperty);
            expectedEdge.addProperty(doubleProperty);
            expectedEdge.addProperty(falseBooleanProperty);
            expectedEdge.addProperty(nullProperty);



            Assert.assertNotNull(api.query("social", "CREATE (:person{name:%s',age:%d, doubleValue:%f, boolValue:%b, nullValue:null})", name, age, doubleValue, boolValue));
            Assert.assertNotNull(api.query("social", "CREATE (:person{name:'amit',age:30})"));
            Assert.assertNotNull(api.query("social", "MATCH (a:person), (b:person) WHERE (a.name = 'roi' AND b.name='amit')  " +
                    "CREATE (a)-[:knows{place:'TLV', since:2000,doubleValue:3.14, boolValue:false, nullValue:null}]->(b)"));

            ResultSet resultSet = api.query("social", "MATCH (a:person)-[r:knows]->(b:person) RETURN a,r, " +
                    "a.name, a.age, a.doubleValue, a.boolValue, a.nullValue, " +
                    "r.place, r.since, r.doubleValue, r.boolValue, r.nullValue");
            Assert.assertNotNull(resultSet);


            Assert.assertEquals(0, resultSet.Statistics.nodesCreated());
            Assert.assertEquals(0, resultSet.Statistics.nodesDeleted());
            Assert.assertEquals(0, resultSet.Statistics.labelsAdded());
            Assert.assertEquals(0, resultSet.Statistics.propertiesSet());
            Assert.assertEquals(0, resultSet.Statistics.relationshipsCreated());
            Assert.assertEquals(0, resultSet.Statistics.relationshipsDeleted());
            Assert.assertNotNull(resultSet.Statistics.getStringValue(Label.QUERY_INTERNAL_EXECUTION_TIME));


            Assert.assertEquals(1, resultSet.size());
            Assert.assertTrue(resultSet.hasNext());
            Record record = resultSet.next();
            Assert.assertFalse(resultSet.hasNext());

            Node node = record.getValue(0);
            Assert.assertNotNull(node);

            Assert.assertEquals(expectedNode, node);

            node = record.getValue("a");
            Assert.assertEquals(expectedNode, node);

            Edge edge = record.getValue(1);
            Assert.assertNotNull(edge);
            Assert.assertEquals(expectedEdge, edge);

            edge = record.getValue("r");
            Assert.assertEquals(expectedEdge, edge);

            Assert.assertEquals(Arrays.asList("a", "r", "a.name", "a.age", "a.doubleValue", "a.boolValue", "a.nullValue",
                    "r.place", "r.since", "r.doubleValue", "r.boolValue", "r.nullValue"), record.keys());

            Assert.assertEquals(Arrays.asList(expectedNode, expectedEdge,
                    name, age, doubleValue, true, null,
                    place, since, doubleValue, false, null),
                    record.values());


            Assert.assertEquals( "roi", record.getString(2));
            Assert.assertEquals( "32", record.getString(3));
            Assert.assertEquals( 32L, ((Integer)(record.getValue(3))).longValue());
            Assert.assertEquals( 32L, ((Integer)record.getValue("a.age")).longValue());
            Assert.assertEquals( "roi", record.getString("a.name"));
            Assert.assertEquals( "32", record.getString("a.age"));

        }


        [Fact]
        public void TinyTestMultiThread()
        {
            ResultSet resultSet = api.query("social", "CREATE ({name:'roi',age:32})");
            api.query("social", "MATCH (a:person) RETURN a");
            for (int i =0; i < 10000; i++){
                List<ResultSet> resultSets = IntStream.range(0,16).parallel().
                        mapToObj(
                                j-> api.query("social", "MATCH (a:person) RETURN a")).
                        collect(Collectors.toList());

            }
        }

        [Fact]
        public void TestMultiThread()
        {

            Assert.assertNotNull(api.query("social", "CREATE (:person {name:'roi', age:32})-[:knows]->(:person {name:'amit',age:30}) "));

            List<ResultSet> resultSets = IntStream.range(0,16).parallel().
                    mapToObj(i-> api.query("social", "MATCH (a:person)-[r:knows]->(b:person) RETURN a,r, a.age")).
                    collect(Collectors.toList());

            Property nameProperty = new Property("name", ResultSet.ResultSetScalarTypes.PROPERTY_STRING, "roi");
            Property ageProperty = new Property("age", ResultSet.ResultSetScalarTypes.PROPERTY_INTEGER, 32);
            Property lastNameProperty =new Property("lastName", ResultSet.ResultSetScalarTypes.PROPERTY_STRING, "a");

            Node expectedNode = new Node();
            expectedNode.setId(0);
            expectedNode.addLabel("person");
            expectedNode.addProperty(nameProperty);
            expectedNode.addProperty(ageProperty);


            Edge expectedEdge = new Edge();
            expectedEdge.setId(0);
            expectedEdge.setSource(0);
            expectedEdge.setDestination(1);
            expectedEdge.setRelationshipType("knows");


            for (ResultSet resultSet : resultSets){
                Assert.assertNotNull(resultSet.getHeader());
                Header header = resultSet.getHeader();
                List<String> schemaNames = header.getSchemaNames();
                List<Header.ResultSetColumnTypes> schemaTypes = header.getSchemaTypes();
                Assert.assertNotNull(schemaNames);
                Assert.assertNotNull(schemaTypes);
                Assert.assertEquals(3, schemaNames.size());
                Assert.assertEquals(3, schemaTypes.size());
                Assert.assertEquals("a", schemaNames.get(0));
                Assert.assertEquals("r", schemaNames.get(1));
                Assert.assertEquals("a.age", schemaNames.get(2));
                Assert.assertEquals(COLUMN_NODE, schemaTypes.get(0));
                Assert.assertEquals(COLUMN_RELATION, schemaTypes.get(1));
                Assert.assertEquals(COLUMN_SCALAR, schemaTypes.get(2));
                Assert.assertEquals(1, resultSet.size());
                Assert.assertTrue(resultSet.hasNext());
                Record record = resultSet.next();
                Assert.assertFalse(resultSet.hasNext());
                Assert.assertEquals(Arrays.asList("a", "r", "a.age"), record.keys());
                Assert.assertEquals(Arrays.asList(expectedNode, expectedEdge, 32), record.values());
            }

            //test for update in local cache
            expectedNode.removeProperty("name");
            expectedNode.removeProperty("age");
            expectedNode.addProperty(lastNameProperty);
            expectedNode.removeLabel("person");
            expectedNode.addLabel("worker");
            expectedNode.setId(2);


            expectedEdge.setRelationshipType("worksWith");
            expectedEdge.setSource(2);
            expectedEdge.setDestination(3);
            expectedEdge.setId(1);

            Assert.assertNotNull(api.query("social", "CREATE (:worker{lastName:'a'})"));
            Assert.assertNotNull(api.query("social", "CREATE (:worker{lastName:'b'})"));
            Assert.assertNotNull(api.query("social", "MATCH (a:worker), (b:worker) WHERE (a.lastName = 'a' AND b.lastName='b')  CREATE (a)-[:worksWith]->(b)"));

            resultSets = IntStream.range(0,16).parallel().
                    mapToObj(i-> api.query("social", "MATCH (a:worker)-[r:worksWith]->(b:worker) RETURN a,r")).
                    collect(Collectors.toList());

            for (ResultSet resultSet : resultSets){
                Assert.assertNotNull(resultSet.getHeader());
                Header header = resultSet.getHeader();
                List<String> schemaNames = header.getSchemaNames();
                List<Header.ResultSetColumnTypes> schemaTypes = header.getSchemaTypes();
                Assert.assertNotNull(schemaNames);
                Assert.assertNotNull(schemaTypes);
                Assert.assertEquals(2, schemaNames.size());
                Assert.assertEquals(2, schemaTypes.size());
                Assert.assertEquals("a", schemaNames.get(0));
                Assert.assertEquals("r", schemaNames.get(1));
                Assert.assertEquals(COLUMN_NODE, schemaTypes.get(0));
                Assert.assertEquals(COLUMN_RELATION, schemaTypes.get(1));
                Assert.assertEquals(1, resultSet.size());
                Assert.assertTrue(resultSet.hasNext());
                Record record = resultSet.next();
                Assert.assertFalse(resultSet.hasNext());
                Assert.assertEquals(Arrays.asList("a", "r"), record.keys());
                Assert.assertEquals(Arrays.asList(expectedNode, expectedEdge), record.values());
            }
        }

        [Fact]
        public void TestAdditionToProcedures()
        {
            Assert.assertNotNull(api.query("social", "CREATE (:person{name:'roi',age:32})"));
            Assert.assertNotNull(api.query("social", "CREATE (:person{name:'amit',age:30})"));
            Assert.assertNotNull(api.query("social", "MATCH (a:person), (b:person) WHERE (a.name = 'roi' AND b.name='amit')  CREATE (a)-[:knows]->(b)"));


            List<ResultSet> resultSets = IntStream.range(0,16).parallel().
                    mapToObj(i-> api.query("social", "MATCH (a:person)-[r:knows]->(b:person) RETURN a,r")).
                    collect(Collectors.toList());

            //expected objects init
            Property nameProperty = new Property("name", ResultSet.ResultSetScalarTypes.PROPERTY_STRING, "roi");
            Property ageProperty = new Property("age", ResultSet.ResultSetScalarTypes.PROPERTY_INTEGER, 32);
            Property lastNameProperty =new Property("lastName", ResultSet.ResultSetScalarTypes.PROPERTY_STRING, "a");

            Node expectedNode = new Node();
            expectedNode.setId(0);
            expectedNode.addLabel("person");
            expectedNode.addProperty(nameProperty);
            expectedNode.addProperty(ageProperty);


            Edge expectedEdge = new Edge();
            expectedEdge.setId(0);
            expectedEdge.setSource(0);
            expectedEdge.setDestination(1);
            expectedEdge.setRelationshipType("knows");


            ResultSet resultSet = api.query("social", "MATCH (a:person)-[r:knows]->(b:person) RETURN a,r");
            Assert.assertNotNull(resultSet.getHeader());
            Header header = resultSet.getHeader();
            List<String> schemaNames = header.getSchemaNames();
            List<Header.ResultSetColumnTypes> schemaTypes = header.getSchemaTypes();
            Assert.assertNotNull(schemaNames);
            Assert.assertNotNull(schemaTypes);
            Assert.assertEquals(2, schemaNames.size());
            Assert.assertEquals(2, schemaTypes.size());
            Assert.assertEquals("a", schemaNames.get(0));
            Assert.assertEquals("r", schemaNames.get(1));
            Assert.assertEquals(COLUMN_NODE, schemaTypes.get(0));
            Assert.assertEquals(COLUMN_RELATION, schemaTypes.get(1));
            Assert.assertEquals(1, resultSet.size());
            Assert.assertTrue(resultSet.hasNext());
            Record record = resultSet.next();
            Assert.assertFalse(resultSet.hasNext());
            Assert.assertEquals(Arrays.asList("a", "r"), record.keys());
            Assert.assertEquals(Arrays.asList(expectedNode, expectedEdge), record.values());

            //test for local cache updates

            expectedNode.removeProperty("name");
            expectedNode.removeProperty("age");
            expectedNode.addProperty(lastNameProperty);
            expectedNode.removeLabel("person");
            expectedNode.addLabel("worker");
            expectedNode.setId(2);
            expectedEdge.setRelationshipType("worksWith");
            expectedEdge.setSource(2);
            expectedEdge.setDestination(3);
            expectedEdge.setId(1);
            Assert.assertNotNull(api.query("social", "CREATE (:worker{lastName:'a'})"));
            Assert.assertNotNull(api.query("social", "CREATE (:worker{lastName:'b'})"));
            Assert.assertNotNull(api.query("social", "MATCH (a:worker), (b:worker) WHERE (a.lastName = 'a' AND b.lastName='b')  CREATE (a)-[:worksWith]->(b)"));
            resultSet = api.query("social", "MATCH (a:worker)-[r:worksWith]->(b:worker) RETURN a,r");
            Assert.assertNotNull(resultSet.getHeader());
            header = resultSet.getHeader();
            schemaNames = header.getSchemaNames();
            schemaTypes = header.getSchemaTypes();
            Assert.assertNotNull(schemaNames);
            Assert.assertNotNull(schemaTypes);
            Assert.assertEquals(2, schemaNames.size());
            Assert.assertEquals(2, schemaTypes.size());
            Assert.assertEquals("a", schemaNames.get(0));
            Assert.assertEquals("r", schemaNames.get(1));
            Assert.assertEquals(COLUMN_NODE, schemaTypes.get(0));
            Assert.assertEquals(COLUMN_RELATION, schemaTypes.get(1));
            Assert.assertEquals(1, resultSet.size());
            Assert.assertTrue(resultSet.hasNext());
            record = resultSet.next();
            Assert.assertFalse(resultSet.hasNext());
            Assert.assertEquals(Arrays.asList("a", "r"), record.keys());
            Assert.assertEquals(Arrays.asList(expectedNode, expectedEdge), record.values());
        }

        [Fact]
        public void TestEscapedQuery() 
        {
            Assert.assertNotNull(api.query("social", "CREATE (:escaped{s1:%s,s2:%s})", "S\"\'", "S\\'\\\""));
            Assert.assertNotNull(api.query("social", "MATCH (n) where n.s1=%s and n.s2=%s RETURN n", "S\"\'", "S\\'\\\""));
            Assert.assertNotNull(api.query("social", "MATCH (n) where n.s1='S\"\\'' RETURN n"));
        }        
    }
}