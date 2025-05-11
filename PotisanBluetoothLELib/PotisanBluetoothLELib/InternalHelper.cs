using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

namespace Potisan.Windows.BluetoothLE;

internal static class InternalHelper
{
	public static string GetInterfacePath(SafeDeviceInfoHandle handle, in SP_DEVICE_INTERFACE_DATA interfaceInfo)
	{
		NativeMethods.SetupDiGetDeviceInterfaceDetailW(handle, interfaceInfo, ref Unsafe.NullRef<byte>(), 0, out var requiredSize, 0);
		var buffer = ArrayPool<byte>.Shared.Rent(checked((int)requiredSize));
		try
		{
			BinaryPrimitives.WriteUInt32LittleEndian(buffer, (uint)Marshal.SizeOf<SP_DEVICE_INTERFACE_DETAIL_DATA_W_1>());
			if (!NativeMethods.SetupDiGetDeviceInterfaceDetailW(handle, interfaceInfo, ref MemoryMarshal.GetArrayDataReference(buffer),
				requiredSize, out Unsafe.NullRef<uint>(), 0))
			{
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			}

			var pathOffset = (uint)Marshal.OffsetOf<SP_DEVICE_INTERFACE_DETAIL_DATA_W_1>(nameof(SP_DEVICE_INTERFACE_DETAIL_DATA_W_1.DevicePath0));
			return Encoding.Unicode.GetString(buffer.AsSpan()[(int)pathOffset..(int)(requiredSize - pathOffset + 2)]);
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(buffer);
		}
	}
}

file static class NativeMethods
{
	[DllImport("setupapi.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetupDiGetDeviceInterfaceDetailW(
		SafeDeviceInfoHandle DeviceInfoSet,
		in SP_DEVICE_INTERFACE_DATA DeviceInterfaceData,
		ref byte DeviceInterfaceDetailData,
		uint DeviceInterfaceDetailDataSize,
		out uint RequiredSize,
		nint DeviceInfoData);
}
