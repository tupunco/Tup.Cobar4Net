using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tup.Cobar4Net
{
    /// <summary>
    ///     集合处理 工具类
    /// </summary>
    public static class CollectionHelper
    {
        #region AsReadOnly

        /// <summary>
        ///     IDictionary As ReadOnly
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dic)
        {
            ThrowHelper.ThrowIfNull(dic, "dic");

            if (dic is IReadOnlyDictionary<TKey, TValue>)
                return dic;

            return new ReadOnlyDictionary<TKey, TValue>(dic);
        }

        /// <summary>
        ///     ICollection As ReadOnly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static ICollection<T> AsReadOnly<T>(this ICollection<T> input)
        {
            ThrowHelper.ThrowIfNull(input, "input");

            if (input is ReadOnlyCollection<T> || input is IReadOnlyCollection<T>)
                return input;

            if (input is IList<T>)
                return new ReadOnlyCollection<T>(input as IList<T>);
            return new ReadOnlyCollection<T>(new List<T>(input));
        }

        #endregion

        #region IsEmpty

        /// <summary>
        ///     指示指定类型的 数组对象是 null 或者 Length = 0。
        /// </summary>
        /// <param name="input">array to check</param>
        /// <returns>bool</returns>
        public static bool IsEmpty<T>(this T[] input)
        {
            return input == null || input.Length <= 0;
        }

        /// <summary>
        ///     指示指定类型的 数组对象是 null 或者 Length = 0。
        /// </summary>
        /// <param name="input">array to check</param>
        /// <returns>bool</returns>
        public static bool IsEmpty<T>(this ICollection<T> input)
        {
            return input == null || input.Count <= 0;
        }

        #endregion

        #region AddRange

        /// <summary>
        ///     AddRange ICollection
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="addCollection"></param>
        public static void AddRange<TInput>(this ICollection<TInput> collection, IEnumerable<TInput> addCollection)
        {
            if (collection == null || addCollection == null)
                return;

            foreach (var item in addCollection)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        ///     AddRange IDictionary
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="addCollection"></param>
        public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> collection,
                                                  IDictionary<TKey, TValue> addCollection)
        {
            if (collection == null || addCollection == null)
                return;

            foreach (var item in addCollection)
            {
                collection[item.Key] = item.Value;
            }
        }

        /// <summary>
        ///     AddRange ICollection
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="addCollection"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        public static void AddRange<TInput>(this ICollection<TInput> collection,
                                            IEnumerable<TInput> addCollection,
                                            int startIndex, int count)
        {
            if (collection == null || addCollection == null || count <= 0)
                return;

            if (startIndex < 0) startIndex = 0;
            foreach (var item in addCollection)
            {
                if (startIndex > 0)
                {
                    startIndex--;
                    continue;
                }

                collection.Add(item);
                count--;
                if (count <= 0)
                    break;
            }
        }

        /// <summary>
        ///     AddRange To Queue
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="collection"></param>
        public static void AddRange<TInput>(this Queue<TInput> collection, IEnumerable<TInput> addCollection)
        {
            if (collection == null || addCollection == null)
                return;

            foreach (var item in addCollection)
            {
                collection.Enqueue(item);
            }
        }

        /// <summary>
        ///     AddRange To Stack
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="collection"></param>
        public static void AddRange<TInput>(this Stack<TInput> collection, IEnumerable<TInput> addCollection)
        {
            if (collection == null || addCollection == null)
                return;

            foreach (var item in addCollection)
            {
                collection.Push(item);
            }
        }

        #endregion

        #region Dictionary GetValue

        /// <summary>
        ///     GetValue From Dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> obj, TKey key)
        {
            return GetValue(obj, key, default(TValue));
        }

        /// <summary>
        ///     GetValue From Dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> obj,
                                                    TKey key, TValue defaultValue)
        {
            if (obj == null)
                return defaultValue;

            var tObj = default(TValue);
            if (obj.TryGetValue(key, out tObj))
                return tObj;

            return defaultValue;
        }

        #endregion
    }
}