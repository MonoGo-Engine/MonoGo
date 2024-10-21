namespace MonoGo.Engine
{
    /// <summary>
    /// Make your object followable by adding an <see cref="IAmMovable"/> property.
    /// </summary>
    public interface IAmFollowable
    {
        public IAmMovable Followable { get; set; }
    }
}
