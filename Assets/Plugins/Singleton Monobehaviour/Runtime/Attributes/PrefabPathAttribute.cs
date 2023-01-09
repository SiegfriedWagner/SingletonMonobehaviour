using System;

namespace SiegfriedWagner.Singletons.Attributes
{
	[AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = false)]
	public class PrefabPathAttribute : Attribute
	{
		public string PathRelativeToResources { get; }
		public PrefabPathAttribute(string pathRelativeToResources)
		{
			PathRelativeToResources = pathRelativeToResources;
		}
	}
}