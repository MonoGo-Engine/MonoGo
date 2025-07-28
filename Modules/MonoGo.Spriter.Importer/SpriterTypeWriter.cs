// Copyright (C) The original author or authors
//
// This software may be modified and distributed under the terms
// of the zlib license.  See the LICENSE file for details.

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MonoGo.Spriter.Content;

namespace MonoGo.Spriter.Importer
{
    [ContentTypeWriter]
    public class SpriterTypeWriter : ContentTypeWriter<SpriterWrapper>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(SpriterTypeReader).AssemblyQualifiedName;
        }

        protected override void Write(ContentWriter output, SpriterWrapper value)
        {
            output.Write(value.SpriterData);
        }
    }
}
