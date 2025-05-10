#pragma warning disable CA1069 // 列挙値を重複させることはできない
#pragma warning disable CA1720 // 識別子に型名が含まれます

namespace Potisan.Windows.Wmi;

internal enum WbemFlags : uint
{
	WBEM_FLAG_RETURN_IMMEDIATELY = 0x10,
	WBEM_FLAG_RETURN_WBEM_COMPLETE = 0,
	WBEM_FLAG_BIDIRECTIONAL = 0,
	WBEM_FLAG_FORWARD_ONLY = 0x20,
	WBEM_FLAG_NO_ERROR_OBJECT = 0x40,
	WBEM_FLAG_RETURN_ERROR_OBJECT = 0,
	WBEM_FLAG_SEND_STATUS = 0x80,
	WBEM_FLAG_DONT_SEND_STATUS = 0,
	WBEM_FLAG_ENSURE_LOCATABLE = 0x100,
	WBEM_FLAG_DIRECT_READ = 0x200,
	WBEM_FLAG_SEND_ONLY_SELECTED = 0,
	WBEM_RETURN_WHEN_COMPLETE = 0,
	WBEM_RETURN_IMMEDIATELY = 0x10,
	WBEM_MASK_RESERVED_FLAGS = 0x1f000,
	WBEM_FLAG_USE_AMENDED_QUALIFIERS = 0x20000,
	WBEM_FLAG_STRONG_VALIDATION = 0x100000,

	WBEM_FLAG_ALWAYS = 0,
	WBEM_FLAG_ONLY_IF_TRUE = 0x1,
	WBEM_FLAG_ONLY_IF_FALSE = 0x2,
	WBEM_FLAG_ONLY_IF_IDENTICAL = 0x3,
	WBEM_MASK_PRIMARY_CONDITION = 0x3,
	WBEM_FLAG_KEYS_ONLY = 0x4,
	WBEM_FLAG_REFS_ONLY = 0x8,
	WBEM_FLAG_LOCAL_ONLY = 0x10,
	WBEM_FLAG_PROPAGATED_ONLY = 0x20,
	WBEM_FLAG_SYSTEM_ONLY = 0x30,
	WBEM_FLAG_NONSYSTEM_ONLY = 0x40,
	WBEM_MASK_CONDITION_ORIGIN = 0x70,
	WBEM_FLAG_CLASS_OVERRIDES_ONLY = 0x100,
	WBEM_FLAG_CLASS_LOCAL_AND_OVERRIDES = 0x200,
	WBEM_MASK_CLASS_CONDITION = 0x300,

	WBEM_FLAG_DEEP = 0,
	WBEM_FLAG_SHALLOW = 1,
	WBEM_FLAG_PROTOTYPE = 2,

	WBEM_FLAG_CREATE_OR_UPDATE = 0,
	WBEM_FLAG_UPDATE_ONLY = 0x1,
	WBEM_FLAG_CREATE_ONLY = 0x2,
	WBEM_FLAG_UPDATE_COMPATIBLE = 0,
	WBEM_FLAG_UPDATE_SAFE_MODE = 0x20,
	WBEM_FLAG_UPDATE_FORCE_MODE = 0x40,
	WBEM_MASK_UPDATE_MODE = 0x60,
	WBEM_FLAG_ADVISORY = 0x10000,

	WBEM_FLAG_OWNER_UPDATE = 0x10000,

	WBEM_FLAG_NO_FLAVORS = 0x1,

	WBEM_FLAG_ALLOW_READ = 0x1,

	WBEM_FLAG_BACKUP_RESTORE_DEFAULT = 0,
	WBEM_FLAG_BACKUP_RESTORE_FORCE_SHUTDOWN = 1,

	WBEM_FLAG_EXCLUDE_OBJECT_QUALIFIERS = 0x10,
	WBEM_FLAG_EXCLUDE_PROPERTY_QUALIFIERS = 0x20,

