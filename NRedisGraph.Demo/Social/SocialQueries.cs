using System.Collections;
using System.Collections.Generic;

namespace NRedisGraph.Demo.Social
{
    public class SocialQueries : IEnumerable<QueryInfo>
    {
        public IEnumerator<QueryInfo> GetEnumerator() => GetQueries().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetQueries().GetEnumerator();

        private static IEnumerable<QueryInfo> GetQueries()
        {
            yield return new QueryInfo
            {
                Query = "MATCH (e) RETURN e.name, LABELS(e) as label ORDER BY label, e.name",
                Description = "Returns each node in the graph, specifing node label."
            };

            yield return new QueryInfo
            {
                Query = "MATCH ()-[e]->() RETURN TYPE(e) as relation_type, COUNT(e) as num_relations ORDER BY relation_type, num_relations",
                Description = "Returns each relation type in the graph and its count."
            };

            yield return new QueryInfo
            {
                Query = "MATCH (p:person) RETURN p.name ORDER BY p.name SKIP 3 LIMIT 5",
                Description = "Get a subset of people."
            };

            yield return new QueryInfo
            {
                Query = "MATCH (ME:person {name:\"Roi Lipman\"})-[:friend]->(f:person) RETURN f.name",
                Description = "My friends?"
            };

            yield return new QueryInfo
            {
                Query = "MATCH (ME:person {name:\"Roi Lipman\"})-[:friend]->(:person)-[:friend]->(fof:person) RETURN fof.name",
                Description = "Friends of friends?"
            };

            yield return new QueryInfo
            {
                Query = "MATCH (ME:person {name:\"Roi Lipman\"})-[:friend]->(:person)-[:friend]->(fof:person {status:\"single\"}) WHERE fof.age > 30 RETURN fof.name, fof.age, fof.gender, fof.status",
                Description = "Friends of friends who are single and over 30?"
            };

            yield return new QueryInfo
            {
                Query = "MATCH (ME:person {name:\"Roi Lipman\"})-[:friend]->(:person)-[:friend]->(fof:person {status:\"single\"})-[:visited]->(:country {name:\"Netherlands\"}) RETURN fof.name ORDER BY fof.name",
                Description = "Friends of friends who visited Netherlands and are single?"
            };

            yield return new QueryInfo
            {
                Query = "MATCH (ME:person {name:\"Roi Lipman\"})-[:visited]->(c:country)<-[:visited]-(f:person)<-[:friend]-(ME) RETURN f.name, c.name ORDER BY f.name, c.name",
                Description = "Friends who have been to places I have visited?"
            };

            yield return new QueryInfo
            {
                Query = "MATCH (A:person {name:\"Roi Lipman\"})-[:visited]->(X:country), (B:person {name:\"Tal Doron\"})-[:visited]->(X), (C:person {name:\"Boaz Arad\"})-[:visited]->(X) RETURN X.name",
                Description = "Countries visited by Roi, Tal and Boaz."
            };

            yield return new QueryInfo
            {
                Query = "MATCH (ME:person {name:\"Roi Lipman\"})-[:friend]->(f:person) WHERE f.age > ME.age RETURN f.name, f.age",
                Description = "Friends who are older than me?"
            };

            yield return new QueryInfo
            {
                Query = "MATCH (ME:person {name:\"Roi Lipman\"})-[:friend]->(f:person) RETURN f.name, abs(ME.age - f.age) AS age_diff ORDER BY age_diff desc",
                Description = "Age difference between me and each of my friends."
            };

            yield return new QueryInfo
            {
                Query = "MATCH (p:person) WITH avg(p.age) AS average_age MATCH(:person)-[:friend]->(f:person) WHERE f.age > average_age RETURN f.name, f.age, round(f.age - average_age) AS age_diff ORDER BY age_diff DESC, f.name DESC LIMIT 4",
                Description = "Friends who are older then the average age."
            };

            yield return new QueryInfo
            {
                Query = "MATCH (ME:person {name:\"Roi Lipman\"})-[:friend]->(friend:person)-[:visited]->(c:country) RETURN friend.name, count(c.name) AS countriesVisited ORDER BY countriesVisited DESC LIMIT 10",
                Description = "Count for each friend how many countires he or she been to?"
            };

            yield return new QueryInfo
            {
                Query = "MATCH (:person {name:\"Roi Lipman\"})-[:friend]->(f:person) SET f.age = f.age + 1 RETURN f.name, f.age order by f.name, f.age",
                Description = "Update friends age."
            };

            yield return new QueryInfo
            {
                Query = "MATCH (ME:person {name:\"Roi Lipman\"})-[:friend]->(f:person) RETURN ME.name, count(f.name), sum(f.age), avg(f.age), min(f.age), max(f.age)",
                Description = "Friends age statistics."
            };

            yield return new QueryInfo
            {
                Query = "MATCH (ME:person {name:\"Roi Lipman\"})-[v:visited]->(c:country) RETURN c.name, v.purpose",
                Description = "For each country i have been to, what was the visit purpose?"
            };

            yield return new QueryInfo
            {
                Query = "MATCH (p:person)-[v:visited {purpose:\"business\"}]->(c:country) RETURN p.name, v.purpose, toUpper(c.name) ORDER BY p.name, c.name",
                Description = "Find out who went on a business trip?"
            };

            yield return new QueryInfo
            {
                Query = "MATCH (p:person)-[v:visited {purpose:\"pleasure\"}]->(c:country) RETURN p.name, count(v.purpose) AS vacations ORDER BY COUNT(v.purpose) DESC, p.name DESC LIMIT 6",
                Description = "Count number of vacations per person?"
            };

            yield return new QueryInfo
            {
                Query = "MATCH (a:person {name:'Roi Lipman'})-[:friend*]->(b:person) RETURN b.name ORDER BY b.name",
                Description = "Find all reachable friends."
            };

            yield return new QueryInfo
            {
                Query = "MATCH (a:person {name:'Roi Lipman'})-[*]->(c:country) RETURN c.name, count(c.name) AS NumPathsToCountry ORDER BY NumPathsToCountry, c.name DESC",
                Description = "Find all reachable countries."
            };

            yield return new QueryInfo
            {
                Query = "MATCH (s:person {name:'Roi Lipman'})-[e:friend|:visited]->(t) RETURN s.name,TYPE(e),t.name ORDER BY t.name",
                Description = "Every person or country one hop away from source node."
            };

            yield return new QueryInfo
            {
                Query = "MATCH (a:person {name:'Roi Lipman'})-[:friend|:visited*]->(e) RETURN e.name, count(e.name) AS NumPathsToEntity ORDER BY NumPathsToEntity, e.name DESC",
                Description = "Every reachable person or country from source node."
            };

            yield return new QueryInfo
            {
                Query = "MATCH (a:person {name:'Roi Lipman'})-[*]->(e) RETURN e.name, count(e.name) AS NumPathsToEntity ORDER BY NumPathsToEntity DESC",
                Description = "Find all reachable entities."
            };

            yield return new QueryInfo
            {
                Query = "MATCH (ME:person {name:'Roi Lipman'})-[*2..]->(e:person) RETURN e.name ORDER BY e.name",
                Description = "Find all reachable people at least 2 hops away from me."
            };

            yield return new QueryInfo
            {
                Query = "MATCH (a)-[*]->(e:country {name:'Greece'}) RETURN count(a.name) AS NumPathsToGreece",
                Description = "Number of paths leading to Greece."
            };

            yield return new QueryInfo
            {
                Query = "MATCH (ME:person {name:'Roi Lipman'})-[:visited]->(c:country)<-[*]-(ME) RETURN c.name, count(c) ORDER BY c.name",
                Description = "Count number of paths to places I have visited."
            };

            yield return new QueryInfo
            {
                Query = "CALL algo.pageRank('person', 'friend') YIELD node, score RETURN node.name ORDER BY score DESC",
                Description = "Pagerank friends."
            };

            // yield return new QueryInfo
            // {
            //     Query = "MATCH (ME:person {name:'Roi Lipman'})-[e:friend]->() DELETE e",
            //     Description = "Delete frienships."
            // };

            // yield return new QueryInfo
            // {
            //     Query = "MATCH (ME:person {name:'Roi Lipman'}) DELETE ME",
            //     Description = "Delete myself from the graph."
            // };

            yield return new QueryInfo
            {
                Query = "MATCH (p:person) RETURN p.name",
                Description = "Retrieve all nodes with person label"
            };
        }
    }
}