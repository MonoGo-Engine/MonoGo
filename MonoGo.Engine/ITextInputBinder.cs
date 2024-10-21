namespace MonoGo.Engine
{
	/// <summary>
	/// Binds Input.TextInput to the platform-specific text input provider.
	/// </summary>
	internal interface ITextInputBinder
	{
		void Init();
	}
}
