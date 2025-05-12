#pragma warning disable CA1720

using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Potisan.Windows.Memory.Heap;

/// <summary>
/// プロセスヒープ。
/// </summary>
/// <remarks>
/// 既定のプロセスヒープ取得には<see cref="DefaultProcessHeap"/>、全プロセスヒープの取得には<see cref="ProcessHeaps"/>、
/// 新しいプロセスヒープの作成には<see cref="Create(nuint, nuint, Potisan.Windows.Memory.Heap.ProcessHeapAllocFlag)"/>を使用してください。
/// </remarks>
[DebuggerDisplay("{Handle}")]
public sealed class ProcessHeap : IDisposable
{
	public SafeProcessHeapHandle Handle { get; }

	/// <summary>
	/// プロセスヒープハンドルをラップします。
	/// </summary>
	/// <param name="handle">プロセスヒープハンドル。</param>
	/// <param name="owns">解放時にプロセスヒープを破壊するか。</param>
	public ProcessHeap(nint handle, bool owns)
	{
		Handle = new(handle, owns);
	}

	/// <summary>
	/// プロセスヒープハンドルをラップします。
	/// </summary>
	/// <param name="handle">プロセスヒープハンドル。</param>
	public ProcessHeap(SafeProcessHeapHandle handle)
	{
		Handle = handle;
	}

	/// <summary>
	/// 既定のプロセスヒープを取得します。
	/// </summary>
	public static ProcessHeap DefaultProcessHeap
	{
		get
		{
			[DllImport("kernel32.dll", SetLastError = true)]
			static extern nint GetProcessHeap();

			var handle = GetProcessHeap();
			if (handle == 0)
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			return new(handle, false);
		}
	}

	/// <summary>
	/// 全てのプロセスヒープを取得します。
	/// </summary>
	public static ImmutableArray<ProcessHeap> ProcessHeaps
	{
		get
		{
			[DllImport("kernel32.dll", SetLastError = true)]
			static extern uint GetProcessHeaps(uint NumberOfHeaps, [Out, MarshalAs(UnmanagedType.LPArray)] nint[]? ProcessHeaps);

			var c = GetProcessHeaps(0, null);
			if (c == 0 && Marshal.GetLastWin32Error() == 0)
				return [];

			var heaps = new nint[c];
			if (GetProcessHeaps(c, heaps) == 0)
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			return [.. heaps.Select(h => new ProcessHeap(h, false))];
		}
	}

	/// <summary>
	/// 新しいプロセスヒープを作成します。
	/// </summary>
	/// <param name="initialSize">初期バイト数。0は1ページ分を意味します。</param>
	/// <param name="maximumSize">最大バイト数。0は可変長を意味します。0以外の場合は固定長となります。</param>
	/// <param name="flags">フラグ。</param>
	/// <returns>新しいプロセスヒープ。</returns>
	public static ProcessHeap Create(nuint initialSize, nuint maximumSize, ProcessHeapAllocFlag flags = ProcessHeapAllocFlag.ZeroMemory)
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern SafeProcessHeapHandle HeapCreate(uint flOptions, nuint dwInitialSize, nuint dwMaximumSize);

