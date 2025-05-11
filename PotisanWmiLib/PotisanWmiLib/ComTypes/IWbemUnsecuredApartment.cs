namespace Potisan.Windows.Wmi.ComTypes;

[ComImport]
[Guid("31739d04-3471-4cf4-9a7c-57a44ae71956")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IWbemUnsecuredApartment //  IUnsecuredApartment
{
	#region IUnsecuredApartment

	[PreserveSig]
	int CreateObjectStub(
		[MarshalAs(UnmanagedType.IUnknown)] object? pObject,
		[MarshalAs(UnmanagedType.IUnknown)] out object? ppStub);

	#endregion IUnsecuredApartment

	[PreserveSig]
	int CreateSinkStub(
		IWbemObjectSink? pSink,
		uint dwFlags,
		[MarshalAs(UnmanagedType.LPWStr)] string? wszReserved,
		out IWbemObjectSink? ppStub);
}
