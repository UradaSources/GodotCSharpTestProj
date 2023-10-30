using System.Reflection;
using System.Diagnostics;

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
		public readonly object target;
		public readonly EventInfo eventInfo;

		public BindEventAttribute(object target, string eventName)
		{
			this.target = target;
			this.eventInfo = target.GetType().GetEvent(eventName);

			Debug.Assert(this.eventInfo != null, 
				$"non-existent event {eventName} from {target}");
		}
	}
}