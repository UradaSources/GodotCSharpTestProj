namespace urd
{
	[System.AttributeUsage(System.AttributeTargets.Field)]
	public class RequireComponentAttribute : System.Attribute
	{
		public readonly bool necessary;

		public RequireComponentAttribute(bool necessary = true) 
		{
			this.necessary = necessary;
		}
	}
}