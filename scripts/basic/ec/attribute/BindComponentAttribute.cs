namespace urd
{
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class BindComponentAttribute : System.Attribute
	{
		public readonly bool require;

		public BindComponentAttribute(bool require = true) 
		{
			this.require = require;
		}
	}
}