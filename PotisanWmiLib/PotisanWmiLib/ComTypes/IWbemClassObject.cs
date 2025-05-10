namespace Potisan.Windows.Wmi.ComTypes;

[ComImport]
[Guid("dc12a681-737f-11cf-884d-00aa004b2e24")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IWbemClassObject
{
	[PreserveSig]
	int GetQualifierSet(
		out IWbemQualifierSet ppQualSet);

	[PreserveSig]
	int Get(
		[MarshalAs(UnmanagedType.LPWStr)] string wszName,
		int lFlags,
		[MarshalAs(UnmanagedType.Struct)] out object pVal,
		out CimType pType,
		out int plFlavor);

	[PreserveSig]
	int Put(
		[MarshalAs(UnmanagedType.LPWStr)] string wszName,
		int lFlags,
		[MarshalAs(UnmanagedType.Struct)] in object pVal,
		CimType Type);

	[PreserveSig]
	int Delete(
		[MarshalAs(UnmanagedType.LPWStr)] string wszName);

	// TODO: CHECK
	[PreserveSig]
	int GetNames(
		[MarshalAs(UnmanagedType.LPWStr)] string? wszQualifierName,
		int lFlags,
		[MarshalAs(UnmanagedType.Struct)] in object? pQualifierVal,
		[MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_BSTR)] out string[] pNames);

	[PreserveSig]
	int BeginEnumeration(
		int lEnumFlags);

	[PreserveSig]
	int Next(
		int lFlags,
		[MarshalAs(UnmanagedType.BStr)] out string? strName,
		[MarshalAs(UnmanagedType.Struct)] out object? pVal,
		out CimType pType,
		out int plFlavor);

	[PreserveSig]
	int EndEnumeration();

	[PreserveSig]
	int GetPropertyQualifierSet(
		[MarshalAs(UnmanagedType.LPWStr)] string wszProperty,
		out IWbemQualifierSet ppQualSet);

	[PreserveSig]
	int Clone(
		out IWbemClassObject? ppCopy);

	[PreserveSig]
	int GetObjectText(
		int lFlags,
		[MarshalAs(UnmanagedType.BStr)] out string? pstrObjectText);

	[PreserveSig]
	int SpawnDerivedClass(
		int lFlags,
		out IWbemClassObject? ppNewClass);

	[PreserveSig]
	int SpawnInstance(
		int lFlags,
		out IWbemClassObject? ppNewInstance);

	[PreserveSig]
	int CompareTo(
		int lFlags,
		IWbemClassObject pCompareTo);

	[PreserveSig]
	int GetPropertyOrigin(
		[MarshalAs(UnmanagedType.LPWStr)] string wszName,
		[MarshalAs(UnmanagedType.BStr)] out string? pstrClassName);

	[PreserveSig]
	int InheritsFrom(
		[MarshalAs(UnmanagedType.LPWStr)] string strAncestor);

	[PreserveSig]
	int GetMethod(
		[MarshalAs(UnmanagedType.LPWStr)] string wszName,
		int lFlags,
		out IWbemClassObject? ppInSignature,
		out IWbemClassObject? ppOutSignature);

	[PreserveSig]
	int PutMethod(
		[MarshalAs(UnmanagedType.LPWStr)] string wszName,
		int lFlags,
		IWbemClassObject? pInSignature,
		IWbemClassObject? pOutSignature);

	[PreserveSig]
	int DeleteMethod(
		[MarshalAs(UnmanagedType.LPWStr)] string wszName);

	[PreserveSig]
	int BeginMethodEnumeration(
		int lEnumFlags);

	[PreserveSig]
	int NextMethod(
		int lFlags,
		[MarshalAs(UnmanagedType.BStr)] out string pstrName,
		out IWbemClassObject? ppInSignature,
		out IWbemClassObject? ppOutSignature);

	[PreserveSig]
	int EndMethodEnumeration();

	[PreserveSig]
	int GetMethodQualifierSet(
		[MarshalAs(UnmanagedType.LPWStr)] string wszMethod,
		out IWbemQualifierSet ppQualSet);

	[PreserveSig]
	int GetMethodOrigin(
		[MarshalAs(UnmanagedType.LPWStr)] string wszMethodName,
		[MarshalAs(UnmanagedType.BStr)] out string? pstrClassName);
}
