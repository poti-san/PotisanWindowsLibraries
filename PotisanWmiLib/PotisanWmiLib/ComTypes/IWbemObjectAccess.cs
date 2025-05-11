namespace Potisan.Windows.Wmi.ComTypes;

// TODO
[ComImport]
[Guid("49353c9a-516b-11d1-aea6-00c04fb68820")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IWbemObjectAccess // IWbemClassObject
{
	#region IWbemClassObject

	[PreserveSig]
	int GetQualifierSet(
		out IWbemQualifierSet ppQualSet);

	[PreserveSig]
	int Get(
		string wszName,
		int lFlags,
		[MarshalAs(UnmanagedType.Struct)] out object pVal,
		out CimType pType,
		out int plFlavor);

	[PreserveSig]
	int Put(
		string wszName,
		int lFlags,
		[MarshalAs(UnmanagedType.Struct)] in object pVal,
		CimType Type);

	[PreserveSig]
	int Delete(
		string wszName);

	// TODO: CHECK
	[PreserveSig]
	int GetNames(
		string wszQualifierName,
		int lFlags,
		[MarshalAs(UnmanagedType.Struct)] in object pQualifierVal,
		[MarshalAs(UnmanagedType.SafeArray)] out object[] pNames);

	[PreserveSig]
	int BeginEnumeration(
		int lEnumFlags);

	[PreserveSig]
	int Next(
		int lFlags,
		[MarshalAs(UnmanagedType.BStr)] out string? strName,
		[MarshalAs(UnmanagedType.Struct)] out object pVal,
		out CimType pType,
		out int plFlavor);

	[PreserveSig]
	int EndEnumeration();

	[PreserveSig]
	int GetPropertyQualifierSet(
		string wszProperty,
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
		string wszName,
		[MarshalAs(UnmanagedType.BStr)] out string? pstrClassName);

	[PreserveSig]
	int InheritsFrom(
		[MarshalAs(UnmanagedType.LPWStr)] string? strAncestor);

	[PreserveSig]
	int GetMethod(
		string wszName,
		int lFlags,
		out IWbemClassObject? ppInSignature,
		out IWbemClassObject? ppOutSignature);

	[PreserveSig]
	int PutMethod(
		string wszName,
		int lFlags,
		IWbemClassObject pInSignature,
		IWbemClassObject pOutSignature);

	[PreserveSig]
	int DeleteMethod(
		string wszName);

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
		string wszMethod,
		out IWbemQualifierSet ppQualSet);

	[PreserveSig]
	int GetMethodOrigin(
		string wszMethodName,
		[MarshalAs(UnmanagedType.BStr)] out string? pstrClassName);

	#endregion IWbemClassObject

	[PreserveSig]
	int GetPropertyHandle(
			string wszPropertyName,
			out CimType pType,
			out int plHandle);

	[PreserveSig]
	int WritePropertyValue(
		int lHandle,
		int lNumBytes,
		in byte aData);

	[PreserveSig]
	int ReadPropertyValue(
		int lHandle,
		int lBufferSize,
		out int plNumBytes,
		ref byte aData);

	[PreserveSig]
	int ReadDWORD(
		int lHandle,
		out uint pdw);

	[PreserveSig]
	int WriteDWORD(
		int lHandle,
		uint dw);

	[PreserveSig]
	int ReadQWORD(
		int lHandle,
		out ulong pqw);

	[PreserveSig]
	int WriteQWORD(
		int lHandle,
		ulong pw);

	[PreserveSig]
	int GetPropertyInfoByHandle(
		int lHandle,
		[MarshalAs(UnmanagedType.BStr)] out string? pstrName,
		out CimType pType);

	[PreserveSig]
	int Lock(
		int lFlags);

	[PreserveSig]
	int Unlock(
		int lFlags);
}