		var h = HeapCreate((uint)flags, initialSize, maximumSize);
		if (h.IsInvalid)
			Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
		return new(h);
	}

	/// <inheritdoc/>
	public void Dispose()
	{
		Handle.Dispose();
	}

	/// <summary>
	/// 概要を取得します。
	/// </summary>
	public (nuint AllocatedSize, nuint CommitedSize, nuint ReservedSize, nuint MaxReserveSize) Summary
	{
		get
		{
			[DllImport("kernel32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			static extern bool HeapSummary(SafeProcessHeapHandle hHeap, uint dwFlags, ref HEAP_SUMMARY lpSummary);

			var hs = new HEAP_SUMMARY() { cb = (uint)Marshal.SizeOf<HEAP_SUMMARY>() };
			if (!HeapSummary(Handle, 0, ref hs))
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			return (hs.cbAllocated, hs.cbCommitted, hs.cbReserved, hs.cbMaxReserve);
		}
	}

	private struct HEAP_SUMMARY
	{
		public uint cb;
		public nuint cbAllocated;
		public nuint cbCommitted;
		public nuint cbReserved;
		public nuint cbMaxReserve;
	}

	/// <summary>
	/// プロセスヒープエントリを列挙します。
	/// </summary>
	public IEnumerable<ProcessHeapEntry> EnumerateEntries()
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool HeapWalk(SafeProcessHeapHandle hHeap, [In, Out] ProcessHeapEntry lpEntry);

		const int ERROR_NO_MORE_ITEMS = 259;

		var handle = Handle;
		var entry = new ProcessHeapEntry();
		for (; ; )
		{
			if (!HeapWalk(handle, entry))
			{
				if (Marshal.GetLastPInvokeError() is ERROR_NO_MORE_ITEMS or 0)
					break;
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}
			yield return entry;
		}
	}

	/// <summary>
	/// プロセスヒープエントリ配列を取得します。
	/// </summary>
	public ProcessHeapEntry[] GetEntries() => [.. EnumerateEntries()];

	/// <summary>
	/// プロセスヒープ上のメモリを確保します。通常は<see cref="Alloc(nuint, ProcessHeapAllocFlag)"/>を使用してください。
	/// </summary>
	/// <param name="size">確保するバイト数。</param>
	/// <param name="flags">フラグ。</param>
	/// <returns>確保されたメモリのポインタ。</returns>
	public nint DangerousAlloc(nuint size, ProcessHeapAllocFlag flags = ProcessHeapAllocFlag.ZeroMemory)
	{
		[DllImport("kernel32.dll")]
		static extern nint HeapAlloc(SafeProcessHeapHandle hHeap, uint dwFlags, nuint dwBytes);

		var p = HeapAlloc(Handle, (uint)flags, size);
		if (p == 0 && Marshal.GetLastPInvokeError() != 0)
			Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
		return p;
	}

	/// <summary>
	/// プロセスヒープ上のメモリを解放します。
	/// 通常は<see cref="Alloc(nuint, ProcessHeapAllocFlag)"/>で確保して解放を自動化してください。
	/// </summary>
	/// <param name="pointer">解放するメモリのポインタ。</param>
	/// <param name="noSerialize">シリアライズしない場合は真。既定のプロセスヒープでは無効です。</param>
	/// <returns>解放の成否。</returns>
	public bool DangerousFree(nint pointer, bool noSerialize = false)
	{
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool HeapFree(SafeProcessHeapHandle hHeap, uint dwFlags, nint lpMem);

		return HeapFree(Handle, noSerialize ? (uint)ProcessHeapAllocFlag.NoSerialize : 0, pointer);
	}

	/// <summary>
	/// プロセスヒープ上にメモリを確保します。
	/// </summary>
	/// <param name="size">確保するバイト数。</param>
	/// <param name="flags">フラグ。</param>
	/// <returns>確保されたメモリのポインタセーフハンドル。</returns>
	public SafeProcessHeapPointer Alloc(nuint size, ProcessHeapAllocFlag flags = ProcessHeapAllocFlag.ZeroMemory)
	{
		nint p = 0;
		try
		{
			p = DangerousAlloc(size, flags);
			return new(this, p);
		}
		catch
		{
			DangerousFree(p);
			throw;
		}
	}

	/// <summary>
	/// ヒープロックの所有。通常は<see cref="ScopedLock"/>を使用してください。
	/// </summary>
	/// <returns>ロック所有の成否。</returns>
	public bool Lock()
	{
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool HeapLock(SafeProcessHeapHandle hHeap);

		return HeapLock(Handle);
	}

	/// <summary>
	/// ヒープロックの解放。通常は<see cref="ScopedLock"/>を使用してください。
	/// </summary>
	/// <returns>ロック所有解放の成否。</returns>
	public bool Unlock()
	{
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool HeapUnlock(SafeProcessHeapHandle hHeap);

		return HeapUnlock(Handle);
	}

	/// <summary>
	/// ヒープロックの所有権を所持するスコープを作成します。usingと組み合わせて使用します。
	/// </summary>
	public LockScope ScopedLock()
		=> new(this);

	/// <summary>
	/// ヒープロックの所有権を所持するスコープ。
	/// usingと組み合わせて使用するか適切に解放してください。
	/// </summary>
	public readonly struct LockScope : IDisposable
	{
		// この構造体は内部で状態を変更しますが、防衛的コピーが不要なのでreadonlyを付加します。

		private readonly ProcessHeap _heap;

		internal LockScope(ProcessHeap heap)
		{
			heap.Lock();
			_heap = heap;
		}

		public readonly void Dispose()
		{
			_heap.Unlock();
		}
	}

	/// <summary>
	/// ヒープ内の隣接空きメモリブロックを結合します。
	/// </summary>
	/// <param name="noSerialize">シリアライズしない場合は真。既定のプロセスヒープでは無効です。</param>
	/// <returns>コミットされた最大空きブロックサイズ。</returns>
	public nint Compact(bool noSerialize = false)
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern nint HeapCompact(SafeProcessHeapHandle hHeap, uint dwFlags);

		var size = HeapCompact(Handle, noSerialize ? (uint)ProcessHeapAllocFlag.NoSerialize : 0);
		if (size == 0 && Marshal.GetLastPInvokeError() != 0)
			Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
		return size;
	}

	/// <summary>
	/// プロセスヒープ上のメモリを再確保します。通常は<see cref="SafeProcessHeapPointer.Realloc(nuint, ProcessHeapAllocFlag)"/>を使用してください。
	/// </summary>
	/// <param name="pointer">再割り当てするメモリのポインタ。</param>
	/// <param name="size">確保するバイト数。非拡張ヒープの場合は<c>0x7FFF8</c>以下です。</param>
	/// <param name="flags">フラグ。</param>
	/// <returns>再確保されたメモリのポインタ。</returns>
	public nint DangerousRealloc(nint pointer, nuint size, ProcessHeapAllocFlag flags = 0)
	{
		[DllImport("kernel32.dll")]
		static extern nint HeapRealloc(SafeProcessHeapHandle hHeap, uint dwFlags, nint lpMem, nuint dwBytes);

		var p = HeapRealloc(Handle, (uint)flags, pointer, size);
		if (p == 0 && Marshal.GetLastPInvokeError() != 0)
			Marshal.ThrowExceptionForHR(Marshal.GetLastPInvokeError());
		return p;
	}

	/// <summary>
	/// プロセスヒープ上のメモリのバイト数を取得します。通常は<see cref="SafeProcessHeapPointer.Size"/>を使用してください。
	/// </summary>
	/// <param name="pointer">サイズ取得するメモリのポインタ。</param>
	/// <param name="noSerialize">シリアライズしない場合は真。既定のプロセスヒープでは無効です。</param>
	/// <returns>メモリのバイト数。</returns>
	public nint DangerousSizeOf(nint pointer, bool noSerialize = false)
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern nint HeapSize(SafeProcessHeapHandle hHeap, uint dwFlags, nint lpMem);

		var size = HeapSize(Handle, noSerialize ? (uint)ProcessHeapAllocFlag.NoSerialize : 0, pointer);
		if (size == 0 && Marshal.GetLastPInvokeError() == 0)
			Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
		return size;
	}

	/// <summary>
	/// プロセスヒープまたはメモリの一貫性を確認します。
	/// 通常はヒープ単位なら<see cref="Validate(bool)"/>、メモリ単位なら<see cref="SafeProcessHeapPointer.Size"/>を使用してください。
	/// </summary>
	/// <param name="pointer">一貫性を確認するメモリのポインタ。0の場合はヒープ単位です。</param>
	/// <param name="noSerialize">シリアライズしない場合は真。既定のプロセスヒープでは無効です。</param>
	/// <returns>ヒープまたはメモリブロックの一貫性。</returns>
	public bool DangerousValidate(nint pointer, bool noSerialize = false)
	{
		[DllImport("kernel32.dll")]
		static extern bool HeapValidate(SafeProcessHeapHandle hHeap, uint dwFlags, nint lpMem);

		var f = HeapValidate(Handle, noSerialize ? (uint)ProcessHeapAllocFlag.NoSerialize : 0, pointer);
		if (!f && Marshal.GetLastPInvokeError() != 0)
			Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
		return f;
	}

	/// <summary>
	/// プロセスヒープの一貫性を確認します。
	/// </summary>
	/// <param name="noSerialize">シリアライズしない場合は真。既定のプロセスヒープでは無効です。</param>
	/// <returns>ヒープの一貫性。</returns>
	public bool Validate(bool noSerialize = false) => DangerousValidate(0, noSerialize);

	/// <summary>
	/// プロセスヒープ情報を取得します。通常は他のメソッドで取得します。
	/// </summary>
	/// <param name="informationClass">情報クラス。</param>
	/// <param name="buffer">取得した情報のバッファー。</param>
	/// <returns>取得した情報の実際のサイズのバッファー。</returns>
	public Span<byte> QueryInformation(int informationClass, Span<byte> buffer)
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool HeapQueryInformation(SafeProcessHeapHandle HeapHandle, int HeapInformationClass,
			ref byte HeapInformation, nuint HeapInformationLength, out nuint ReturnLength);

		var f = HeapQueryInformation(Handle, informationClass, ref MemoryMarshal.GetReference(buffer),
			(nuint)buffer.Length, out var len);
		if (!f && Marshal.GetLastPInvokeError() != 0)
			Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
		// lenは与えた長さを超えないはずなので、長さチェックはcheckedに任せる。
		return buffer[..checked((int)len)];
	}

	/// <summary>
	/// プロセスヒープ情報を取得します。通常は他のメソッドで取得します。
	/// </summary>
	/// <typeparam name="T">取得する情報の型（アンマネージド型）。</typeparam>
	/// <param name="informationClass">情報クラス。</param>
	/// <returns>取得した情報。</returns>
	/// <exception cref="InvalidDataException"></exception>
	public T QueryInformation<T>(int informationClass)
		where T : unmanaged
	{
		var t = new T();
		var size = Marshal.SizeOf<T>();
		if (QueryInformation(informationClass, MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref t, 1))).Length != size)
			throw new InvalidDataException("ジェネリック引数Tと戻り値のサイズが一致しません。informationClassとTの対応を間違えている可能性があります。");
		return t;
	}

	private void SetInformation(int informationClass, in byte buffer, int bufferLength)
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		static extern bool HeapSetInformation(SafeProcessHeapHandle HeapHandle, int HeapInformationClass,
			in byte HeapInformation, nuint HeapInformationLength);

		var f = HeapSetInformation(Handle, informationClass, buffer, (nuint)bufferLength);
		if (!f && Marshal.GetLastPInvokeError() != 0)
			Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
	}

	/// <summary>
	/// プロセスヒープ情報を設定します。通常は他のメソッドで設定します。
	/// </summary>
	/// <param name="informationClass">情報クラス。</param>
	/// <param name="buffer">設定する情報のバッファー。</param>
	public void SetInformation(int informationClass, ReadOnlySpan<byte> buffer)
	{
		SetInformation(informationClass, in MemoryMarshal.GetReference(buffer), buffer.Length);
	}

	/// <summary>
	/// プロセスヒープ情報を設定します。通常は他のメソッドで設定します。
	/// </summary>
	/// <typeparam name="T">取得する情報の型（アンマネージド型）。</typeparam>
	/// <param name="informationClass">情報クラス</param>
	/// <param name="value">設定する情報の値。</param>
	public void SetInformation<T>(int informationClass, in T value)
		where T : unmanaged
	{
		SetInformation(informationClass, MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(in value, 1)));
	}

	/// <summary>
	/// 互換性情報を取得します。
	/// </summary>
	public ProcessHeapCompatibilityInfo CompatibilityInfo
	{
		get => (ProcessHeapCompatibilityInfo)QueryInformation<uint>(0);
		set => SetInformation(0, (uint)value);
	}

	/// <summary>
	/// 破損時の終了機能を有効化します。この設定は有効可のみ可能です。
	/// </summary>
	public void EnableTeriminationOnCorruption()
	{
		SetInformation(1, in Unsafe.NullRef<byte>());
	}

	/// <summary>
	/// キャッシュを最適化して不要なメモリのコミットを解除します。
	/// </summary>
	public void OptimizeResources()
	{
		SetInformation(1, new HEAP_OPTIMIZE_RESOURCES_INFORMATION(1, 0));
	}

	private readonly struct HEAP_OPTIMIZE_RESOURCES_INFORMATION(uint version, uint flags)
	{
		public readonly uint Version = version;
		public readonly uint Flags = flags;
	}
}

