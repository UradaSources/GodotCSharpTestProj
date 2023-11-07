namespace urd
{
	public class ModuleVersion
	{
		public readonly int mid;
		public readonly string name;
		public readonly string version;

		public ModuleVersion(int mid, string name, string version)
		{
			this.mid = mid;
			this.name = name;
			this.version = version;
		}
	}

}
