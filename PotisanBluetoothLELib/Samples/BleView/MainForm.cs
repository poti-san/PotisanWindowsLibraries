using System.Diagnostics;
using System.Text;

using Potisan.Windows.BluetoothLE;

namespace BleView;

internal partial class MainForm : Form
{
	public MainForm()
	{
		InitializeComponent();
	}

	private void MainForm_Load(object sender, EventArgs e)
	{
		using var devices = BluetoothLE.CreateDeviceInfoCollection();
		using var services = BluetoothLE.CreateGattServiceDeviceInfoCollection();

		DeviceListView.Items.AddRange(
			[.. devices.OfType<IBluetoothLEDeviceInfo>().Concat(services.OfType<IBluetoothLEDeviceInfo>())
			.Select(device => new ListViewItem([
				device is BluetoothLEDeviceInfo ? "デバイス" : "サービス",
				device.Name,
				$"{device.DeviceClassName} ({device.ClassGuid:B})",
				device.Path,
			])
			{ Tag = device })]);
		DeviceListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

		foreach (var device in devices)
		{
			if (device.TryOpen() is not { } dev)
				continue;
			using (dev)
			{
				foreach (var service in dev.Services)
				{
					foreach (var characteristic in service.Characteristics)
					{
						try
						{
							_events.Add(characteristic.RegisterEvent(BluetoothLEGattEventType.CharacteristicValueChangedEvent,
								static (_, eventOutParameter, _) => Debug.WriteLine($"BTH_LE_GATT_EVENT: {eventOutParameter}"), 0));
						}
						catch { }
					}
				}
			}
		}
	}

	private readonly List<BluetoothLEGattEvent> _events = [];

	private void DeviceListView_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (DeviceListView.SelectedIndices.Count != 1)
		{
			DeviceInfoTreeView.Nodes.Clear();
			return;
		}

		var selectedNode = DeviceListView.SelectedItems[0];

		DeviceInfoTreeView.BeginUpdate();
		var nodes = DeviceInfoTreeView.Nodes;
		var deviceInfo = selectedNode.Tag as IBluetoothLEDeviceInfo;
		var device = default(IBluetoothLEDevice);
		try
		{
			device = deviceInfo.Open();
		}
		catch (Exception ex)
		{
			statusMessageLbel.Text = ex.Message;
			DeviceInfoTreeView.EndUpdate();
			return;
		}

		// TODO 関連サービス対応
		using (device)
		{
			// デバイスの場合はサービスを追加と特性
			if (device is BluetoothLEDevice bleDevice)
			{
				nodes.AddRange([.. bleDevice.Services
					.Select(static service =>
					{
						var serviceDeviceInfo = service.GetServiceDeviceInfo();
						using var serviceDevice = serviceDeviceInfo.TryOpen();
						var serviceEnabled = serviceDevice != null;
						var serviceGuid = service.ServiceUuid.ToGuid().ToString("B");
						var node = new TreeNode($"{service.ServiceUuid} (DeviceClass: {serviceDeviceInfo.DeviceClassName}), Openable: {serviceEnabled}");
						// serviceDeviceも渡せますが、特性の取得元が変わってしまいます。
						AddServiceCharacteristicNodes(node.Nodes, service);
						return node;
					})
				]);
			}
			else
			{
				var serviceDevice = device as BluetoothLEGattServiceDevice;
				AddServiceCharacteristicNodes(nodes, serviceDevice);
			}
		}

		DeviceInfoTreeView.ExpandAll();
		if (DeviceInfoTreeView.Nodes.Count > 0)
			DeviceInfoTreeView.Nodes[0].EnsureVisible();
		DeviceInfoTreeView.EndUpdate();

		statusMessageLbel.Text = "完了";