/// <summary>
/// プロセスヒープエントリの情報。
/// </summary>
/// <remarks>Win APIの<c>PROCESS_HEAP_ENTRY</c>構造体に対応します。</remarks>
[StructLayout(LayoutKind.Sequential)]
public sealed class ProcessHeapEntry
{
	/// <summary>
	/// データのポインタ。
	/// </summary>
	public nint DataPointer { get; }
	/// <summary>
	/// データのバイト数。Regionの場合はアドレス空間の予約サイズ、Rangeの場合はコミット前のメモリ範囲サイズです。
	/// </summary>
	public uint DataSize { get; }
	/// <summary>
	/// エントリ情報保持用のシステムデータバイト数。
	/// </summary>
	public byte OverheadSize { get; }
	/// <summary>
	/// エントリを含むヒープ領域のリージョンインデックス。
	/// </summary>
	public byte RegionIndex { get; }
	/// <summary>
	/// エントリのプロパティ。
	/// </summary>
	public ProcessHeapEntryFlag Flags { get; }
	private readonly DummyUnion u;

	/// <summary>
	/// メモリブロック情報。
	/// </summary>
	public ref readonly BlockInfo Block
	{
		get
		{
			if (!HasBlock)
				throw new InvalidDataException();
			return ref u.Block;
		}
	}

