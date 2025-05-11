namespace Potisan.Windows.Wmi.ComTypes;

[ComImport]
[Guid("027947e1-d731-11ce-a357-000000000001")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IEnumWbemClassObject
{
	[PreserveSig]
	int Reset();

	[PreserveSig]
	int Next(
		uint lTimeout,
		uint uCount,
		out IWbemClassObject? apObjects,
		out uint puReturned);

	[PreserveSig]
	int NextAsync(
		uint uCount,
		IWbemObjectSink? pSink);

	[PreserveSig]
	int Clone(
		out IEnumWbemClassObject? ppEnum);

	[PreserveSig]
	int Skip(
		uint lTimeout,
		uint nCount);
}
