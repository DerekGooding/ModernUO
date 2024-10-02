/*************************************************************************
 * ModernUO                                                              *
 * Copyright 2019-2024 - ModernUO Development Team                       *
 * Email: hi@modernuo.com                                                *
 * File: ISerializableExtensions.cs                                      *
 *                                                                       *
 * This program is free software: you can redistribute it and/or modify  *
 * it under the terms of the GNU General Public License as published by  *
 * the Free Software Foundation, either version 3 of the License, or     *
 * (at your option) any later version.                                   *
 *                                                                       *
 * You should have received a copy of the GNU General Public License     *
 * along with this program.  If not, see <http://www.gnu.org/licenses/>. *
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Server;

public static class ISerializableExtensions
{
    /// <summary>
    /// Currently in development
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void MarkDirty(this ISerializable entity)
    {
        // TODO: Add dirty tracking back
    }

    /// <summary>
    /// 
    /// <para>Marks this <see cref="ISerializable"/> object as dirty</para>
    /// </summary>
    /// <param name="toDelete"><see cref="IEntity"/> to be deleted.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Delete(this ISerializable entity, IEntity toDelete)
    {
        toDelete?.Delete();
        entity.MarkDirty();
    }

    /// <summary>
    /// Adds an item to the <see cref="ICollection{T}"/>.
    /// <para>Marks this <see cref="ISerializable"/> object as dirty</para>
    /// </summary>
    /// <param name="item">The object to add to the collection.</param>
    /// <exception cref="NotSupportedException">Thrown if the collection is read-only.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Add<T>(this ISerializable entity, ICollection<T> list, T item)
    {
        list.Add(item);
        entity.MarkDirty();
    }

    /// <summary>
    /// Adds the specified key and value to the <see cref="IDictionary"/>.
    /// <para>Marks this <see cref="ISerializable"/> object as dirty</para>
    /// </summary>
    /// <param name="key">The object to use as the key of the element to add.</param>
    /// <param name="value">The object to use as the value of the element to add.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if an element with the same key already exists in the <see cref="IDictionary"/>.</exception>
    /// <exception cref="NotSupportedException">Thrown if the <see cref="IDictionary"/> is read-only.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Add<K, V>(this ISerializable entity, IDictionary<K, V> dict, K key, V value)
    {
        dict.Add(key, value);
        entity.MarkDirty();
    }

    /// <summary>
    /// Determines whether the <see cref="IDictionary"/> contains an element with the specified key.
    /// Tries to update a specific key with a value.
    /// <para>Marks this <see cref="ISerializable"/> object as dirty</para>
    /// </summary>
    /// <param name="key">The key to locate in the <see cref="IDictionary"/>.</param>
    /// <param name="value">The value to update with.</param>
    /// <returns><see langword="true"/> if the <see cref="IDictionary"/> contains an element with the key; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryUpdate<K, V>(this ISerializable entity, IDictionary<K, V> dict, K key, V value)
    {
        if (dict.ContainsKey(key))
        {
            dict[key] = value;
            entity.MarkDirty();
            return true;
        }
        return false;
    }

    /// <summary>
    /// 
    /// <para>Marks this <see cref="ISerializable"/> object as dirty</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="value"></param>
    /// <param name="index"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Insert<T>(this ISerializable entity, IList<T> list, T value, int index)
    {
        list.Insert(index, value);
        entity.MarkDirty();
    }

    /// <summary>
    /// 
    /// <para>Marks this <see cref="ISerializable"/> object as dirty</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Remove<T>(this ISerializable entity, ICollection<T> list, T value)
    {
        if (list.Remove(value))
        {
            entity.MarkDirty();
            return true;
        }

        return false;
    }

    /// <summary>
    /// 
    /// <para>Marks this <see cref="ISerializable"/> object as dirty</para>
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Remove<K, V>(this ISerializable entity, IDictionary<K, V> dict, K key, out V value)
    {
        if (dict.Remove(key, out value))
        {
            entity.MarkDirty();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Removes the item at the specified index of the <see cref="IList"/>.
    /// <para>Marks this <see cref="ISerializable"/> object as dirty</para>
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="index"/> is not a valid index in the list.
    /// </exception>
    /// <exception cref="NotSupportedException">Thrown if the <see cref="IList"/> is read-only.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveAt<T>(this ISerializable entity, IList<T> list, int index)
    {
        list.RemoveAt(index);
        entity.MarkDirty();
    }

    /// <summary>
    /// Removes all items from the <see cref="ICollection{T}"/>.
    /// <para>Marks this <see cref="ISerializable"/> object as dirty</para>
    /// </summary>
    /// <exception cref="NotSupportedException">Thrown if the collection is read-only.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(this ISerializable entity, ICollection<T> list)
    {
        list.Clear();
        entity.MarkDirty();
    }

    /// <summary>
    /// 
    /// <para>Marks this <see cref="ISerializable"/> object as dirty</para>
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="timer"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Stop(this ISerializable entity, Timer timer)
    {
        timer?.Stop();
        entity.MarkDirty();
    }

    /// <summary>
    /// 
    /// <para>Marks this <see cref="ISerializable"/> object as dirty</para>
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="timer"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Start(this ISerializable entity, Timer timer)
    {
        timer?.Start();
        entity.MarkDirty();
    }

    /// <summary>
    /// 
    /// <para>Marks this <see cref="ISerializable"/> object as dirty</para>
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="timer"></param>
    /// <param name="delay"></param>
    /// <param name="interval"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Restart(this ISerializable entity, Timer timer, TimeSpan delay, TimeSpan interval)
    {
        if (timer != null)
        {
            timer.Stop();
            timer.Delay = delay;
            timer.Interval = interval;
            timer.Start();
            entity.MarkDirty();
        }
    }

    /// <summary>
    /// 
    /// <para>Marks this <see cref="ISerializable"/> object as dirty</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <param name="list"></param>
    /// <param name="value"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Add<T>(this ISerializable entity, ref List<T> list, T value)
    {
        Utility.Add(ref list, value);
        entity.MarkDirty();
    }

    /// <summary>
    /// 
    /// <para>Marks this <see cref="ISerializable"/> object as dirty</para>
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="entity"></param>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Add<K, V>(this ISerializable entity, ref Dictionary<K, V> dict, K key, V value)
    {
        Utility.Add(ref dict, key, value);
        entity.MarkDirty();
    }

    /// <summary>
    /// 
    /// <para>Marks this <see cref="ISerializable"/> object as dirty</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <param name="list"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Remove<T>(this ISerializable entity, ref List<T> list, T value)
    {
        if (Utility.Remove(ref list, value))
        {
            entity.MarkDirty();
            return true;
        }

        return false;
    }

    /// <summary>
    /// 
    /// <para>Marks this <see cref="ISerializable"/> object as dirty</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <param name="list"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(this ISerializable entity, ref List<T> list)
    {
        Utility.Clear(ref list);
        entity.MarkDirty();
    }

    /// <summary>
    /// 
    /// <para>Marks this <see cref="ISerializable"/> object as dirty</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <param name="set"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<T>(this ISerializable entity, ref HashSet<T> set)
    {
        Utility.Clear(ref set);
        entity.MarkDirty();
    }

    /// <summary>
    /// 
    /// <para>Marks this <see cref="ISerializable"/> object as dirty</para>
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="entity"></param>
    /// <param name="dict"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear<K, V>(this ISerializable entity, ref Dictionary<K, V> dict)
    {
        Utility.Clear(ref dict);
        entity.MarkDirty();
    }

    /// <summary>
    /// 
    /// <para>Marks this <see cref="ISerializable"/> object as dirty</para>
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="timer"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Stop(this ISerializable entity, ref Timer timer)
    {
        timer?.Stop();
        timer = null;
        entity.MarkDirty();
    }
}
