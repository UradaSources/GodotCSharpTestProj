using System.Reflection;
using System.Diagnostics;

namespace urd
{
	[System.AttributeUsage(System.AttributeTargets.Method)]
	public class BindEventAttribute : System.Attribute
	{
		public readonly EventInfo eventInfo;

		public BindEventAttribute(object target, string eventName)
		{
			var type = target.GetType();
			eventInfo = type.GetEvent(eventName);
			Debug.Assert(eventInfo != null);
		}
	}
}