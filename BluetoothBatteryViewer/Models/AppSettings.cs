using System.Text.Json.Serialization;
using System.Windows.Forms; 

namespace BluetoothBatteryViewer.Models
{

    public sealed class AppSettings
    {
        public HotkeySetting Hotkey { get; set; } = new HotkeySetting();

        public bool RunAtStartup { get; set; }

        [JsonPropertyName("UseScreenReaderSpeech")]
        public bool UseScreenReaderSpeech { get; set; }

        [JsonPropertyName("UseZdsrSpeech")]
        public bool LegacyUseZdsrSpeech
        {
            get => UseScreenReaderSpeech;
            set => UseScreenReaderSpeech = value;
        }

        [JsonIgnore]
        public bool HasHotkey => Hotkey.Key != Keys.None && Hotkey.Modifiers != HotkeyModifiers.None;
    }

}