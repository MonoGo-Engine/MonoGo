using System;
using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;

namespace MonoGo.Engine.PostProcessing
{
    internal static class Serialization
    {
        internal static string Serialize(string? fileName = null)
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

            var jsonObject = new JsonObject()
            {
                { "PostFX", postFXNode }
            };

            var jsonString = Engine.Serialization.Serialize(jsonObject);

            if (!string.IsNullOrEmpty(fileName))
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(fs))
                {
                    writer.Write(jsonString);
                }
            }

            return jsonObject.ToString();
        }

        internal static string? Deserialize(string? fileName = null)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string jsonString;
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(fs))
                {
                    jsonString = reader.ReadToEnd();
                }
                var jsonObject = Engine.Serialization.Deserialize<JsonNode>(jsonString);

                if (jsonObject != null)
                {
                    var postFX = jsonObject["PostFX"];
                    if (postFX != null)
                    {
                        var bloom = postFX["Bloom"];
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
                        var colorGrading = postFX["ColorGrading"];
                        if (colorGrading != null)
                        {
                            ColorGrading.SetLut(colorGrading["LUT"]!.ToString());
                        }
                    }
                    return jsonObject.ToString();
                }
            }
            return null;
        }
    }
}
