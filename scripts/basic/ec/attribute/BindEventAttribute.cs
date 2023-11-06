using System.Reflection;
using System.Diagnostics;

namespace urd
{
	[System.AttributeUsage(System.AttributeTargets.Method)]
	public class BindEventAttribute : System.Attribute
	{
		public readonly object target;
		public readonly EventInfo eventInfo;

		public BindEventAttribute(object target, string eventName)
		{
			var type = target.GetType();
			
			this.target = target;
			this.eventInfo = type.GetEvent(eventName);

			Debug.Assert(eventInfo != null, $"{type.Name} does not request binding event {eventName}");
		}
	}
}