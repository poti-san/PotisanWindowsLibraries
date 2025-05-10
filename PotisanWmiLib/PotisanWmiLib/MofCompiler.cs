using Potisan.Windows.Wmi.ComTypes;

namespace Potisan.Windows.Wmi;

/// <summary>
/// マネージドオブジェクト形式(MOF)コンパイラ。
/// </summary>
public sealed class MofCompiler : IDisposable
{
	private IMofCompiler? _obj;

	internal MofCompiler(object o)
	{
		_obj = (IMofCompiler)o;
	}

	public MofCompiler()
	{
		Guid CLSID_MofCompiler = new("6daf9757-2e37-11d2-aec9-00c04fb68820");
		_obj = (IMofCompiler)Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_MofCompiler)!)!;
	}

	[NotNullIfNotNull(nameof(_obj))]
	public object? WrappedObject => _obj;

	[MemberNotNullWhen(false, nameof(_obj))]
	public bool IsDisposed => _obj == null;

	public void Dispose()
	{
		if (_obj == null) return;
		Marshal.FinalReleaseComObject(_obj);
		_obj = null;
		GC.SuppressFinalize(this);
	}

	[MemberNotNull(nameof(_obj))]
	public void ThrowIfDisposed()
	{
		ObjectDisposedException.ThrowIf(_obj == null, typeof(WbemLocator));
	}

	public WbemCompileStatusInfo CompileFile(
		string fileName,
		string? serverAndNamespace,
		string? user,
		string? authority,
		string? password,
		bool checkOnly = false,
		bool autoRecover = false,
		bool consolePrint = false,
		bool doesntAddToList = false,
		CreateOrUpdate classCreateOrUpdate = CreateOrUpdate.Both,
		UpdateMode classUpdateMode = UpdateMode.Compatible,
		CreateOrUpdate instanceCreateOrUpdate = CreateOrUpdate.Both
		)
	{
		ThrowIfDisposed();
		var optionFlags = (int)((checkOnly ? WbemCompilerOption.WBEM_FLAG_CHECK_ONLY : 0)
			| (autoRecover ? WbemCompilerOption.WBEM_FLAG_AUTORECOVER : 0)
			| (consolePrint ? WbemCompilerOption.WBEM_FLAG_CONSOLE_PRINT : 0)
			| (doesntAddToList ? WbemCompilerOption.WBEM_FLAG_DONT_ADD_TO_LIST : 0));
		var classFlags = (int)classCreateOrUpdate | (int)classUpdateMode;
		var instanceFlags = (int)instanceCreateOrUpdate;
		var info = new WbemCompileStatusInfo();
		Marshal.ThrowExceptionForHR(_obj.CompileFile(fileName, serverAndNamespace, user, authority, password, optionFlags, classFlags, instanceFlags, info));
		return info;
	}

	public WbemCompileStatusInfo CompileBuffer(
		ReadOnlySpan<byte> buffer,
		string? serverAndNamespace,
		string? user,
		string? authority,
		string? password,
		bool checkOnly = false,
		bool autoRecover = false,
		bool consolePrint = false,
		bool doesntAddToList = false
		)
	{
		ThrowIfDisposed();
		var optionFlags = (int)((checkOnly ? WbemCompilerOption.WBEM_FLAG_CHECK_ONLY : 0)
			| (autoRecover ? WbemCompilerOption.WBEM_FLAG_AUTORECOVER : 0)
			| (consolePrint ? WbemCompilerOption.WBEM_FLAG_CONSOLE_PRINT : 0)
			| (doesntAddToList ? WbemCompilerOption.WBEM_FLAG_DONT_ADD_TO_LIST : 0));
		var info = new WbemCompileStatusInfo();
		Marshal.ThrowExceptionForHR(_obj.CompileBuffer(buffer.Length, MemoryMarshal.GetReference(buffer),
			serverAndNamespace, user, authority, password, optionFlags, 0, 0, info));
		return info;
	}

	public WbemCompileStatusInfo CreateBmof(
		string textFileName,
		string bmofFileName,
		string? serverAndNamespace,
		bool checkOnly = false,
		bool consolePrint = false,
		bool wmiCheck = false,
		CreateOrUpdate classCreateOrUpdate = CreateOrUpdate.Both,
		UpdateMode classUpdateMode = UpdateMode.Compatible,
		CreateOrUpdate instanceCreateOrUpdate = CreateOrUpdate.Both
		)
	{
		ThrowIfDisposed();
		var optionFlags = (int)((checkOnly ? WbemCompilerOption.WBEM_FLAG_CHECK_ONLY : 0)
			| (consolePrint ? WbemCompilerOption.WBEM_FLAG_CONSOLE_PRINT : 0)
			| (wmiCheck ? WbemCompilerOption.WBEM_FLAG_WMI_CHECK : 0));
		var classFlags = (int)classCreateOrUpdate | (int)classUpdateMode;
		var instanceFlags = (int)instanceCreateOrUpdate;
		var info = new WbemCompileStatusInfo();
		Marshal.ThrowExceptionForHR(_obj.CreateBMOF(textFileName, bmofFileName, serverAndNamespace, optionFlags, classFlags, instanceFlags, info));
		return info;
	}

	public enum CreateOrUpdate : uint
	{
		Both = WbemFlags.WBEM_FLAG_CREATE_OR_UPDATE,
		CreateOnly = WbemFlags.WBEM_FLAG_CREATE_ONLY,
		UpdateOnly = WbemFlags.WBEM_FLAG_UPDATE_ONLY,
	}

	public enum UpdateMode : uint
	{
		Compatible = WbemFlags.WBEM_FLAG_UPDATE_COMPATIBLE,
		SafeMode = WbemFlags.WBEM_FLAG_UPDATE_SAFE_MODE,
		Force = WbemFlags.WBEM_FLAG_UPDATE_FORCE_MODE,
	}
}

/// <summary>
/// WBEM_COMPILE_STATUS_INFO
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public sealed class WbemCompileStatusInfo
{
	public int PhaseError;
	public int HResult;
	public int ObjectNum;
	public int FirstLine;
	public int LastLine;
	public uint OutFlags;
}