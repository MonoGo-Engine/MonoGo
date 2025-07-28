// Copyright (C) The original author or authors
//
// This software may be modified and distributed under the terms
// of the zlib license.  See the LICENSE file for details.

using Microsoft.Xna.Framework.Content.Pipeline;
using SpriterDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace MonoGo.Spriter.Importer
{
    [ContentImporter(".json", DisplayName = "TexturePacker Sheet Importer - MonoGo", DefaultProcessor = "PassThroughProcessor")]
    public class TexturePackerSheetImporter : ContentImporter<TexturePackerSheetWrapper>
    {
        public override TexturePackerSheetWrapper Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage("Importing Spriter Atlas file: {0}", filename);
            string jsonData = File.ReadAllText(filename);

            TexturePackerSheetWrapper ret = new TexturePackerSheetWrapper();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new FramesJsonConverter(), new BooleanConverter() }
            };

            SpriterAtlasJson atlasJson = JsonSerializer.Deserialize<SpriterAtlasJson>(jsonData, options);
            TexturePackerSheet atlas = new TexturePackerSheet();
            atlasJson.Fill(atlas);

            XmlSerializer serializer = new XmlSerializer(typeof(TexturePackerSheet));

            using (StringWriter sww = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                serializer.Serialize(writer, atlas);
                ret.AtlasData = sww.ToString();
            }

            return ret;
        }
    }

    public class SpriterAtlasJson
    {
        public FramesJson Frames { get; set; }
        public Meta Meta { get; set; }

        public void Fill(TexturePackerSheet atlas)
        {
            atlas.Meta = Meta;
            atlas.ImageInfos = new List<ImageInfo>();

            if (Frames?.ImageInfos != null)
            {
                foreach (var entry in Frames.ImageInfos)
                {
                    ImageInfo info = entry.Value;
                    info.Name = entry.Key;
                    atlas.ImageInfos.Add(info);
                }
            }
        }
    }

    public class FramesJson
    {
        public Dictionary<string, ImageInfo> ImageInfos { get; set; }
    }

    public class FramesJsonConverter : JsonConverter<FramesJson>
    {
        public override FramesJson Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dict = JsonSerializer.Deserialize<Dictionary<string, ImageInfo>>(ref reader, options);
            return new FramesJson { ImageInfos = dict };
        }

        public override void Write(Utf8JsonWriter writer, FramesJson value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }

    public class BooleanConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();
            string chkValue = value.ToLower();
            if (chkValue.Equals("true") || chkValue.Equals("yes") || chkValue.Equals("1"))
            {
                return true;
            }
            if (chkValue.Equals("false") || chkValue.Equals("no") || chkValue.Equals("0"))
            {
                return false;
            }
            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}