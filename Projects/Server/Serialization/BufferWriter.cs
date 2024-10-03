/*************************************************************************
 * ModernUO                                                              *
 * Copyright 2019-2024 - ModernUO Development Team                       *
 * Email: hi@modernuo.com                                                *
 * File: BufferWriter.cs                                                 *
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
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Server.Text;

namespace Server;

/// <summary>
/// Writes bits of data to a buffer in memory in little-endian format.
/// </summary>
public class BufferWriter : IGenericWriter
{
    private readonly ConcurrentQueue<Type> _types;
    private readonly Encoding _encoding;
    private readonly bool _prefixStrings;
    private long _bytesWritten;
    private long _index;

    protected long Index
    {
        get => _index;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value)
                    , "If you are receiving this exception and your value is negative, you probably used Seek incorrectly.");
            }
            if(value > _buffer.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(value)
                    , "If you are receiving this exception and your value is too large, you may need to use `Resize`.");
            }

            _index = value;

            if (value > _bytesWritten)
            {
                _bytesWritten = value;
            }
        }
    }

    private byte[] _buffer;

    /// <summary>
    /// Writes bits of data to a buffer in memory.
    /// </summary>
    /// <param name="buffer">The array of <see cref="byte"/> to be wrapped and managed by this <see cref="BufferWriter"/></param>
    /// <param name="prefixStr">Specifies whether strings will be prefixed with a bool byte to identify it is a string.</param>
    /// <param name="types"></param>
    public BufferWriter(byte[] buffer, bool prefixStr, ConcurrentQueue<Type> types = null)
    {
        _prefixStrings = prefixStr;
        _encoding = TextEncoding.UTF8;
        _buffer = buffer;
        _types = types;
    }

    /// <summary>
    /// Writes bits of data to a buffer in memory.
    /// </summary>
    /// <param name="prefixStr">Specifies whether strings will be prefixed with a bool byte to identify it is a string.</param>
    /// <param name="types"></param>
    public BufferWriter(bool prefixStr, ConcurrentQueue<Type> types = null) : this(0, prefixStr, types)
    {
    }

    /// <summary>
    /// Writes bits of data to a buffer in memory.
    /// </summary>
    /// <param name="count">Initializes a <see cref="byte"/> array of length <paramref name="count"/></param>
    /// <param name="prefixStr">Specifies whether strings will be prefixed with a bool byte to identify it is a string.</param>
    /// <param name="types"></param>
    public BufferWriter(int count, bool prefixStr, ConcurrentQueue<Type> types = null)
    {
        _prefixStrings = prefixStr;
        _encoding = TextEncoding.UTF8;
        _buffer = GC.AllocateUninitializedArray<byte>(count < 1 ? BufferSize : count);
        _types = types;
    }

    public virtual long Position => Index;

    protected virtual int BufferSize => 256;

    /// <summary>
    /// The wrapped buffer as an array of <see cref="byte"/>.
    /// </summary>
    public byte[] Buffer => _buffer;

    public virtual void Close()
    {
    }

    /// <summary>
    /// Change the buffer size. Cannot be set to less than 1.Buffer lengths of 0 or less are dangerous.
    /// </summary>
    /// <param name="size">The new size of the buffer. If less than 1, will not change the size.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Resize(int size)
    {
        if (size <= 0)
        {
            size = BufferSize;
        }

        if (size < _buffer.Length)
        {
            _bytesWritten = size;
        }

        var newBuffer = GC.AllocateUninitializedArray<byte>(size);
        _buffer.AsSpan(0, Math.Min(size, _buffer.Length)).CopyTo(newBuffer);
        _buffer = newBuffer;
    }

    public virtual void Flush() => Resize(Math.Clamp(_buffer.Length * 2, BufferSize, _buffer.Length + 1024 * 1024 * 64));

    /// <summary>
    /// Will call <see cref="Flush"/> if the <paramref name="amount"/> is greater than the empty room in the buffer.
    /// </summary>
    /// <param name="amount">The amount of room to check. If greater than remaining room in the buffer, call <see cref="Flush"/></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FlushIfNeeded(int amount)
    {
        if (amount > _buffer.Length - Index)
        {
            Flush();
        }
    }

    public virtual void Write(byte[] bytes) => Write(bytes.AsSpan());

    public virtual void Write(byte[] bytes, int offset, int count) => Write(bytes.AsSpan(offset, count));

    public virtual void Write(ReadOnlySpan<byte> bytes)
    {
        var length = bytes.Length;

        while (_buffer.Length - _index < length)
        {
            Flush();
        }

        bytes.CopyTo(_buffer.AsSpan((int)_index));
        Index += length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual long Seek(long offset, SeekOrigin origin)
    {
        Debug.Assert(
            origin != SeekOrigin.End || offset <= 0 && offset > -_buffer.Length,
            "Attempting to seek to an invalid position using SeekOrigin.End"
        );
        Debug.Assert(
            origin != SeekOrigin.Begin || offset >= 0 && offset < _buffer.Length,
            "Attempting to seek to an invalid position using SeekOrigin.Begin"
        );
        Debug.Assert(
            origin != SeekOrigin.Current || Index + offset >= 0 && Index + offset < _buffer.Length,
            "Attempting to seek to an invalid position using SeekOrigin.Current"
        );

        return Index = Math.Max(0, origin switch
        {
            SeekOrigin.Current => Index + offset,
            SeekOrigin.End     => _bytesWritten + offset,
            _                  => offset // Begin
        });
    }

    /// <summary>
    /// Writes a <see cref="string"/> to the buffer. If PrefixStrings is <see langword="true"/> then write a preceeding identifier byte.
    /// </summary>
    /// <param name="value">The <see cref="string"/> to be written as <see cref="byte"/> data to the buffer.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(string value)
    {
        if (_prefixStrings)
        {
            if (value == null)
            {
                Write(false);
            }
            else
            {
                Write(true);
                InternalWriteString(value);
            }
        }
        else
        {
            InternalWriteString(value);
        }
    }

    /// <summary>
    /// Write a <see cref="long" /> into a span of bytes, as little endian. Flushes the buffer if neccessary.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(long value)
    {
        FlushIfNeeded(8);

        BinaryPrimitives.WriteInt64LittleEndian(_buffer.AsSpan((int)_index), value);
        Index += 8;
    }

    /// <summary>
    /// Write a <see cref="ulong" /> into a span of bytes, as little endian. Flushes the buffer if neccessary.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ulong value)
    {
        FlushIfNeeded(8);

        BinaryPrimitives.WriteUInt64LittleEndian(_buffer.AsSpan((int)_index), value);
        Index += 8;
    }

    /// <summary>
    /// Write a <see cref="int" /> into a span of bytes, as little endian. Flushes the buffer if neccessary.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(int value)
    {
        FlushIfNeeded(4);

        BinaryPrimitives.WriteInt32LittleEndian(_buffer.AsSpan((int)_index), value);
        Index += 4;
    }

    /// <summary>
    /// Write a <see cref="uint" /> into a span of bytes, as little endian. Flushes the buffer if neccessary.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(uint value)
    {
        FlushIfNeeded(4);

        BinaryPrimitives.WriteUInt32LittleEndian(_buffer.AsSpan((int)_index), value);
        Index += 4;
    }

    /// <summary>
    /// Write a <see cref="short" /> into a span of bytes, as little endian. Flushes the buffer if neccessary.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(short value)
    {
        FlushIfNeeded(2);

        BinaryPrimitives.WriteInt16LittleEndian(_buffer.AsSpan((int)_index), value);
        Index += 2;
    }

    /// <summary>
    /// Write a <see cref="ushort" /> into a span of bytes, as little endian. Flushes the buffer if neccessary.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ushort value)
    {
        FlushIfNeeded(2);

        BinaryPrimitives.WriteUInt16LittleEndian(_buffer.AsSpan((int)_index), value);
        Index += 2;
    }

    /// <summary>
    /// Write a <see cref="double" /> into a span of bytes, as little endian. Flushes the buffer if neccessary.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(double value)
    {
        FlushIfNeeded(8);

        BinaryPrimitives.WriteDoubleLittleEndian(_buffer.AsSpan((int)_index), value);
        Index += 8;
    }

    /// <summary>
    /// Write a <see cref="float" /> into a span of bytes, as little endian. Flushes the buffer if neccessary.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(float value)
    {
        FlushIfNeeded(4);

        BinaryPrimitives.WriteSingleLittleEndian(_buffer.AsSpan((int)_index), value);
        Index += 4;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(byte value)
    {
        FlushIfNeeded(1);
        _buffer[Index++] = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(sbyte value)
    {
        FlushIfNeeded(1);
        _buffer[Index++] = (byte)value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Write(bool value)
    {
        FlushIfNeeded(1);
        _buffer[Index++] = *(byte*)&value; // up to 30% faster to dereference the raw value on the stack
    }

    /// <summary>
    /// Convert a <see cref="Serial"/> into <see cref="uint"/> then write into a span of bytes, as little endian. Flushes the buffer if neccessary.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Serial serial) => Write(serial.Value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Type type)
    {
        if (type == null)
        {
            Write((byte)0);
        }
        else
        {
            Write((byte)0x2); // xxHash3 64bit
            Write(AssemblyHandler.GetTypeHash(type));
            _types?.Enqueue(type);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(decimal value)
    {
        Span<int> buffer = stackalloc int[sizeof(decimal) / 4];
        decimal.GetBits(value, buffer);

        Write(MemoryMarshal.Cast<int, byte>(buffer));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InternalWriteString(string value)
    {
        var length = _encoding.GetByteCount(value);

        ((IGenericWriter)this).WriteEncodedInt(length);

        while (_buffer.Length - _index < length)
        {
            Flush();
        }

        // We don't use spans here since that incurs extra allocations for safety.
        Index += _encoding.GetBytes(value, 0, value.Length, _buffer, (int)_index);
    }
}
