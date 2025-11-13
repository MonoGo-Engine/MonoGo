using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using static MonoGo.Engine.AdditionalConverters;
using static MonoGo.Engine.Axis;
using static MonoGo.Engine.HSLColor;
using static MonoGo.Engine.HSLRange;
using static MonoGo.Engine.Range;
using static MonoGo.Engine.RangeF;

namespace MonoGo.Engine
{
    /// <summary>
    /// Marks a <see cref="JsonConverter"/> to be automatically registered in the MonoGo serialization system.
    /// </summary>
    /// <remarks>
    /// Apply this attribute to custom JSON converters in external modules to have them automatically
    /// discovered and registered during serialization initialization.
    /// The converter class must have a parameterless constructor.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class MonoGoConverterAttribute : Attribute
    {
    }

    public enum SerializeType
    {
        PostFX
    }

    /// <summary>
    /// This Serialization class is based on JSON and contains the <see cref="JsonSerializerOptions"/> and the <see cref="JsonConverter"/>'s used by the engine.
    /// </summary>
    /// <remarks>
    /// Use it for your own objects as well! A quick way of doing so would be using the "Serialize" or "Deserialize" method from this class.
    /// For advanced serialization and deserialization you should utilize <see cref="JsonSerializer"/> directly.
    /// </remarks>
    public static class Serialization
    {
        public static JsonSerializerOptions SerializerOptions { get; private set; }

        internal static void Init()
        {
            SerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
            {
                WriteIndented = true,
                PropertyNamingPolicy = null,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters =
                {
                    new VectorConverter(),
                    new HexColorConverter(),
                    new RectangleConverter(),
                    new PointConverter(),
                    new AxisConverter(),
                    new RangeConverter(),
                    new RangeFConverter(),
                    new HSLConverter(),
                    new ColourRangeConverter()
                }
            };

            // Load external converters from loaded assemblies
            LoadExternalConverters();
        }

        /// <summary>
        /// Scans all loaded assemblies for types marked with <see cref="MonoGoConverterAttribute"/> 
        /// and registers them as JSON converters.
        /// </summary>
        private static void LoadExternalConverters()
        {
            try
            {
                var assemblies = GameMgr.Assemblies.Values.ToArray();
                
                foreach (var assembly in assemblies)
                {
                    // Skip system assemblies for better performance
                    var assemblyName = assembly.FullName;
                    if (assemblyName == null || 
                        assemblyName.StartsWith("System", StringComparison.OrdinalIgnoreCase) || 
                        assemblyName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase) ||
                        assemblyName.StartsWith("netstandard", StringComparison.OrdinalIgnoreCase) ||
                        assemblyName.StartsWith("mscorlib", StringComparison.OrdinalIgnoreCase))
                        continue;

                    try
                    {
                        var converterTypes = assembly.GetTypes()
                            .Where(t => t.GetCustomAttribute<MonoGoConverterAttribute>() != null
                                     && typeof(JsonConverter).IsAssignableFrom(t)
                                     && !t.IsAbstract
                                     && t.GetConstructor(Type.EmptyTypes) != null);

                        foreach (var converterType in converterTypes)
                        {
                            var converter = (JsonConverter)Activator.CreateInstance(converterType)!;
                            SerializerOptions.Converters.Add(converter);
                        }
                    }
                    catch (ReflectionTypeLoadException)
                    {
                        // Assembly cannot be loaded, skip it
                        continue;
                    }
                    catch (Exception)
                    {
                        // Skip assemblies that throw other exceptions during type loading
                        continue;
                    }
                }
            }
            catch (Exception)
            {
                // Errors loading external converters should not prevent initialization
            }
        }

        public static JsonObject? Serialize(SerializeType type, string? fileName = null)
        {
            return type switch
            {
                SerializeType.PostFX => PostProcessing.Serialization.SerializePostFX(fileName),
                _ => null,
            };
        }

        public static JsonNode? Deserialize(SerializeType type, string? fileName = null)
        {
            return type switch
            {
                SerializeType.PostFX => PostProcessing.Serialization.DeserializePostFX(fileName),
                _ => null,
            };
        }

