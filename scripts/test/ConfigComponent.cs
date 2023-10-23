namespace urd
{
	public interface ConfigComponent
	{
		bool beforeJoinContainer(Component com, ComponentContainer container);
	}
}