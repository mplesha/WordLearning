using System.Collections.Generic;

namespace BusinessLayer.ExtensionMethods
{
    public static class ExtensionMethodsIEnumerable
    {
        public static IEnumerable<T> Add<T>(this IEnumerable<T> e, T value)
        {
            foreach (var cur in e)
            {
                yield return cur;
            }
            yield return value;
        }

        public static IEnumerable<T> AddOnBeginning<T>(this IEnumerable<T> e, T value)
        {
            yield return value;
            foreach (var cur in e)
            {
                yield return cur;
            }
        }

        public static IEnumerable<T> RemoveFirst<T>(this IEnumerable<T> e, T value)
        {
            List<T> tmplList = new List<T>(e);
            tmplList.Remove(value);
            return tmplList;
        }
    }
}
