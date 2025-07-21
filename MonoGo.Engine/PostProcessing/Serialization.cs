using System;
using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;

namespace MonoGo.Engine.PostProcessing
{
    public static class Serialization
    {
        internal static JsonObject SerializePostFX(string? fileName = null)
        {
            var bloomNode = new JsonObject
            {
                { "Preset", Bloom.BloomPreset.ToString() },
                { "Threshold", Bloom.Threshold },
                { "StreakLength", Bloom.StreakLength }
            };

            var colorGradingNode = new JsonObject
            {
                { "LUT", ColorGrading.CurrentLUT.Name }
            };

            var postFXNode = new JsonObject
            {
                { "Bloom", bloomNode },
                { "ColorGrading", colorGradingNode }
            };

            var jsonString = Engine.Serialization.Serialize(postFXNode);

            if (!string.IsNullOrEmpty(fileName))
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(fs))
                {
                    writer.Write(jsonString);
                }
            }

            return postFXNode;
        }

        internal static JsonNode? DeserializePostFX(string? fileName = null)
        {
            var jsonObject = Engine.Serialization.Deserialize<JsonObject>(fileName: fileName);
            return ReadPostFX(jsonObject);
        }

        public static JsonNode? ReadPostFX(JsonNode? jsonObject)
        {
            if (jsonObject != null)
            {
                var bloom = jsonObject["Bloom"];
                if (bloom != null)
                {
                    if (Enum.TryParse<BloomPresets>(bloom["Preset"]!.ToString(), true, out var preset))
                    {
                        Bloom.Preset(preset);
                    }
                    if (float.TryParse(bloom["Threshold"]!.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var threshold))
                    {
                        Bloom.Threshold = threshold;
                    }
                    if (float.TryParse(bloom["StreakLength"]!.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var streakLength))
                    {
                        Bloom.StreakLength = streakLength;
                    }
                }
                var colorGrading = jsonObject["ColorGrading"];
                if (colorGrading != null)
                {
                    ColorGrading.SetLut(colorGrading["LUT"]!.ToString());
                }
                return jsonObject;
            }
            return null;
        }
    }
}
