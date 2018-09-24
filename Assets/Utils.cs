using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rnd = UnityEngine.Random;

namespace Assets
{
    static class Utils
    {
        /// <summary>
        ///     Brings the elements of the given list into a random order.</summary>
        /// <typeparam name="T">
        ///     Type of the list.</typeparam>
        /// <param name="list">
        ///     List to shuffle.</param>
        /// <param name="rnd">
        ///     Random number generator, or null to use <see cref="Rnd"/>.</param>
        /// <returns>
        ///     The list operated on.</returns>
        public static T Shuffle<T>(this T list, Random rnd = null) where T : IList
        {
            if (list == null)
                throw new ArgumentNullException("list");
            for (int j = list.Count; j >= 1; j--)
            {
                int item = Rnd.Range(0, j);
                if (item < j - 1)
                {
                    var t = list[item];
                    list[item] = list[j - 1];
                    list[j - 1] = t;
                }
            }
            return list;
        }

        /// <summary>
        ///     Returns a random element from the specified collection.</summary>
        /// <typeparam name="T">
        ///     The type of the elements in the collection.</typeparam>
        /// <param name="src">
        ///     The collection to pick from.</param>
        /// <param name="rnd">
        ///     Optionally, a random number generator to use.</param>
        /// <returns>
        ///     The element randomly picked.</returns>
        /// <remarks>
        ///     This method enumerates the entire input sequence into an array.</remarks>
        public static T PickRandom<T>(this IEnumerable<T> src, Random rnd = null)
        {
            var list = (src as IList<T>) ?? src.ToArray();
            if (list.Count == 0)
                throw new InvalidOperationException("Cannot pick an element from an empty set.");
            return list[Rnd.Range(0, list.Count)];
        }

        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialisation method. NOTE: Private members are not cloned using this method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }
    }
}
