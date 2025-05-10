using Potisan.Windows.BluetoothLE;
using Potisan.Windows.BluetoothLE.KnownShortUuids;

var deviceInfos = BluetoothLE.CreateDeviceInfoCollection();

// バッテリーサービス対応のデバイスを探す。
var deviceInfosWithBattery = deviceInfos.GetDevicesByServiceUuid(KnownServiceShortUuid.Battery);

foreach (var deviceInfo in deviceInfosWithBattery)
{
	using var device = deviceInfo.Open();
	var batteryService = device.GetSingleServiceByUuid(KnownServiceShortUuid.Battery)!;
	Console.WriteLine($"{deviceInfo.Name} {batteryService.GetSingleCharacteristicByUuid(KnownCharacteristicsShortUuid.BatteryLevel)?.Value[0]}%");
}
