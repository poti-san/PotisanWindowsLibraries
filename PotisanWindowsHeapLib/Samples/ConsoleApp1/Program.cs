using Potisan.Windows.Memory.Heap;

// 全プロセスヒープ概要の表示
Console.WriteLine("#, Handle, AllocatedSize, CommitedSize, ReservedSize, MaxReserveSize");
foreach (var (i, heap) in ProcessHeap.ProcessHeaps.Index())
{
	var (AllocatedSize, CommitedSize, ReservedSize, MaxReserveSize) = heap.Summary;
	Console.WriteLine($"{i}, {heap.Handle}, {AllocatedSize}, {CommitedSize}, {ReservedSize}, {MaxReserveSize}");
}
Console.WriteLine();

// メインプロセスヒープのハンドル取得
Console.WriteLine($"メインプロセスヒープのハンドル: 0x{ProcessHeap.DefaultProcessHeap.Handle:X}");
