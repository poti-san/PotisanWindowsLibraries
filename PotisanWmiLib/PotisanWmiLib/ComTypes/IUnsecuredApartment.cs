namespace Potisan.Windows.Wmi.ComTypes;

[ComImport]
[Guid("1cfaba8c-1523-11d1-ad79-00c04fd8fdff")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IUnsecuredApartment
{
	[PreserveSig]
	int CreateObjectStub(
		[MarshalAs(UnmanagedType.IUnknown)] object? pObject,
		[MarshalAs(UnmanagedType.IUnknown)] out object? ppStub);
}
