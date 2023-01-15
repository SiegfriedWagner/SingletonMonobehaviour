using System;

namespace SiegfriedWagner.Singletons.Attributes
{
	/// <summary>
	/// Allows to create new instance of prefab on demand.
	/// </summary>
	[AttributeUsage(validOn: AttributeTargets.Class, Inherited = false)]
	public class LazyInstantiatedAttribute : Attribute
	{
		
	}
}