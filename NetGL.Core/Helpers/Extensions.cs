using NetGL.Core.Mathematics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public static class Extensions {
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T> action) {
        foreach (var item in items)
            action(item);

        return items;
    }
    public static IEnumerable ForEach<T>(this IEnumerable items, Action<object> action) {
        foreach (var item in items)
            action(item);

        return items;
    }
    public static List<T> ForEach<T>(this List<T> items, Action<T> action) {
        for (int i = 0; i < items.Count; i++)
            action(items[i]);

        return items;
    }
    public static IReadOnlyList<T> ForEach<T>(this IReadOnlyList<T> items, Action<T> action) {
        for (int i = 0; i < items.Count; i++)
            action(items[i]);

        return items;
    }
    public static IList ForEach<T>(this IList items, Action<object> action) {
        for (int i = 0; i < items.Count; i++)
            action(items[i]);

        return items;
    }
    public static T[] ForEach<T>(this T[] items, Action<T> action) {
        for (int i = 0; i < items.Length; i++)
            action(items[i]);

        return items;
    }

    public static Vector3 Sum(this List<Vector3> items) {
        var result = new Vector3();

        for (int i = 0; i < items.Count; i++)
            result += items[i];

        return result;
    }
    public static Vector3 Sum(this IReadOnlyList<Vector3> items) {
        var result = new Vector3();

        for (int i = 0; i < items.Count; i++)
            result += items[i];

        return result;
    }
    public static Vector3 Sum(this Vector3[] items) {
        var result = new Vector3();

        for (int i = 0; i < items.Length; i++)
            result += items[i];

        return result;
    }
    public static IEnumerable<Vector3> Sum(this IEnumerable<Vector3> items) {
        var result = new Vector3();

        foreach (var item in items)
            result += item;

        return items;
    }

    [Obsolete("generates GC")]
    public static List<T> ForEachReverse<T>(this List<T> items, Action<int, T> action) {
        for (int i = items.Count - 1; i >= 0; i--)
            action(i, items[i]);

        return items;
    }
    [Obsolete("generates GC")]
    public static IReadOnlyList<T> ForEachReverse<T>(this IReadOnlyList<T> items, Action<int, T> action) {
        for (int i = items.Count - 1; i >= 0; i--)
            action(i, items[i]);

        return items;
    }
    [Obsolete("generates GC")]
    public static IList ForEachReverse<T>(this IList items, Action<int, object> action) {
        for (int i = items.Count - 1; i >= 0; i--)
            action(i, items[i]);

        return items;
    }

    public static double GetEllapsedSeconds(this Stopwatch sw) {
        return sw.ElapsedTicks / (double)Stopwatch.Frequency;
    }
}