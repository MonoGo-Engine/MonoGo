using Microsoft.Xna.Framework;
using MonoGo.Engine.Particles.Modifiers;
using MonoGo.Engine.Particles.Profiles;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using static MonoGo.Engine.AdditionalConverters;
using static MonoGo.Engine.Axis;
using static MonoGo.Engine.HSLColor;
using static MonoGo.Engine.HSLRange;
using static MonoGo.Engine.Particles.ModifierExecutionStrategy;
using static MonoGo.Engine.Range;
using static MonoGo.Engine.RangeF;

namespace MonoGo.Engine
{
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
                Converters =
                {
                    new VectorConverter(),
                    new RectangleConverter(),
                    new PointConverter(),
                    new AxisConverter(),
                    new RangeConverter(),
                    new RangeFConverter(),
                    new HSLConverter(),
                    new ColourRangeConverter(),
                    new ModifierExecutionStrategyConverter(),
                    new BaseTypeJsonConverter<Profile>(),
                    new BaseTypeJsonConverter<IModifier>()
                }
            };
        }

        public static string? Serialize(SerializeType type, string? fileName = null)
        {
            return type switch
            {
                SerializeType.PostFX => PostProcessing.Serialization.Serialize(fileName),
                _ => null,
            };
        }

        public static string? Deserialize(SerializeType type, string? fileName = null)
        {
            return type switch
            {
                SerializeType.PostFX => PostProcessing.Serialization.Deserialize(fileName),
                _ => null,
            };
        }

        /// <summary>
        /// Converts the provided value into a <see cref="string"/>.
        /// </summary>
        /// <typeparam name="TValue">The type to serialize.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <returns>A JSON string representation of the value.</returns>
        public static string Serialize<TValue>(TValue value)
        {
            return JsonSerializer.Serialize(value, SerializerOptions);
        }

        /// <summary>
        /// Parses the text representing a single JSON value into a <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TValue">The type to deserialize the JSON value into.</typeparam>
        /// <param name="content">The string to deserialize.</param>
        /// <returns>A <typeparamref name="TValue"/> representation of the JSON value.</returns>
        public static TValue? Deserialize<TValue>(string content)
        {
            return JsonSerializer.Deserialize<TValue>(content, SerializerOptions);
        }
    }

    internal class AdditionalConverters
    {
        internal class VectorConverter : JsonConverter<Vector2>
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
        internal class PointConverter : JsonConverter<Point>
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
        internal class RectangleConverter : JsonConverter<Rectangle>
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
        internal class BaseTypeJsonConverter<T> : JsonConverter<T>
        {
            private readonly Dictionary<string, Type> _baseTypes;

            internal BaseTypeJsonConverter()
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