	/// <summary>
	/// メモリ領域情報。
	/// </summary>
	public ref readonly RegionInfo Region
	{
		get
		{
			if (!HasRegion)
				throw new InvalidDataException();
			return ref u.Region;
		}
	}

	/// <summary>
	/// メモリブロック情報の保持有無を取得します。
	/// </summary>
	public bool HasBlock => (Flags & (ProcessHeapEntryFlag.Busy | ProcessHeapEntryFlag.Moveable)) != 0;

	/// <summary>
	/// メモリ領域情報の保持有無を取得します。
	/// </summary>
	public bool HasRegion => (Flags & ProcessHeapEntryFlag.Region) != 0;

	/// <summary>
	/// メモリブロック情報。
	/// </summary>
	public readonly struct BlockInfo
	{
		/// <summary>
		/// 移動可能なメモリブロックのハンドル。
		/// </summary>
		public readonly nint MemoryHandle { get; }
		/// <summary>
		/// 未使用。
		/// </summary>
		public readonly uint Reserved1 { get; }
		/// <summary>
		/// 未使用。
		/// </summary>
		public readonly uint Reserved2 { get; }
		/// <summary>
		/// 未使用。
		/// </summary>
		public readonly uint Reserved3 { get; }
	}

	/// <summary>
	/// メモリ領域情報。
	/// </summary>
	public readonly struct RegionInfo
	{
		/// <summary>
		/// 空きメモリブロック、ビジーメモリブロック、ヒープ制御構造体としてコミットされているヒープ領域のバイト数。
		/// バイト数が使用できない場合は0です。
		/// </summary>
		public readonly uint CommittedSize { get; }
		/// <summary>
		/// コミットされていないヒープ領域のバイト数。
		/// バイト数が使用できない場合は0です。
		/// </summary>
		public readonly uint UnCommittedSize { get; }
		/// <summary>
		/// 最初の有効メモリブロックへのポインター。
		/// </summary>
		public readonly nint FirstBlockPointer { get; }
		/// <summary>
		/// 最初の無効メモリブロックへのポインター。
		/// </summary>
		public readonly nint LastBlockPointer { get; }
	}