        /// <summary>
        /// Converts the provided value into a <see cref="string"/>.
        /// </summary>
        /// <typeparam name="TValue">The type to serialize.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <returns>A JSON string representation of the value.</returns>
        public static string Serialize<TValue>(TValue value, string? fileName = null)
        {
            var jsonString = JsonSerializer.Serialize(value, SerializerOptions);

            if (!string.IsNullOrEmpty(fileName))
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(fs))
                {
                    writer.Write(jsonString);
                }
            }

            return jsonString;
        }

        /// <summary>
        /// Serializes the specified value to a JSON node and optionally writes it to a file.
        /// </summary>
        /// <remarks>If a <paramref name="fileName"/> is provided, the method writes the JSON output to
        /// the specified file. The file is created or overwritten if it already exists.</remarks>
        /// <typeparam name="TValue">The type of the value to serialize.</typeparam>
        /// <param name="value">The value to serialize into a JSON node.</param>
        /// <param name="fileName">The optional file name to which the JSON representation will be written.  If <see langword="null"/> or
        /// empty, the JSON is not written to a file.</param>
        /// <returns>A <see cref="JsonNode"/> representing the serialized JSON of the specified value.</returns>
        public static JsonNode SerializeToNode<TValue>(TValue value, string? fileName = null)
        {
            var jsonString = JsonSerializer.SerializeToNode(value, SerializerOptions);

            if (!string.IsNullOrEmpty(fileName))
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(fs))
                {
                    writer.Write(jsonString);
                }
            }

            return jsonString;
        }

        public static TValue? Deserialize<TValue>(JsonNode? value = null, string? fileName = null)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string jsonString;
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(fs))
                {
                    jsonString = reader.ReadToEnd();
                }
                return JsonSerializer.Deserialize<TValue>(jsonString, SerializerOptions);
            }
            else return JsonSerializer.Deserialize<TValue>(value, SerializerOptions);
        }
    }

    /// <summary>
    /// Contains additional JSON converters used by the MonoGo engine.
    /// </summary>
    public static class AdditionalConverters
    {
        public class VectorConverter : JsonConverter<Vector2>
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(Vector2));
            }

            public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return Parse(reader.GetString()!);
            }

            public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(Write(value));
            }

            internal static Vector2 Parse(string value)
            {
                string trimed = value.TrimStart('(').TrimEnd(')');
                string[] xy = trimed.Split(';');

                var x = float.Parse(xy[0], NumberStyles.Float, CultureInfo.InvariantCulture);
                var y = float.Parse(xy[1], NumberStyles.Float, CultureInfo.InvariantCulture);

                return new Vector2(x, y);
            }

            internal static string Write(Vector2 value)
            {
                return string.Format(CultureInfo.InvariantCulture,
                    $"({value.X.ToString(CultureInfo.InvariantCulture)}; {value.Y.ToString(CultureInfo.InvariantCulture)})");
            }
        }
        public class HexColorConverter : JsonConverter<Color>
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(Color));
            }

            public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return ColorHelper.HexToColor(reader.GetString()!);
            }

            public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToHex());
            }
        }
        public class PointConverter : JsonConverter<Point>
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(Point));
            }

            public override Point Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();
                reader.Read(); var x = reader.GetInt32();
                reader.Read();
                reader.Read(); var y = reader.GetInt32();
                reader.Read();

                return new Point(x, y);
            }

            public override void Write(Utf8JsonWriter writer, Point value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteNumber("X", value.X);
                writer.WriteNumber("Y", value.Y);
                writer.WriteEndObject();
            }
        }
        public class RectangleConverter : JsonConverter<Rectangle>
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(Rectangle));
            }

            public override Rectangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();
                reader.Read(); var x = reader.GetInt32();
                reader.Read();
                reader.Read(); var y = reader.GetInt32();
                reader.Read();
                reader.Read(); var width = reader.GetInt32();
                reader.Read();
                reader.Read(); var height = reader.GetInt32();
                reader.Read();

                return new Rectangle(x, y, width, height);
            }

            public override void Write(Utf8JsonWriter writer, Rectangle value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteNumber("X", value.X);
                writer.WriteNumber("Y", value.Y);
                writer.WriteNumber("Width", value.Width);
                writer.WriteNumber("Height", value.Height);
                writer.WriteEndObject();
            }
        }
        
        /// <summary>
        /// Generic JSON converter that supports polymorphic serialization of base types and their derived types.
        /// Can be used by external modules to create converters for their own type hierarchies.
        /// </summary>
        /// <typeparam name="T">The base type or interface to serialize.</typeparam>
        public class BaseTypeJsonConverter<T> : JsonConverter<T>
        {
            private readonly Dictionary<string, Type> _baseTypes;

            /// <summary>
            /// Initializes a new instance of the <see cref="BaseTypeJsonConverter{T}"/> class.
            /// Scans the assembly containing type <typeparamref name="T"/> for all non-abstract types that implement or derive from it.
            /// </summary>
            public BaseTypeJsonConverter()
            {
                var typeList = typeof(T).GetTypeInfo().Assembly.DefinedTypes
                .Where(type => typeof(T).GetTypeInfo().IsAssignableFrom(type) && !type.IsAbstract);

                _baseTypes = typeList.ToDictionary(t => t.Name, t => t.AsType());
            }

            public override bool CanConvert(Type typeToConvert)
            {                
                return _baseTypes.ContainsValue(typeToConvert) || typeof(T) == typeToConvert;
            }

            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                reader.Read();
                var name = reader.GetString();

                using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
                {
                    var jObject = doc.RootElement;
                    if (_baseTypes.TryGetValue(name, out Type type))
                    {
                        var value = (T)JsonSerializer.Deserialize(jObject.GetRawText(), type, GetTempOptions(options));
                        reader.Read();
                        return value;
                    }
                }
                reader.Read();
                return default;
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                string name = ((dynamic)value).ToString();
                string shortName = name.Split('.').Last();

                writer.WriteStartObject();
                writer.WritePropertyName(shortName);
                JsonSerializer.Serialize(writer, (dynamic)value, GetTempOptions(options));
                writer.WriteEndObject();
            }

            /// <summary>
            /// Creates fresh serializer options to avoid infinite loops during (De-)Serialization.
            /// </summary>
            private JsonSerializerOptions GetTempOptions(JsonSerializerOptions options)
            {
                var tempOptions = new JsonSerializerOptions(options);
                
                // Remove all existing converters.
                foreach (var converter in tempOptions.Converters.ToList())
                {
                    tempOptions.Converters.Remove(converter);
                }
                
                // Re-Add all converters besides this one.
                foreach (var converter in options.Converters)
                {
                    if (converter == this) continue;
                    tempOptions.Converters.Add(converter);
                }
                return tempOptions;
            }
        }
    }
}
