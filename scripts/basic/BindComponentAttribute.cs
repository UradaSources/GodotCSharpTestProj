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

	[System.AttributeUsage(System.AttributeTargets.Method)]
	public class BindEventAttribute : System.Attribute
	{
		public readonly string eventName;

		public BindEventAttribute()
		{
			this.require = require;
		}
	}
}