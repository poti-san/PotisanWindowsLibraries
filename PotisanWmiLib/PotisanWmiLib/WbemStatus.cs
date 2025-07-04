﻿#pragma warning disable CA1707 // 識別子はアンダースコアを含むことはできません
#pragma warning disable CA1069 // 列挙値を重複させることはできない

namespace Potisan.Windows.Wmi;

/// <summary>
/// WMI関数の結果。
/// </summary>
public enum WbemStatus : uint
{
	WBEM_NO_ERROR = 0,
	WBEM_S_NO_ERROR = 0,
	WBEM_S_SAME = 0,
	WBEM_S_FALSE = 1,
	WBEM_S_ALREADY_EXISTS = 0x40001,
	WBEM_S_RESET_TO_DEFAULT = 0x40002,
	WBEM_S_DIFFERENT = 0x40003,
	WBEM_S_TIMEDOUT = 0x40004,
	WBEM_S_NO_MORE_DATA = 0x40005,
	WBEM_S_OPERATION_CANCELLED = 0x40006,
	WBEM_S_PENDING = 0x40007,
	WBEM_S_DUPLICATE_OBJECTS = 0x40008,
	WBEM_S_ACCESS_DENIED = 0x40009,
	WBEM_S_PARTIAL_RESULTS = 0x40010,
	WBEM_S_SOURCE_NOT_AVAILABLE = 0x40017,
	WBEM_E_FAILED = 0x80041001,
	WBEM_E_NOT_FOUND = 0x80041002,
	WBEM_E_ACCESS_DENIED = 0x80041003,
	WBEM_E_PROVIDER_FAILURE = 0x80041004,
	WBEM_E_TYPE_MISMATCH = 0x80041005,
	WBEM_E_OUT_OF_MEMORY = 0x80041006,
	WBEM_E_INVALID_CONTEXT = 0x80041007,
	WBEM_E_INVALID_PARAMETER = 0x80041008,
	WBEM_E_NOT_AVAILABLE = 0x80041009,
	WBEM_E_CRITICAL_ERROR = 0x8004100a,
	WBEM_E_INVALID_STREAM = 0x8004100b,
	WBEM_E_NOT_SUPPORTED = 0x8004100c,
	WBEM_E_INVALID_SUPERCLASS = 0x8004100d,
	WBEM_E_INVALID_NAMESPACE = 0x8004100e,
	WBEM_E_INVALID_OBJECT = 0x8004100f,
	WBEM_E_INVALID_CLASS = 0x80041010,
	WBEM_E_PROVIDER_NOT_FOUND = 0x80041011,
	WBEM_E_INVALID_PROVIDER_REGISTRATION = 0x80041012,
	WBEM_E_PROVIDER_LOAD_FAILURE = 0x80041013,
	WBEM_E_INITIALIZATION_FAILURE = 0x80041014,
	WBEM_E_TRANSPORT_FAILURE = 0x80041015,
	WBEM_E_INVALID_OPERATION = 0x80041016,
	WBEM_E_INVALID_QUERY = 0x80041017,
	WBEM_E_INVALID_QUERY_TYPE = 0x80041018,
	WBEM_E_ALREADY_EXISTS = 0x80041019,
	WBEM_E_OVERRIDE_NOT_ALLOWED = 0x8004101a,
	WBEM_E_PROPAGATED_QUALIFIER = 0x8004101b,
	WBEM_E_PROPAGATED_PROPERTY = 0x8004101c,
	WBEM_E_UNEXPECTED = 0x8004101d,
	WBEM_E_ILLEGAL_OPERATION = 0x8004101e,
	WBEM_E_CANNOT_BE_KEY = 0x8004101f,
	WBEM_E_INCOMPLETE_CLASS = 0x80041020,
	WBEM_E_INVALID_SYNTAX = 0x80041021,
	WBEM_E_NONDECORATED_OBJECT = 0x80041022,
	WBEM_E_READ_ONLY = 0x80041023,
	WBEM_E_PROVIDER_NOT_CAPABLE = 0x80041024,
	WBEM_E_CLASS_HAS_CHILDREN = 0x80041025,
	WBEM_E_CLASS_HAS_INSTANCES = 0x80041026,
	WBEM_E_QUERY_NOT_IMPLEMENTED = 0x80041027,
	WBEM_E_ILLEGAL_NULL = 0x80041028,
	WBEM_E_INVALID_QUALIFIER_TYPE = 0x80041029,
	WBEM_E_INVALID_PROPERTY_TYPE = 0x8004102a,
	WBEM_E_VALUE_OUT_OF_RANGE = 0x8004102b,
	WBEM_E_CANNOT_BE_SINGLETON = 0x8004102c,
	WBEM_E_INVALID_CIM_TYPE = 0x8004102d,
	WBEM_E_INVALID_METHOD = 0x8004102e,
	WBEM_E_INVALID_METHOD_PARAMETERS = 0x8004102f,
	WBEM_E_SYSTEM_PROPERTY = 0x80041030,
	WBEM_E_INVALID_PROPERTY = 0x80041031,
	WBEM_E_CALL_CANCELLED = 0x80041032,
	WBEM_E_SHUTTING_DOWN = 0x80041033,
	WBEM_E_PROPAGATED_METHOD = 0x80041034,
	WBEM_E_UNSUPPORTED_PARAMETER = 0x80041035,
	WBEM_E_MISSING_PARAMETER_ID = 0x80041036,
	WBEM_E_INVALID_PARAMETER_ID = 0x80041037,
	WBEM_E_NONCONSECUTIVE_PARAMETER_IDS = 0x80041038,
	WBEM_E_PARAMETER_ID_ON_RETVAL = 0x80041039,
	WBEM_E_INVALID_OBJECT_PATH = 0x8004103a,
	WBEM_E_OUT_OF_DISK_SPACE = 0x8004103b,
	WBEM_E_BUFFER_TOO_SMALL = 0x8004103c,
	WBEM_E_UNSUPPORTED_PUT_EXTENSION = 0x8004103d,
	WBEM_E_UNKNOWN_OBJECT_TYPE = 0x8004103e,
	WBEM_E_UNKNOWN_PACKET_TYPE = 0x8004103f,
	WBEM_E_MARSHAL_VERSION_MISMATCH = 0x80041040,
	WBEM_E_MARSHAL_INVALID_SIGNATURE = 0x80041041,
	WBEM_E_INVALID_QUALIFIER = 0x80041042,
	WBEM_E_INVALID_DUPLICATE_PARAMETER = 0x80041043,
	WBEM_E_TOO_MUCH_DATA = 0x80041044,
	WBEM_E_SERVER_TOO_BUSY = 0x80041045,
	WBEM_E_INVALID_FLAVOR = 0x80041046,
	WBEM_E_CIRCULAR_REFERENCE = 0x80041047,
	WBEM_E_UNSUPPORTED_CLASS_UPDATE = 0x80041048,
	WBEM_E_CANNOT_CHANGE_KEY_INHERITANCE = 0x80041049,
	WBEM_E_CANNOT_CHANGE_INDEX_INHERITANCE = 0x80041050,
	WBEM_E_TOO_MANY_PROPERTIES = 0x80041051,
	WBEM_E_UPDATE_TYPE_MISMATCH = 0x80041052,
	WBEM_E_UPDATE_OVERRIDE_NOT_ALLOWED = 0x80041053,
	WBEM_E_UPDATE_PROPAGATED_METHOD = 0x80041054,
	WBEM_E_METHOD_NOT_IMPLEMENTED = 0x80041055,
	WBEM_E_METHOD_DISABLED = 0x80041056,
	WBEM_E_REFRESHER_BUSY = 0x80041057,
	WBEM_E_UNPARSABLE_QUERY = 0x80041058,
	WBEM_E_NOT_EVENT_CLASS = 0x80041059,
	WBEM_E_MISSING_GROUP_WITHIN = 0x8004105a,
	WBEM_E_MISSING_AGGREGATION_LIST = 0x8004105b,
	WBEM_E_PROPERTY_NOT_AN_OBJECT = 0x8004105c,
	WBEM_E_AGGREGATING_BY_OBJECT = 0x8004105d,
	WBEM_E_UNINTERPRETABLE_PROVIDER_QUERY = 0x8004105f,
	WBEM_E_BACKUP_RESTORE_WINMGMT_RUNNING = 0x80041060,
	WBEM_E_QUEUE_OVERFLOW = 0x80041061,
	WBEM_E_PRIVILEGE_NOT_HELD = 0x80041062,
	WBEM_E_INVALID_OPERATOR = 0x80041063,
	WBEM_E_LOCAL_CREDENTIALS = 0x80041064,
	WBEM_E_CANNOT_BE_ABSTRACT = 0x80041065,
	WBEM_E_AMENDED_OBJECT = 0x80041066,
	WBEM_E_CLIENT_TOO_SLOW = 0x80041067,
	WBEM_E_NULL_SECURITY_DESCRIPTOR = 0x80041068,
	WBEM_E_TIMED_OUT = 0x80041069,
	WBEM_E_INVALID_ASSOCIATION = 0x8004106a,
	WBEM_E_AMBIGUOUS_OPERATION = 0x8004106b,
	WBEM_E_QUOTA_VIOLATION = 0x8004106c,
	WBEM_E_RESERVED_001 = 0x8004106d,
	WBEM_E_RESERVED_002 = 0x8004106e,
	WBEM_E_UNSUPPORTED_LOCALE = 0x8004106f,
	WBEM_E_HANDLE_OUT_OF_DATE = 0x80041070,
	WBEM_E_CONNECTION_FAILED = 0x80041071,
	WBEM_E_INVALID_HANDLE_REQUEST = 0x80041072,
	WBEM_E_PROPERTY_NAME_TOO_WIDE = 0x80041073,
	WBEM_E_CLASS_NAME_TOO_WIDE = 0x80041074,
	WBEM_E_METHOD_NAME_TOO_WIDE = 0x80041075,
	WBEM_E_QUALIFIER_NAME_TOO_WIDE = 0x80041076,
	WBEM_E_RERUN_COMMAND = 0x80041077,
	WBEM_E_DATABASE_VER_MISMATCH = 0x80041078,
	WBEM_E_VETO_DELETE = 0x80041079,
	WBEM_E_VETO_PUT = 0x8004107a,
	WBEM_E_INVALID_LOCALE = 0x80041080,
	WBEM_E_PROVIDER_SUSPENDED = 0x80041081,
	WBEM_E_SYNCHRONIZATION_REQUIRED = 0x80041082,
	WBEM_E_NO_SCHEMA = 0x80041083,
	WBEM_E_PROVIDER_ALREADY_REGISTERED = 0x80041084,
	WBEM_E_PROVIDER_NOT_REGISTERED = 0x80041085,
	WBEM_E_FATAL_TRANSPORT_ERROR = 0x80041086,
	WBEM_E_ENCRYPTED_CONNECTION_REQUIRED = 0x80041087,
	WBEM_E_PROVIDER_TIMED_OUT = 0x80041088,
	WBEM_E_NO_KEY = 0x80041089,
	WBEM_E_PROVIDER_DISABLED = 0x8004108a,
	WBEMESS_E_REGISTRATION_TOO_BROAD = 0x80042001,
	WBEMESS_E_REGISTRATION_TOO_PRECISE = 0x80042002,
	WBEMESS_E_AUTHZ_NOT_PRIVILEGED = 0x80042003,
	WBEMMOF_E_EXPECTED_QUALIFIER_NAME = 0x80044001,
	WBEMMOF_E_EXPECTED_SEMI = 0x80044002,
	WBEMMOF_E_EXPECTED_OPEN_BRACE = 0x80044003,
	WBEMMOF_E_EXPECTED_CLOSE_BRACE = 0x80044004,
	WBEMMOF_E_EXPECTED_CLOSE_BRACKET = 0x80044005,
	WBEMMOF_E_EXPECTED_CLOSE_PAREN = 0x80044006,
	WBEMMOF_E_ILLEGAL_CONSTANT_VALUE = 0x80044007,
	WBEMMOF_E_EXPECTED_TYPE_IDENTIFIER = 0x80044008,
	WBEMMOF_E_EXPECTED_OPEN_PAREN = 0x80044009,
	WBEMMOF_E_UNRECOGNIZED_TOKEN = 0x8004400a,
	WBEMMOF_E_UNRECOGNIZED_TYPE = 0x8004400b,
	WBEMMOF_E_EXPECTED_PROPERTY_NAME = 0x8004400c,
	WBEMMOF_E_TYPEDEF_NOT_SUPPORTED = 0x8004400d,
	WBEMMOF_E_UNEXPECTED_ALIAS = 0x8004400e,
	WBEMMOF_E_UNEXPECTED_ARRAY_INIT = 0x8004400f,
	WBEMMOF_E_INVALID_AMENDMENT_SYNTAX = 0x80044010,
	WBEMMOF_E_INVALID_DUPLICATE_AMENDMENT = 0x80044011,
	WBEMMOF_E_INVALID_PRAGMA = 0x80044012,
	WBEMMOF_E_INVALID_NAMESPACE_SYNTAX = 0x80044013,
	WBEMMOF_E_EXPECTED_CLASS_NAME = 0x80044014,
	WBEMMOF_E_TYPE_MISMATCH = 0x80044015,
	WBEMMOF_E_EXPECTED_ALIAS_NAME = 0x80044016,
	WBEMMOF_E_INVALID_CLASS_DECLARATION = 0x80044017,
	WBEMMOF_E_INVALID_INSTANCE_DECLARATION = 0x80044018,
	WBEMMOF_E_EXPECTED_DOLLAR = 0x80044019,
	WBEMMOF_E_CIMTYPE_QUALIFIER = 0x8004401a,
	WBEMMOF_E_DUPLICATE_PROPERTY = 0x8004401b,
	WBEMMOF_E_INVALID_NAMESPACE_SPECIFICATION = 0x8004401c,
	WBEMMOF_E_OUT_OF_RANGE = 0x8004401d,
	WBEMMOF_E_INVALID_FILE = 0x8004401e,
	WBEMMOF_E_ALIASES_IN_EMBEDDED = 0x8004401f,
	WBEMMOF_E_NULL_ARRAY_ELEM = 0x80044020,
	WBEMMOF_E_DUPLICATE_QUALIFIER = 0x80044021,
	WBEMMOF_E_EXPECTED_FLAVOR_TYPE = 0x80044022,
	WBEMMOF_E_INCOMPATIBLE_FLAVOR_TYPES = 0x80044023,
	WBEMMOF_E_MULTIPLE_ALIASES = 0x80044024,
	WBEMMOF_E_INCOMPATIBLE_FLAVOR_TYPES2 = 0x80044025,
	WBEMMOF_E_NO_ARRAYS_RETURNED = 0x80044026,
	WBEMMOF_E_MUST_BE_IN_OR_OUT = 0x80044027,
	WBEMMOF_E_INVALID_FLAGS_SYNTAX = 0x80044028,
	WBEMMOF_E_EXPECTED_BRACE_OR_BAD_TYPE = 0x80044029,
	WBEMMOF_E_UNSUPPORTED_CIMV22_QUAL_VALUE = 0x8004402a,
	WBEMMOF_E_UNSUPPORTED_CIMV22_DATA_TYPE = 0x8004402b,
	WBEMMOF_E_INVALID_DELETEINSTANCE_SYNTAX = 0x8004402c,
	WBEMMOF_E_INVALID_QUALIFIER_SYNTAX = 0x8004402d,
	WBEMMOF_E_QUALIFIER_USED_OUTSIDE_SCOPE = 0x8004402e,
	WBEMMOF_E_ERROR_CREATING_TEMP_FILE = 0x8004402f,
	WBEMMOF_E_ERROR_INVALID_INCLUDE_FILE = 0x80044030,
	WBEMMOF_E_INVALID_DELETECLASS_SYNTAX = 0x80044031
}
