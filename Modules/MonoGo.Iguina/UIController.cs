using MonoGo.Engine.EC;
using MonoGo.Engine.SceneSystem;

namespace MonoGo.Iguina
{
    internal class UIController : Entity
    {
        internal UIController(Layer layer) : base(layer) { }

        public override void Update()
        {
            base.Update();

            GUIMgr.Update();
        }

        public override void Draw()
        {
            base.Draw();

            GUIMgr.Draw();
        }
    }
}
