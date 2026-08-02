using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YARG.Core;
using YARG.Core.Logging;

namespace YARG.Input.Serialization
{
    // Version 3: The big profile rework

    // Unchanged data types
    using SerializedControllerV3 = SerializedInputDeviceV0;
    using SerializedMicV3 = SerializedMicV0;

    public class SerializedBindingsV3
    {
        public const int VERSION = 2;

        public int Version = VERSION;
        public Dictionary<Guid, SerializedProfileDeviceInfoV3> Profiles = new();
        public Dictionary<Guid, Guid> Controllers = new();
        public List<SerializedReusableBindingSetV3> ReusableBindingSets = new();

        [JsonConstructor]
        public SerializedBindingsV3() { }

        public SerializedBindingsV3(SerializedBindings serialized)
        {
            foreach (var (id, info) in serialized.Profiles)
            {
                Profiles[id] = new SerializedProfileDeviceInfoV3(info);
            }

            foreach (var (id, bindingsId) in serialized.Controllers)
            {
                Controllers[id] = bindingsId;
            }

            foreach (var reusableBindingSet in serialized.ReusableBindingSets)
            {
                ReusableBindingSets.Add(new(reusableBindingSet));
            }
        }

        public SerializedBindings Deserialize()
        {
            var deserialized = new SerializedBindings();
            foreach (var (id, bind) in Profiles)
            {
                deserialized.Profiles[id] = bind.Deserialize();
            }

            return deserialized;
        }
    }

    public class SerializedProfileDeviceInfoV3
    {
        public List<SerializedControllerV3> Controllers = new();
        public List<SerializedMicV3> Mics = new();

        // Key is controller hash, value is SerializedReusableBindingSet GUID
        // If one of these controllers is in Controllers, use the corresponding reusable binding set
        public Dictionary<string, Guid> ControllerMappings = new();

        // Key is PlasticBand layout, value is SerializedReusableBindingSet GUID
        // For each controller that isn't in Controllers, fall back to the corresponding reusable binding set for that controller's layout
        // If this fails too, we'll check if the device itself has a default layout, and if not then we use the hardcoded default bindings
        public Dictionary<string, Guid> LayoutMappings = new();

        [JsonConstructor]
        public SerializedProfileDeviceInfoV3() { }

        public SerializedProfileDeviceInfoV3(SerializedProfileDeviceInfo serialized)
        {
            Controllers.AddRange(serialized.Controllers.Select((controller) => new SerializedControllerV3(controller)));
            Mics.AddRange(serialized.Mics.Select((mic) => new SerializedMicV3(mic)));

            foreach (var (controllerHash, reusableBindingSetId) in serialized.ControllerMappings)
            {
                ControllerMappings[controllerHash] = reusableBindingSetId;
            }

            foreach (var (layout, reusableBindingSetId) in serialized.LayoutMappings)
            {
                LayoutMappings[layout] = reusableBindingSetId;
            }
        }

        public SerializedProfileDeviceInfo Deserialize()
        {
            var deserialized = new SerializedProfileDeviceInfo();

            deserialized.Controllers.AddRange(Controllers.Select((controller) => controller.Deserialize()));
            deserialized.Mics.AddRange(Mics.Select((mic) => mic.Deserialize()));

            foreach (var (controllerHash, reusableBindingSetId) in ControllerMappings)
            {
                deserialized.ControllerMappings[controllerHash] = reusableBindingSetId;
            }

            foreach (var (layout, reusableBindingSetId) in LayoutMappings)
            {
                deserialized.LayoutMappings[layout] = reusableBindingSetId;
            }

            return deserialized;
        }
    }

    public class SerializedReusableBindingSetV3
    {
        public string Name;
        public Guid Id;
        public SerializedBindingCollectionV3 Bindings = new();

        [JsonConstructor]
        public SerializedReusableBindingSetV3() { }

        public SerializedReusableBindingSetV3(SerializedReusableBindingSet reusableBindingSet)
        {
            Name = reusableBindingSet.Name;
            Id = reusableBindingSet.Id;
            Bindings = new(reusableBindingSet);
        }
    }

    public class SerializedBindingCollectionV3
    {
        public Dictionary<string, SerializedControlBindingV3> Bindings = new();

        [JsonConstructor]
        public SerializedBindingCollectionV3() { }

        public SerializedBindingCollectionV3(SerializedBindingCollection serialized)
        {
            foreach (var (id, serializedBinds) in serialized.Bindings)
            {
                Bindings[id] = new SerializedControlBindingV3(serializedBinds);
            }
        }

        public SerializedBindingCollection Deserialize()
        {
            var converted = new SerializedBindingCollection();
            foreach (var (id, serializedBinds) in Bindings)
            {
                converted.Bindings[id] = serializedBinds.Deserialize();
            }

            return converted;
        }
    }

    public class SerializedControlBindingV3
    {
        public Dictionary<string, string> Parameters = new();
        public List<SerializedInputControlV3> Controls = new();

        [JsonConstructor]
        public SerializedControlBindingV3() { }

        public SerializedControlBindingV3(SerializedControlBinding serialized)
        {
            foreach (var (name, value) in serialized.Parameters)
            {
                Parameters.Add(name, value);
            }

            Controls.AddRange(serialized.Controls.Select((bind) => new SerializedInputControlV3(bind)));
        }

        public SerializedControlBinding Deserialize()
        {
            var control = new SerializedControlBinding();

            foreach (var (name, value) in Parameters)
            {
                control.Parameters.Add(name, value);
            }

            foreach (var bind in Controls)
            {
                var deserialized = bind.Deserialize();
                if (deserialized is null)
                    continue;

                control.Controls.Add(deserialized);
            }

            return control;
        }
    }

    public class SerializedInputControlV3
    {
        public string ControlPath;
        public Dictionary<string, string> Parameters = new();

        [JsonConstructor]
        public SerializedInputControlV3()
        {
            ControlPath = string.Empty;
        }

        public SerializedInputControlV3(SerializedInputControl serialized)
        {
            ControlPath = serialized.ControlPath;
            Parameters = serialized.Parameters;
        }

        public SerializedInputControl? Deserialize()
        {
            return new(ControlPath)
            {
                Parameters = Parameters
            };
        }

        public bool ShouldSerializeParameters() => Parameters.Count > 0;
    }

    public static partial class BindingSerialization
    {
        private static SerializedBindingsV3 SerializeBindingsV3(SerializedBindings serialized)
        {
            return new SerializedBindingsV3(serialized);
        }

        private static SerializedBindings? DeserializeBindingsV3(JObject obj)
        {
            var serialized = obj.ToObject<SerializedBindingsV3>();
            if (serialized is null || serialized.Version != SerializedBindingsV3.VERSION)
                return null;

            return serialized.Deserialize();
        }
    }
}
