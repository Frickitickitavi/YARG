using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using YARG.Audio;
using YARG.Core;
using YARG.Core.Logging;
using YARG.Core.Audio;

#nullable enable

namespace YARG.Input.Serialization
{
    // These classes are what the bindings will use for serialization/deserialization.
    // They are *not* what will get written to the bindings file in the end, however,
    // since bindings are versioned. These are used to separate the written format and actual loaded format,
    // and to make adding things easier, since each layer of the serialization has its own type.
    //
    // When making changes to the bindings format, create a copy of the current `BindingsVersion.vX.cs` file
    // and make your changes to that. **Do not modify the existing version files!**
    // Next, make changes to the classes below, if needed, e.g. new data needs to be stored/loaded.
    // Finally, update SerializeBindings/DeserializeBindings here to reflect the new version:
    // - Make SerializeBindings serialize to the new version of the format.
    // - Add a new case branch to DeserializeBindings for the new version.

    public class SerializedBindings
    {
        public Dictionary<Guid, SerializedProfileDeviceInfo> Profiles = new();
        public Dictionary<Guid, Guid> Controllers = new(); // Controller config GUID to reusable binding set GUID
        public List<SerializedReusableBindingSet> ReusableBindingSets = new();
    }

    public class SerializedProfileDeviceInfo // previously called SerializedProfileBindings
    {
        public List<SerializedInputDevice> Controllers = new();
        public List<SerializedMic> Mics = new();

        // Key is controller hash, value is SerializedReusableBindingSet GUID
        // If one of these controllers is in Controllers, use the corresponding reusable binding set
        public Dictionary<string, Guid> ControllerMappings = new();

        // Key is PlasticBand layout, value is SerializedReusableBindingSet GUID
        // For each controller that isn't in Controllers, fall back to the corresponding reusable binding set for that controller's layout
        // If this fails too, we'll check if the device itself has a default layout, and if not then we use the hardcoded default bindings
        public Dictionary<string, Guid> LayoutMappings = new();

        public SerializedBindingCollection? MenuMappings;
    }

    public class SerializedReusableBindingSet : SerializedBindingCollection
    {
        public string Name;
        public Guid Id;
    }
    public class SerializedBindingCollection
    {
        public Dictionary<string, SerializedControlBinding> Bindings = new();
    }

    public class SerializedControlBinding
    {
        public Dictionary<string, string> Parameters = new();
        public List<SerializedInputControl> Controls = new();
    }

    public class SerializedInputDevice
    {
        public string Layout;
        public string Hash;

        public SerializedInputDevice(string layout, string hash)
        {
            Layout = layout;
            Hash = hash;
        }

        public SerializedInputDevice(InputDevice device)
        {
            Layout = device.layout;
            Hash = device.GetHash();
        }

        public bool MatchesDevice(InputDevice device)
        {
            return Layout == device.layout && Hash == device.GetHash();
        }
    }

    public class SerializedInputControl
    {
        public string ControlPath;
        public Dictionary<string, string> Parameters = new();

        public SerializedInputControl(string path)
        {
            ControlPath = path;
        }
    }

    public static partial class BindingSerialization
    {
        private static readonly SHA1 _hashAlgorithm = SHA1.Create();
        private static readonly Regex _xinputUserIndexRegex = new(@"\\""userIndex\\"":\s*\d,");

        private static readonly Dictionary<InputDevice, string> _hashCache = new();

        public static SerializedInputDevice Serialize(this InputDevice device)
        {
            return new(device);
        }

        public static string GetHash(this InputDevice device)
        {
            // Check if we have a calculated hash cached already
            if (_hashCache.TryGetValue(device, out string hash))
                return hash;

            var description = device.description;
            string descriptionJson = description.ToJson();
            // Exclude user index on XInput devices
            if (description.interfaceName == "XInput")
                descriptionJson = _xinputUserIndexRegex.Replace(descriptionJson, "");

            // Calculate the hash
            var descriptionBytes = Encoding.Default.GetBytes(descriptionJson);
            var hashBytes = _hashAlgorithm.ComputeHash(descriptionBytes);
            hash = BitConverter.ToString(hashBytes).Replace("-", "");

            // Cache the calculated hash
            _hashCache.Add(device, hash);

            return hash;
        }

        public static void SerializeBindings(SerializedBindings bindings, string bindingsPath)
        {
            try
            {
                var serialized = SerializeBindingsV3(bindings);
                string bindingsJson = JsonConvert.SerializeObject(serialized, Formatting.Indented);
                File.WriteAllText(bindingsPath, bindingsJson);
            }
            catch (Exception ex)
            {
                YargLogger.LogException(ex, "Error while saving bindings!");
            }
        }

        public static SerializedBindings? DeserializeBindings(string bindingsPath)
        {
            try
            {
                if (!File.Exists(bindingsPath))
                    return null;

                string bindingsJson = File.ReadAllText(bindingsPath);
                var jObject = JObject.Parse(bindingsJson);

                int version = jObject["Version"] switch
                {
                    null => 0,
                    { Type: JTokenType.Integer } versionToken => (int) versionToken,
                    {} unhandled => throw new InvalidDataException($"Invalid bindings version! Expected JSON type {JTokenType.Integer}, got {unhandled.Type}")
                };

                var bindings = version switch
                {
                    0 => DeserializeBindingsV0(jObject),
                    1 => DeserializeBindingsV1(jObject),
                    2 => DeserializeBindingsV2(jObject),
                    3 => DeserializeBindingsV3(jObject),
                    _ => throw new NotImplementedException($"Unhandled bindings version {version}!")
                };

                return bindings;
            }
            catch (Exception ex)
            {
                YargLogger.LogException(ex, "Error while loading bindings!");
                return null;
            }
        }
    }
}