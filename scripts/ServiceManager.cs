using urd;

public class ServiceManager
{
	private static InputService _Input;
	public static InputService Input
	{
		get
		{
			if (_Input == null)
			{
				var name = "InputService";
				_Input = MiscUtils.CreateRootNode<CacheGodotInput>(name);
			}
			
			return _Input;
		}
	}
}