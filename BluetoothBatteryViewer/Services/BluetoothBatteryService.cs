using BluetoothBatteryViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace BluetoothBatteryViewer.Services
{
    public sealed class BluetoothBatteryService
    {
        public async Task<IReadOnlyList<BatteryDeviceInfo>> GetBatteryDevicesAsync()
        {
            var results = new List<BatteryDeviceInfo>();

            // 1. 获取所有包含电池服务的终端节点
            var selector = GattDeviceService.GetDeviceSelectorFromUuid(GattServiceUuids.Battery);
            var serviceEndpoints = await DeviceInformation.FindAllAsync(selector);

            foreach (var endpoint in serviceEndpoints)
            {
                try
                {
                    using (var service = await GattDeviceService.FromIdAsync(endpoint.Id))
                    {
                        if (service == null)
                        {
                            continue;
                        }

                        // SessionStatus.Active 表示当前会话处于活动状态，设备已连接且可通信
                        if (service.Session.SessionStatus != GattSessionStatus.Active)
                        {
                            continue;
                        }

                        var characteristicResult = await service.GetCharacteristicsForUuidAsync(
                            GattCharacteristicUuids.BatteryLevel,
                            BluetoothCacheMode.Cached
                        );

                        if (characteristicResult.Status != GattCommunicationStatus.Success)
                        {
                            continue;
                        }

                        var characteristic = characteristicResult.Characteristics.FirstOrDefault();
                        if (characteristic != null)
                        {
                            var readResult = await characteristic.ReadValueAsync(BluetoothCacheMode.Cached);

                            if (readResult.Status == GattCommunicationStatus.Success)
                            {
                                var reader = DataReader.FromBuffer(readResult.Value);
                                if (reader.UnconsumedBufferLength > 0)
                                {
                                    byte batteryLevel = reader.ReadByte();
                                    string deviceName = "未知设备";

                                    // 3. 核心修复：通过 Session 提供的 DeviceId 安全地反查获取所属蓝牙设备的名称
                                    // 仅在成功读取到电量后才执行此操作，最大化节省性能开销
                                    // 修正 2：将 using var 改为传统的 using 块
                                    using (var bleDevice = await BluetoothLEDevice.FromIdAsync(service.Session.DeviceId.Id))
                                    {
                                        if (bleDevice != null && !string.IsNullOrWhiteSpace(bleDevice.Name))
                                        {
                                            deviceName = bleDevice.Name;
                                        }
                                    }

                                    results.Add(new BatteryDeviceInfo
                                    {
                                        Name = deviceName,
                                        BatteryLevel = batteryLevel,
                                    });
                                }
                            }
                        }
                    }
                }
                catch
                {
                    // 忽略单个设备的异常，继续扫描下一个
                    continue;
                }
            }

            return results
                .GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .OrderBy(x => x.Name, StringComparer.CurrentCultureIgnoreCase)
                .ToList();
        }

        public async Task<string> BuildAnnouncementAsync()
        {
            var devices = await GetBatteryDevicesAsync();
            if (devices.Count == 0)
            {
                return "当前没有已连接且支持电量读取的蓝牙设备。";
            }

            return string.Join("。", devices.Select(d => $"{d.Name}，电量 {d.BatteryLevel}%")) + "。";
        }
    }
}