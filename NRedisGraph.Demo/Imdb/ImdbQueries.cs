using System;
using System.Collections;
using System.Collections.Generic;

namespace NRedisGraph.Demo.Imdb
{
    public class ImdbQueries : IEnumerable<QueryInfo>
    {
        public IEnumerator<QueryInfo> GetEnumerator() => GetQueries().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetQueries().GetEnumerator();

        private static IEnumerable<QueryInfo> GetQueries()
        {
            yield return new QueryInfo
            {
                Query = "MATCH (n:actor) RETURN count(n) as actors_count",
                Description = "How many actors are in the graph?",
                MaxRuntime = TimeSpan.FromMilliseconds(0.2),
                ExpectedResult = null
            };

            yield return new QueryInfo
            {
                Query = "MATCH (n:actor {name:\"Nicolas Cage\"})-[:act]->(m:movie)<-[:act]-(a:actor) RETURN a.name, m.title",
                Description = "Which actors played along side Nicolage Cage?",
                MaxRuntime = TimeSpan.FromMilliseconds(4),
                ExpectedResult = null
            };

            yield return new QueryInfo
            {
                Query = "MATCH (nicolas:actor {name:\"Nicolas Cage\"})-[:act]->(m:movie)<-[:act]-(a:actor) RETURN a.name, m.title ORDER BY a.name, m.title LIMIT 3",
                Description = "Get 3 actors who have played along side Nicolas Cage?",
                MaxRuntime = TimeSpan.FromMilliseconds(3),
                ExpectedResult = null
            };

            yield return new QueryInfo
            {
                Query = "MATCH (a:actor)-[:act]->(m:movie {title:\"Straight Outta Compton\"}) RETURN a.name",
                Description = "Which actors played in the movie Straight Outta Compton?",
                MaxRuntime = TimeSpan.FromMilliseconds(3.5),
                ExpectedResult = null
            };

            yield return new QueryInfo
            {
                Query = "MATCH (a:actor)-[:act]->(m:movie) WHERE a.age >= 50 AND m.votes > 10000 AND m.rating > 8.2 RETURN a, m ORDER BY a.name, m.name",
                Description = "Which actors who are over 50 played in blockbuster movies?",
                MaxRuntime = TimeSpan.FromMilliseconds(4),
                ExpectedResult = null
            };

            yield return new QueryInfo
            {
                Query = "MATCH (a:actor)-[:act]->(m:movie) WHERE (m.genre = \"Drama\" OR m.genre = \"Comedy\") AND m.rating < 5.5 AND m.votes > 50000 RETURN a.name, m.title, m.genre ORDER BY m.rating",
                Description = "Which actors played in bad drama or comedy?",
                MaxRuntime = TimeSpan.FromMilliseconds(4),
                ExpectedResult = null
            };

            yield return new QueryInfo
            {
                Query = "MATCH (a:actor)-[:act]->(m0:movie {genre:'Action'}), (a)-[:act]->(m1:movie {genre:'Drama'}), (a)-[:act]->(m2:movie {genre:'Comedy'}) RETURN DISTINCT a.name",
                Description = "Which actors played in Action, Drama and Comedy movies?",
                MaxRuntime = TimeSpan.FromMilliseconds(1.5),
                ExpectedResult = null
            };

            yield return new QueryInfo
            {
                Query = "MATCH (Cameron:actor {name:\"Cameron Diaz\"})-[:act]->(m:movie)<-[:act]-(a:actor) WHERE a.age < 35 RETURN a, m.title ORDER BY m.title",
                Description = "Which young actors played along side Cameron Diaz?",
                MaxRuntime = TimeSpan.FromMilliseconds(5),
                ExpectedResult = null
            };

            yield return new QueryInfo
            {
                Query = "MATCH (Cameron:actor {name:\"Cameron Diaz\"})-[:act]->(m:movie)<-[:act]-(a:actor) WHERE a.age < Cameron.age RETURN a, m.title order by a.name",
                Description = "Which actors played along side Cameron Diaz and are younger then her?",
                MaxRuntime = TimeSpan.FromMilliseconds(7),
                ExpectedResult = null
            };

            yield return new QueryInfo
            {
                Query = "MATCH (a:actor)-[:act]->(m:movie{title:\"Straight Outta Compton\"}) RETURN m.title, SUM(a.age), AVG(a.age)",
                Description = "What is the sum and average age of the Straight Outta Compton cast?",
                MaxRuntime = TimeSpan.FromMilliseconds(4),
                ExpectedResult = null
            };

            yield return new QueryInfo
            {
                Query = "MATCH (Cameron:actor{name:\"Cameron Diaz\"})-[:act]->(m:movie) RETURN Cameron.name, COUNT(m.title)",
                Description = "In how many movies did Cameron Diaz played?",
                MaxRuntime = TimeSpan.FromMilliseconds(1.2),
                ExpectedResult = null
            };

            yield return new QueryInfo
            {
                Query = "MATCH (a:actor) RETURN DISTINCT * ORDER BY a.age DESC LIMIT 10",
                Description = "10 Oldest actors?",
                MaxRuntime = TimeSpan.FromMilliseconds(4.5),
                ExpectedResult = null
            };

            yield return new QueryInfo
            {
                Query = "MATCH (a:actor) WHERE a.age > 85 RETURN * ORDER BY a.age, a.name",
                Description = "Actors over 85 on indexed property?",
                MaxRuntime = TimeSpan.FromMilliseconds(1.5),
                ExpectedResult = null
            };

            yield return new QueryInfo
            {
                Query = "MATCH (m:movie) WHERE 1980 <= m.year AND m.year < 1990 RETURN m.title, m.year ORDER BY m.year",
                Description = "Multiple filters on indexed property?",
                MaxRuntime = TimeSpan.FromMilliseconds(1.5),
                ExpectedResult = null
            };

            yield return new QueryInfo
            {
                Query = "MATCH (m:movie) WHERE LEFT(m.title, 8) = 'American' RETURN m.title ORDER BY m.title",
                Description = "Movies starting with \"American\"?",
                MaxRuntime = TimeSpan.FromMilliseconds(4),
                ExpectedResult = null
            };

            yield return new QueryInfo
            {
                Query = "MATCH (base:movie), (option:movie) WHERE base.title = 'Hunt for the Wilderpeople' AND base.year = option.year AND option.rating > base.rating RETURN option.title, option.rating ORDER BY option.rating, option.title desc LIMIT 10",
                Description = "List 10 movies released on the same year as \"Hunt for the Wilderpeople\" that got higher rating than it.",
                MaxRuntime = TimeSpan.FromMilliseconds(0.8),
                ExpectedResult = null
            };

            yield return new QueryInfo
            {
                Query = "CALL db.idx.fulltext.queryNodes('actor', 'tim') YIELD node RETURN node.name ORDER BY node.name",
                Description = "All actors named Tim.",
                MaxRuntime = TimeSpan.FromMilliseconds(4),
                ExpectedResult = null
            };
        }
    }
}