	[StructLayout(LayoutKind.Explicit)]
	private readonly struct DummyUnion
	{
		[FieldOffset(0)]
		public readonly BlockInfo Block;
		[FieldOffset(0)]
		public readonly RegionInfo Region;
	}
}

/// <summary>
/// 安全なプロセスヒープハンドル。
/// </summary>
public sealed class SafeProcessHeapHandle : SafeHandle, IFormattable
{
	/// <summary>
	/// 所有権を持つ状態で空のセーフハンドルを作成します。
	/// </summary>
	public SafeProcessHeapHandle() : base(0, true) { }

	/// <summary>
	/// ハンドルと所有権を指定してセーフハンドルを作成します。
	/// </summary>
	public SafeProcessHeapHandle(nint handle, bool owns) : base(handle, owns) { }

	/// <inheritdoc/>
	public override bool IsInvalid => handle == 0;

	/// <inheritdoc/>
	protected override bool ReleaseHandle()
	{
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool HeapDestroy(nint hHeap);

		return HeapDestroy(handle);
	}

#pragma warning disable CA1305
	public override string ToString() => handle.ToString();
#pragma warning restore CA1305
	public string ToString(string? format, IFormatProvider? formatProvider) => handle.ToString(format, formatProvider);
}

#pragma warning disable CA1711
/// <summary>
/// プロセスヒープエントリのフラグ。
/// </summary>
[Flags]
public enum ProcessHeapEntryFlag : ushort
{
	/// <summary>
	/// ヒープエントリはヒープ上の連続仮想メモリ領域の先頭。
	/// </summary>
	Region = 0x0001,
	/// <summary>
	/// ヒープエントリは未コミット領域内。
	/// </summary>
	UncommitedRange = 0x0002,
	/// <summary>
	/// ヒープエントリは割り当て済みブロック。
	/// </summary>
	Busy = 0x0004,
	/// <summary>
	/// ヒープ領域は移動可能な割り当て済みブロック。
	/// </summary>
	Moveable = 0x0010,
	/// <summary>
	/// ヒープ領域はDDE共有割り当て済みブロック。
	/// </summary>
	DdeShare = 0x0020,
}

