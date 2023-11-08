namespace urd
{
	[System.AttributeUsage(System.AttributeTargets.Class)]
	public class RecordObjectAttribute : System.Attribute
	{
		public readonly System.Type overrideRecordKey;

		public RecordObjectAttribute(System.Type overrideRecordKey)
		{ 
			this.overrideRecordKey = overrideRecordKey;
		}
		public RecordObjectAttribute()
		{
			this.overrideRecordKey = null;
		}
	}
}
