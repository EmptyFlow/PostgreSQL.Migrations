using Xunit.Abstractions;
using Xunit.Sdk;

namespace PostgreSQL.Migrations.UnitTests.IntegrationTests {

    public class PriorityOrderer : ITestCaseOrderer {

        public IEnumerable<TTestCase> OrderTestCases<TTestCase> ( IEnumerable<TTestCase> testCases ) where TTestCase : ITestCase {
            var sortedMethods = new SortedDictionary<int, List<TTestCase>> ();

            foreach ( TTestCase testCase in testCases ) {
                int priority = 0;

                foreach ( IAttributeInfo attr in testCase.TestMethod.Method.GetCustomAttributes ( typeof ( TestPriorityAttribute ).AssemblyQualifiedName ) ) {
                    priority = attr.GetNamedArgument<int> ( "Priority" );
                }

                GetOrCreate ( sortedMethods, priority ).Add ( testCase );
            }

            foreach ( var list in sortedMethods.Keys.Select ( priority => sortedMethods[priority] ) ) {
                list.Sort ( ( x, y ) => StringComparer.OrdinalIgnoreCase.Compare ( x.TestMethod.Method.Name, y.TestMethod.Method.Name ) );
                foreach ( TTestCase testCase in list ) {
                    yield return testCase;
                }
            }
        }

        static TValue GetOrCreate<TKey, TValue> ( IDictionary<TKey, TValue> dictionary, TKey key ) where TValue : new() {
            TValue result;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            if ( dictionary.TryGetValue ( key, out result ) ) return result;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            result = new TValue ();
            dictionary[key] = result;

            return result;
        }

    }

}
