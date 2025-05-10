# PotisanBluetoothLELib

## サンプルコード

### バッテリーサービス対応デバイスのバッテリー残量を表示する。

```cs
using Potisan.Windows.BluetoothLE;
using Potisan.Windows.BluetoothLE.KnownShortUuids;

var deviceInfos = BluetoothLE.CreateDeviceInfoCollection();

// バッテリーサービス対応のデバイスを探す。
var deviceInfosWithBattery = deviceInfos.GetDeviceInfosByServiceUuid(KnownServiceShortUuid.Battery);

foreach (var deviceInfo in deviceInfosWithBattery)
{
	using var device = deviceInfo.Open();
	var batteryService = device.GetSingleServiceByUuid(KnownServiceShortUuid.Battery)!;
	Console.WriteLine($"{deviceInfo.Name} {batteryService.GetSingleCharacteristicByUuid(KnownCharacteristicsShortUuid.BatteryLevel)?.Value[0]}%");
}
```

## デバイスや特性を列挙する。

```cs
using Potisan.Windows.BluetoothLE;

var deviceInfos = BluetoothLE.CreateDeviceInfoCollection();

foreach (var deviceInfo in deviceInfos)
{
	// ここでデバイス情報を処理します。
	Console.WriteLine(deviceInfo.Name);

	// 可能なら使用後に解放してください。
	// ファイナライザでも閉じられます。
	using var device = deviceInfo.TryOpen();
	if (device is null)
		continue;

	// ここでデバイスを処理します。
	// ...

	// GATTサービスの取得
	foreach (var service in device.Services)
	{
		// ここでGATTサービスを処理します。
		Console.WriteLine($" {service.ServiceUuid}  IncludedServices: {{{string.Join(',', service.IncludedServices.Select(svc => svc.ServiceUuid))}}}");

		// GATTプロファイル特性の取得
		foreach (var characteristic in service.Characteristics)
		{
			// ここでGATTプロファイル特性を処理します。
			Console.WriteLine($"  {characteristic.CharacteristicUuid}  Value: {{{string.Join(',', characteristic.ValueOrNull ?? [])}}}");

			// GATTプロファイル特性記述子の取得
			foreach (var descriptor in characteristic.Descriptors)
			{
				// ここでGATTプロファイル特性記述子を処理します。
				Console.WriteLine($"   {descriptor.DescriptorUuid}");
			}
		}
	}
}
```

### GATTサービスデバイスや特性を列挙する。

```cs
using System.Reflection.PortableExecutable;

using Potisan.Windows.BluetoothLE;

var serviceDeviceInfos = BluetoothLE.CreateGattServiceDeviceInfoCollection();

foreach (var serviceDeviceInfo in serviceDeviceInfos)
{
	// ここでデバイス情報を処理します。
	Console.WriteLine(serviceDeviceInfo.Name);

	// 可能なら使用後に解放してください。
	// ファイナライザでも閉じられます。
	using var serviceDevice = serviceDeviceInfo.TryOpen();
	if (serviceDevice is null)
		continue;

	// ここでデバイスを処理します。
	Console.WriteLine($" IncludedServices: {{{string.Join(',', serviceDevice.IncludedServices)}}}");

	// GATTプロファイル特性の取得
	foreach (var characteristic in serviceDevice.Characteristics)
	{
		// ここでGATTプロファイル特性を処理します。
		Console.WriteLine($" {characteristic.CharacteristicUuid}  Value: {{{string.Join(',', characteristic.ValueOrNull ?? [])}}}");

		// GATTプロファイル特性記述子の取得
		foreach (var descriptor in characteristic.Descriptors)
		{
			// ここでGATTプロファイル特性記述子を処理します。
			Console.WriteLine($"  {descriptor.DescriptorUuid}");
		}
	}
}
```

## 謝辞

作成の過程で以下の情報を参照しています。ここに謝辞を申し上げます。

- Bluetooth LE Explorer (Microsoft) - Microsoft Store
- [DerekGn/WinBle - GitHub](https://github.com/DerekGn/WinBle)
