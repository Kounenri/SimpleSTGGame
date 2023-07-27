public class BaseTEvent
{
	#region STATES
	public const string READY = @"OnReady";
	public const string LOADED = @"OnLoaded";
	public const string FAILED = @"OnFailed";
	public const string CHANGED = @"OnChanged";
	public const string COMPLETE = @"OnComplete";
	public const string INITIALIZED = @"OnInitialized";
	#endregion

	#region SIMPLE
	public const string NO = @"OnNo";
	public const string YES = @"OnYes";

	public const string OPEN = @"OnOpen";
	public const string CLOSED = @"OnClosed";
	#endregion

	#region MOUSE
	public const string CLICK = @"OnClick";
	public const string DOUBLE_CLICK = @"OnDoubleClick";
	#endregion

	#region ANIMATION
	public const string ANIMATION_STARTED = @"OnAnimationStarted";
	public const string ANIMATION_COMPLETE = @"OnAnimationComplete";
	#endregion
}
