using System;

namespace SiegfriedWagner.Singletons.Attributes
{
	[AttributeUsage(validOn: AttributeTargets.Class, Inherited = false)]
	public class LazyInstantiatedAttribute : Attribute
	{
		
	}
}