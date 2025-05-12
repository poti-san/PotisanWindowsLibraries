// 16ビットUUIDの個別説明は公式ドキュメントに任せます。
#pragma warning disable CS1591

namespace Potisan.Windows.BluetoothLE.KnownShortUuids;

/// <summary>
/// 定義済みのBluetoothLE特性16ビットUUID。
/// 詳細はBluetoothLE公式ドキュメントを参照してください。
/// </summary>
public enum KnownDescriptorShortUuid : ushort
{
	CharacteristicExtendedProperties = 0x2900,
	CharacteristicUserDescription = 0x2901,
	ClientCharacteristicConfiguration = 0x2902,
	ServerCharacteristicConfiguration = 0x2903,
	CharacteristicPresentationFormat = 0x2904,
	CharacteristicAggregateFormat = 0x2905,
	ValidRange = 0x2906,
	ExternalReportReference = 0x2907,
	ReportReference = 0x2908,
	NumberOfDigitals = 0x2909,
	ValueTriggerSetting = 0x290A,
	EnvironmentalSensingConfiguration = 0x290B,
	EnvironmentalSensingMeasurement = 0x290C,
	EnvironmentalSensingTriggerSetting = 0x290D,
	TimeTriggerSetting = 0x290E,
	CompleteBREdrTransportBlockData = 0x290F,
	ObservationSchedule = 0x2910,
	ValidRangeAndAccuracy = 0x2911,
	MeasurementDescription = 0x2912,
	ManufacturerLimits = 0x2913,
	ProcessTolerances = 0x2914,
	ImdTriggerSetting = 0x2915,
}
