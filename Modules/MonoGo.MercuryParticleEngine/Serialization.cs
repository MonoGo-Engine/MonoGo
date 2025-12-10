using MonoGo.Engine;
using MonoGo.MercuryParticleEngine.Modifiers;
using MonoGo.MercuryParticleEngine.Profiles;
using static MonoGo.Engine.AdditionalConverters;

namespace MonoGo.MercuryParticleEngine
{
    /// <summary>
    /// JSON converter for Profile types that supports polymorphic serialization.
    /// </summary>
    [MonoGoConverter]
    public class ProfileConverter : BaseTypeJsonConverter<Profile>
    {
    }

    /// <summary>
    /// JSON converter for IModifier types that supports polymorphic serialization.
    /// </summary>
    [MonoGoConverter]
    public class ModifierConverter : BaseTypeJsonConverter<IModifier>
    {
    }
}
