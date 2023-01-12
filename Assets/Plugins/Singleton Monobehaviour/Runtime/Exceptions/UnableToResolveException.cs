using System;

namespace SiegfriedWagner.Singletons.Exceptions
{
	public sealed class UnableToResolveException : Exception
	{
		public UnableToResolveException(string message) : base(message) { }
	}
}