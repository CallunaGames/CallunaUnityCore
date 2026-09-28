using System;
using System.Collections.Generic;
using System.Linq;

namespace Calluna
{
    /// <summary>
    /// Checks <see cref="ScriptableObjectId"/>s for empty ids and ids used by several objects of the same
    /// type - either breaks lookups by id, e.g. of <see cref="Id{TDefinition}"/> keys or saved ids.
    /// The editor's ScriptableObjectIdValidator runs it on all id assets of a project.
    /// </summary>
    public static class ScriptableObjectIdValidation
    {
        /// <param name="ids">The ids to check, each with a location (e.g. its asset path) for the messages.</param>
        /// <returns>One message per empty or duplicated id; empty if all ids are fine.</returns>
        public static IReadOnlyList<string> FindProblems(IEnumerable<(ScriptableObjectId id, string location)> ids)
        {
            List<string> problems = new List<string>();
            foreach (IGrouping<Type, (ScriptableObjectId id, string location)> ofType in ids.GroupBy(entry => entry.id.GetType()))
            {
                foreach ((ScriptableObjectId id, string location) in ofType)
                {
                    if (string.IsNullOrEmpty(id.Id))
                        problems.Add($"{ofType.Key.Name} '{id.name}' has no id ({location}).");
                }

                IEnumerable<IGrouping<string, (ScriptableObjectId id, string location)>> duplicates = ofType
                    .Where(entry => !string.IsNullOrEmpty(entry.id.Id))
                    .GroupBy(entry => entry.id.Id, StringComparer.Ordinal)
                    .Where(sameId => sameId.Count() > 1);
                foreach (IGrouping<string, (ScriptableObjectId id, string location)> sameId in duplicates)
                {
                    problems.Add($"{ofType.Key.Name} id '{sameId.Key}' is used several times: " +
                                 string.Join(", ", sameId.Select(entry => entry.location)));
                }
            }
            return problems;
        }
    }
}
