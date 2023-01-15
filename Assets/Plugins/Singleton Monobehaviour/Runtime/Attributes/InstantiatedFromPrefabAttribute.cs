using System;

namespace SiegfriedWagner.Singletons.Attributes
{
	/// <summary>
	///     Configures prefab that will be used to lazy instantiate new prefab instance when needed.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class InstantiatedFromPrefabAttribute : LazyInstantiatedAttribute
	{
		/// <summary>
		///     Craetes attribute.
		/// </summary>
		/// <param name="pathRelativeToResources">Path relative to Resources directory containing prefab instance.</param>
		public InstantiatedFromPrefabAttribute(string pathRelativeToResources)
		{
			PathRelativeToResources = pathRelativeToResources;
		}

		/// <summary>
		///     Path relative to Resources directory containing prefab instance.
		/// </summary>
		public string PathRelativeToResources { get; }
	}
}