/// <summary>
/// プロセスヒープの割り当てフラグ。
/// </summary>
[Flags]
public enum ProcessHeapAllocFlag : uint
{
	/// <summary>
	/// シリアル化アクセスを行いません。
	/// 既定のプロセスヒープでは使用しないでください。
	/// </summary>
	NoSerialize = 0x00000001,
	/// <summary>
	/// 無効値を返す代わりに例外を発生させます。
	/// <see cref="ProcessHeap.Create(nuint, nuint, ProcessHeapAllocFlag)"/>等で指定すると以降の呼び出しでも適用されます。
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)] GenerateExceptions = 0x00000004,
	/// <summary>
	/// メモリを0初期化します。再割り当て時は追加部分のみに適用されます。
	/// </summary>
	ZeroMemory = 0x00000008,
	/// <summary>
	/// メモリブロックを移動しません。
	/// <see cref="ProcessHeap.Create(nuint, nuint, ProcessHeapAllocFlag)"/>等では使用せず、
	/// <see cref="SafeProcessHeapPointer.Realloc(nuint, ProcessHeapAllocFlag)"/>等で使用します。
	/// </summary>
	ReallocInPlaceOnly = 0x00000010,
}
#pragma warning restore CA1711

/// <summary>
/// プロセスヒープ内のメモリポインタ。
/// </summary>
public sealed class SafeProcessHeapPointer : SafeHandle, IFormattable
{
	private readonly ProcessHeap _heap;

	internal SafeProcessHeapPointer(ProcessHeap heap, nint handle)
		: base(handle, true)
	{
		_heap = heap;
	}

	public override bool IsInvalid => handle == 0;

	protected override bool ReleaseHandle()
		=> _heap.DangerousFree(handle);

	public SafeProcessHeapPointer Realloc(nuint size, ProcessHeapAllocFlag flags = 0)
	{
		handle = _heap.DangerousRealloc(handle, size, flags);
		return this;
	}

	public nint Size => _heap.DangerousSizeOf(handle);
	public nint SizeWithoutSerialize => _heap.DangerousSizeOf(handle, true);

	public bool IsValidated => _heap.DangerousValidate(handle);
	public bool IsValidatedWithoutSerialize => _heap.DangerousValidate(handle, true);

#pragma warning disable CA1305
	public override string ToString() => handle.ToString();
#pragma warning restore CA1305
	public string ToString(string? format, IFormatProvider? formatProvider) => handle.ToString(format, formatProvider);
}

/// <summary>
/// プロセスヒープの互換性情報。
/// </summary>
public enum ProcessHeapCompatibilityInfo
{
	StandardHeap = 0,
	LookAsideLists = 1,
	LowFragmentationHeap = 2,
}
