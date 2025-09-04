using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

/// <summary>
/// Utility class for managing Arduino port detection and connection
/// Shared between SerialCOM and SerialCOMSliders for consistency
/// </summary>
public static class ArduinoPortManager
{
    private static Dictionary<string, string> configCache = null;

    [System.Serializable]
    public class ArduinoDevice
    {
        public string portName;
        public string vid;
        public string pid;
        public string deviceName;

        public ArduinoDevice(string port, string vendorId, string productId, string name = "")
        {
            portName = port;
            vid = vendorId;
            pid = productId;
            deviceName = name;
        }
    }

    /// <summary>
    /// Find the first available Arduino port
    /// </summary>
    public static string FindArduinoPort()
    {
        try
        {
            LoadConfiguration();

            // Method 1: Try config file specified VID/PID
            string configPort = FindPortByConfigFile();
            if (!string.IsNullOrEmpty(configPort))
            {
                Debug.Log($"Arduino found via config: {configPort}");
                return configPort;
            }

            // Method 2: Try common Arduino VID/PID combinations
            if (GetConfigBool("AUTO_DETECT_COMMON_ARDUINO", true))
            {
                string commonPort = FindPortByCommonIds();
                if (!string.IsNullOrEmpty(commonPort))
                {
                    Debug.Log($"Arduino found via common IDs: {commonPort}");
                    return commonPort;
                }
            }

            // Method 3: Try all available ports as fallback
            if (GetConfigBool("TRY_ALL_PORTS_AS_FALLBACK", true))
            {
                string fallbackPort = TryAllAvailablePorts();
                if (!string.IsNullOrEmpty(fallbackPort))
                {
                    Debug.Log($"Arduino found via port scanning: {fallbackPort}");
                    return fallbackPort;
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in FindArduinoPort: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Get all Arduino devices found on the system
    /// </summary>
    public static List<ArduinoDevice> FindAllArduinoDevices()
    {
        List<ArduinoDevice> devices = new List<ArduinoDevice>();

        try
        {
            LoadConfiguration();

            // Get devices from config file VID/PID
            var configDevices = GetDevicesByConfigFile();
            devices.AddRange(configDevices);

            // Get devices from common VID/PID combinations
            if (GetConfigBool("AUTO_DETECT_COMMON_ARDUINO", true))
            {
                var commonDevices = GetDevicesByCommonIds();
                devices.AddRange(commonDevices.Where(d => !devices.Any(existing => existing.portName == d.portName)));
            }

            return devices;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error finding Arduino devices: {ex.Message}");
            return devices;
        }
    }

    private static void LoadConfiguration()
    {
        if (configCache != null) return;

        configCache = new Dictionary<string, string>();

        try
        {
            string configPath = Path.Combine(Application.streamingAssetsPath, "config.txt");
            if (!File.Exists(configPath))
            {
                Debug.LogWarning("Config file not found. Using default values.");
                return;
            }

            string[] lines = File.ReadAllLines(configPath);
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#"))
                    continue;

                string[] parts = trimmedLine.Split('=');
                if (parts.Length >= 2)
                {
                    string key = parts[0].Trim().ToUpper();
                    string value = string.Join("=", parts.Skip(1)).Trim();
                    configCache[key] = value;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Error loading config: {ex.Message}");
        }
    }

    private static string GetConfigValue(string key, string defaultValue = "")
    {
        LoadConfiguration();
        return configCache.TryGetValue(key.ToUpper(), out string value) ? value : defaultValue;
    }

    private static bool GetConfigBool(string key, bool defaultValue = false)
    {
        string value = GetConfigValue(key);
        return bool.TryParse(value, out bool result) ? result : defaultValue;
    }

    private static int GetConfigInt(string key, int defaultValue = 0)
    {
        string value = GetConfigValue(key);
        return int.TryParse(value, out int result) ? result : defaultValue;
    }

    private static string FindPortByConfigFile()
    {
        string vid = GetConfigValue("VID");
        string pid = GetConfigValue("PID");

        if (string.IsNullOrEmpty(vid) || string.IsNullOrEmpty(pid))
            return null;

        List<string> ports = GetComPortsByVidPid(vid, pid);
        return ValidateAndReturnWorkingPort(ports);
    }

    private static List<ArduinoDevice> GetDevicesByConfigFile()
    {
        List<ArduinoDevice> devices = new List<ArduinoDevice>();

        string vid = GetConfigValue("VID");
        string pid = GetConfigValue("PID");

        if (!string.IsNullOrEmpty(vid) && !string.IsNullOrEmpty(pid))
        {
            List<string> ports = GetComPortsByVidPid(vid, pid);
            foreach (string port in ports)
            {
                if (TestPortConnection(port))
                {
                    devices.Add(new ArduinoDevice(port, vid, pid, "Config Arduino"));
                }
            }
        }

        return devices;
    }

    private static string FindPortByCommonIds()
    {
        var commonIds = GetCommonArduinoIds();

        foreach (var (vid, pid) in commonIds)
        {
            List<string> ports = GetComPortsByVidPid(vid, pid);
            string workingPort = ValidateAndReturnWorkingPort(ports);
            if (!string.IsNullOrEmpty(workingPort))
            {
                return workingPort;
            }
        }

        return null;
    }

    private static List<ArduinoDevice> GetDevicesByCommonIds()
    {
        List<ArduinoDevice> devices = new List<ArduinoDevice>();
        var commonIds = GetCommonArduinoIds();

        foreach (var (vid, pid) in commonIds)
        {
            List<string> ports = GetComPortsByVidPid(vid, pid);
            foreach (string port in ports)
            {
                if (TestPortConnection(port))
                {
                    string deviceName = GetDeviceNameByVidPid(vid, pid);
                    devices.Add(new ArduinoDevice(port, vid, pid, deviceName));
                }
            }
        }

        return devices;
    }

    private static (string vid, string pid)[] GetCommonArduinoIds()
    {
        return new[]
        {
            ("2341", "0043"), // Arduino Uno
            ("2341", "0001"), // Arduino Uno (older)
            ("2A03", "0043"), // Arduino Uno clone
            ("1A86", "7523"), // CH340 chip (common in clones)
            ("10C4", "EA60"), // CP210x UART Bridge
            ("0403", "6001"), // FTDI chip
            ("2341", "8036"), // Arduino Leonardo
            ("2341", "0036"), // Arduino Leonardo (older)
            ("2341", "003B"), // Arduino Micro
            ("239A", "800B"), // Adafruit boards
            ("1B4F", "9206"), // SparkFun boards
            ("2341", "0010"), // Arduino Mega
            ("2341", "0042"), // Arduino Mega (R3)
            ("16C0", "0483")  // Teensy boards
        };
    }

    private static string GetDeviceNameByVidPid(string vid, string pid)
    {
        var nameMap = new Dictionary<string, string>
        {
            ["2341:0043"] = "Arduino Uno",
            ["2341:0001"] = "Arduino Uno",
            ["2A03:0043"] = "Arduino Uno Clone",
            ["1A86:7523"] = "Arduino (CH340)",
            ["10C4:EA60"] = "Arduino (CP210x)",
            ["0403:6001"] = "Arduino (FTDI)",
            ["2341:8036"] = "Arduino Leonardo",
            ["2341:0036"] = "Arduino Leonardo",
            ["2341:003B"] = "Arduino Micro",
            ["239A:800B"] = "Adafruit Board",
            ["1B4F:9206"] = "SparkFun Board",
            ["2341:0010"] = "Arduino Mega",
            ["2341:0042"] = "Arduino Mega R3",
            ["16C0:0483"] = "Teensy Board"
        };

        string key = $"{vid}:{pid}";
        return nameMap.TryGetValue(key, out string name) ? name : "Arduino Compatible";
    }

    private static string ValidateAndReturnWorkingPort(List<string> ports)
    {
        foreach (string port in ports)
        {
            if (TestPortConnection(port))
            {
                return port;
            }
        }
        return null;
    }

    public static bool TestPortConnection(string portName)
    {
        try
        {
            int baudRate = GetConfigInt("BAUDRATE", 9600);
            int timeout = GetConfigInt("READ_TIMEOUT", 1000);

            using (SerialPort testPort = new SerialPort(portName, baudRate))
            {
                testPort.ReadTimeout = Math.Min(timeout, 500);
                testPort.WriteTimeout = Math.Min(timeout, 500);
                testPort.DtrEnable = false;
                testPort.RtsEnable = false;

                testPort.Open();
                Thread.Sleep(100); // Give it a moment to stabilize
                testPort.Close();
                return true;
            }
        }
        catch
        {
            return false;
        }
    }

    private static string TryAllAvailablePorts()
    {
        if (GetConfigBool("ENABLE_DEBUG_LOGGING", false))
            Debug.Log("Trying all available COM ports...");

        string[] availablePorts = SerialPort.GetPortNames();

        foreach (string port in availablePorts)
        {
            if (TestPortConnection(port))
            {
                if (GetConfigBool("ENABLE_DEBUG_LOGGING", false))
                    Debug.Log($"Successfully connected to {port}");
                return port;
            }
        }

        return null;
    }

    private static List<string> GetComPortsByVidPid(string VID, string PID)
    {
        try
        {
            string pattern = $"^VID_{VID}.PID_{PID}";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            List<string> comports = new List<string>();

            using (RegistryKey rk1 = Registry.LocalMachine)
            using (RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum"))
            {
                if (rk2 == null) return comports;

                foreach (string deviceType in rk2.GetSubKeyNames())
                {
                    using (RegistryKey rk3 = rk2.OpenSubKey(deviceType))
                    {
                        if (rk3 == null) continue;

                        foreach (string deviceId in rk3.GetSubKeyNames())
                        {
                            if (regex.Match(deviceId).Success)
                            {
                                using (RegistryKey rk4 = rk3.OpenSubKey(deviceId))
                                {
                                    if (rk4 == null) continue;

                                    foreach (string instanceId in rk4.GetSubKeyNames())
                                    {
                                        using (RegistryKey rk5 = rk4.OpenSubKey(instanceId))
                                        using (RegistryKey rk6 = rk5?.OpenSubKey("Device Parameters"))
                                        {
                                            if (rk6?.GetValue("PortName") is string portName)
                                            {
                                                comports.Add(portName);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return comports;
        }
        catch (Exception ex)
        {
            if (GetConfigBool("ENABLE_DEBUG_LOGGING", false))
                Debug.LogWarning($"Registry access failed: {ex.Message}");
            return new List<string>();
        }
    }

    /// <summary>
    /// Get the configured baud rate
    /// </summary>
    public static int GetBaudRate()
    {
        return GetConfigInt("BAUDRATE", 9600);
    }

    /// <summary>
    /// Get the configured read timeout
    /// </summary>
    public static int GetReadTimeout()
    {
        return GetConfigInt("READ_TIMEOUT", 1000);
    }

    /// <summary>
    /// Get the configured write timeout
    /// </summary>
    public static int GetWriteTimeout()
    {
        return GetConfigInt("WRITE_TIMEOUT", 1000);
    }

    /// <summary>
    /// Get the configured retry interval
    /// </summary>
    public static float GetRetryInterval()
    {
        string value = GetConfigValue("RETRY_INTERVAL", "5.0");
        return float.TryParse(value, out float result) ? result : 5.0f;
    }

    /// <summary>
    /// Get the configured write throttle time
    /// </summary>
    public static float GetWriteThrottleTime()
    {
        string value = GetConfigValue("WRITE_THROTTLE_TIME", "0.1");
        return float.TryParse(value, out float result) ? result : 0.1f;
    }
}