	WBEM_FLAG_REFRESH_AUTO_RECONNECT = 0,
	WBEM_FLAG_REFRESH_NO_AUTO_RECONNECT = 1,

	WBEM_FLAG_SHORT_NAME = 0x1,
	WBEM_FLAG_LONG_NAME = 0x2,
}

/// <summary>
/// WBEM_COMPILER_OPTIONS
/// </summary>
[Flags]
internal enum WbemCompilerOption : uint
{
	WBEM_FLAG_CHECK_ONLY = 0x1,
	WBEM_FLAG_AUTORECOVER = 0x2,
	WBEM_FLAG_WMI_CHECK = 0x4,
	WBEM_FLAG_CONSOLE_PRINT = 0x8,
	WBEM_FLAG_DONT_ADD_TO_LIST = 0x10,
	WBEM_FLAG_SPLIT_FILES = 0x20,
	WBEM_FLAG_STORE_FILE = 0x100
}

public enum WbemGenus : uint
{
	Class = 1,
	Instance = 2
}

/// <summary>
/// WBEM_FLAVOR_TYPE
/// </summary>
[Flags]
public enum WbemFlavor : uint
{
	DontPropagate = 0,
	PropagateToInstance = 0x1,
	PropagateToDerivedClass = 0x2,
	// WBEM_FLAVOR_MASK_PROPAGATION = 0xf,
	Overridable = 0,
	NoyOverridable = 0x10,
	// WBEM_FLAVOR_MASK_PERMISSIONS = 0x10,
	OriginLocal = 0,
	OriginPropagated = 0x20,
	OriginSystem = 0x40,
	// WBEM_FLAVOR_MASK_ORIGIN = 0x60,
	NotAmended = 0,
	Amended = 0x80,
	// WBEM_FLAVOR_MASK_AMENDED = 0x80
}

internal enum WBEM_SECURITY_FLAGS : uint
{
	WBEM_ENABLE = 1,
	WBEM_METHOD_EXECUTE = 2,
	WBEM_FULL_WRITE_REP = 4,
	WBEM_PARTIAL_WRITE_REP = 8,
	WBEM_WRITE_PROVIDER = 0x10,
	WBEM_REMOTE_ACCESS = 0x20,
	WBEM_RIGHT_SUBSCRIBE = 0x40,
	WBEM_RIGHT_PUBLISH = 0x80
}

/// <summary>
/// WBEM_COMPARISON_FLAG
/// </summary>
[Flags]
public enum WbemComparison : uint
{
	IncludeAll = 0,
	IgnoreQualifiers = 0x1,
	IgnoreObjectSource = 0x2,
	IgnoreDefaultValues = 0x4,
	IgnoreClass = 0x8,
	IgnoreCase = 0x10,
	IgnoreFlavor = 0x20,
}

/// <summary>
/// CIMTYPE
/// </summary>
[Flags]
public enum CimType : uint
{
	Illegal = 0xfff,
	Empty = 0,
	Int8 = 16,
	UInt8 = 17,
	Int16 = 2,
	UInt16 = 18,
	Int32 = 3,
	UInt32 = 19,
	Int64 = 20,
	UInt64 = 21,
	Float = 4,
	Double = 5,
	Boolean = 11,
	String = 8,
	DateTime = 101,
	Reference = 102,
	WChar = 103,
	Object = 13,
	Array = 0x2000
}

/// <summary>
/// WBEM_SHUTDOWN_FLAGS
/// </summary>
[Flags]
internal enum WbemShutdownFlag : uint
{
	UnloadComponent = 1,
	Wmi = 2,
	OS = 3
}

/// <summary>
/// WBEMSTATUS_FORMAT
/// </summary>
internal enum WbemStatusFormat : uint
{
	NewLine = 0,
	NoNewLine = 1
}

public enum WbemLimits : uint
{
	MaxID = 0x1000,
	MaxQuery = 0x4000,
	MaxPath = 0x2000,
	MaxObjectNesting = 64,
	MaxUserProperties = 1024
}
