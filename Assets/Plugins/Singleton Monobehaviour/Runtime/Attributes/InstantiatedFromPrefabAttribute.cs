using System;

namespace SiegfriedWagner.Singletons.Attributes
{
	[AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = false)]
	public class InstantiatedFromPrefabAttribute : LazyInstantiatedAttribute
	{
		public string PathRelativeToResources { get; }
		public InstantiatedFromPrefabAttribute(string pathRelativeToResources)
		{
			PathRelativeToResources = pathRelativeToResources;
		}
	}
}