		static void AddServiceCharacteristicNodes(TreeNodeCollection nodes, IBluetoothLEGattService service)
		{
			foreach (var (i, characteristic) in service.Characteristics.Index())
			{
				var charNodes = nodes.Add($"Characteristics #{i}: {characteristic.CharacteristicUuid}").Nodes;
				var attrs = new List<string>();
				if (characteristic.IsBroadcastable) attrs.Add("Broadcastable");
				if (characteristic.IsReadable) attrs.Add("Readable");
				if (characteristic.IsWritable) attrs.Add("Writable");
				if (characteristic.IsWritableWithoutResponse) attrs.Add("WritableWithoutResponse");
				if (characteristic.IsSignedWritable) attrs.Add("SignedWritable");
				if (characteristic.IsNotifiable) attrs.Add("Notifiable");
				if (characteristic.IsIndicatable) attrs.Add("Indicatable");
				if (characteristic.HasExtendedProperties) attrs.Add("HasExtendedProperties");
				charNodes.Add($"Attributes: {string.Join(", ", attrs)}");

				charNodes.Add($"ServiceHandle: {characteristic.ServiceHandle}");
				charNodes.Add($"CharacteristicUuid (Guid): {characteristic.CharacteristicUuid.ToGuid():B}");
				charNodes.Add($"AttributeHandle: {characteristic.AttributeHandle}");
				charNodes.Add($"CharacteristicValueHandle: {characteristic.CharacteristicValueHandle}");
				try
				{
					var value = characteristic.Value;
					if (characteristic.CharacteristicUuid.ShortUuid is 0x2A00)
						charNodes.Add($"Value: {Encoding.UTF8.GetString(value)}");
					else
						charNodes.Add($"Value: {string.Join(",", value)}");
				}
				catch
				{
					charNodes.Add("Value: <ERROR>");
				}

				// Descriptor
				foreach (var (j, desc) in characteristic.Descriptors.Index())
				{
					var descNodes = charNodes.Add($"Descriptor #{j}").Nodes;
					descNodes.Add($"ServiceHandle {desc.ServiceHandle}");
					descNodes.Add($"CharacteristicHandle {desc.CharacteristicHandle}");
					descNodes.Add($"DescriptorType {desc.DescriptorType}");
					descNodes.Add($"DescriptorUuid {desc.DescriptorUuid}");
					descNodes.Add($"AttributeHandle {desc.AttributeHandle}");
					var descValue = desc.ValueOrNull;
					if (descValue == null)
					{
						descNodes.Add("Value: <ERROR>");
					}
					else
					{
						switch (descValue.DescriptorType)
						{
						case BluetoothLEGattDescriptorType.CharacteristicExtendedProperties:
							{
								var nodes2 = descNodes.Add("CharacteristicExtendedProperties").Nodes;
								nodes2.Add($"IsReliableWriteEnabled: {descValue.CharacteristicExtendedProperties.IsReliableWriteEnabled}");
								nodes2.Add($"IsAuxiliariesWritable: {descValue.CharacteristicExtendedProperties.IsAuxiliariesWritable}");
							}
							break;
						case BluetoothLEGattDescriptorType.CharacteristicUserDescription:
							{
								var nodes2 = descNodes.Add("CharacteristicUserDescription").Nodes;
							}
							break;
						case BluetoothLEGattDescriptorType.ClientCharacteristicConfiguration:
							{
								var nodes2 = descNodes.Add("ClientCharacteristicConfiguration").Nodes;
								nodes2.Add($"IsSubscribeToNotification: {descValue.ClientCharacteristicConfiguration.IsSubscribeToNotification}");
								nodes2.Add($"IsSubscribeToIndication: {descValue.ClientCharacteristicConfiguration.IsSubscribeToIndication}");
							}
							break;
						case BluetoothLEGattDescriptorType.ServerCharacteristicConfiguration:
							{
								var nodes2 = descNodes.Add("ServerCharacteristicConfiguration").Nodes;
								nodes2.Add($"IsBroadcast: {descValue.ServerCharacteristicConfiguration.IsBroadcast}");
							}
							break;
						case BluetoothLEGattDescriptorType.CharacteristicFormat:
							{
								var nodes2 = descNodes.Add("CharacteristicFormat").Nodes;
								nodes2.Add($"Format: {descValue.CharacteristicFormat.Format}");
								nodes2.Add($"Exponent: {descValue.CharacteristicFormat.Exponent}");
								nodes2.Add($"Unit: {descValue.CharacteristicFormat.Unit}");
								nodes2.Add($"NameSpace: {descValue.CharacteristicFormat.NameSpace}");
								nodes2.Add($"Description: {descValue.CharacteristicFormat.Description}");
							}
							break;
						case BluetoothLEGattDescriptorType.CharacteristicAggregateFormat:
							{
								var nodes2 = descNodes.Add("CharacteristicAggregateFormat").Nodes;
							}
							break;
						case BluetoothLEGattDescriptorType.CustomDescriptor:
							{
								var nodes2 = descNodes.Add("CustomDescriptor").Nodes;
							}
							break;
						}
						descNodes.Add($"Data: {string.Join(',', descValue.Data.ToArray())}");
					}
				}
			}
		}
	}
}
