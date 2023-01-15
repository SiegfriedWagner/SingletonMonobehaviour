using System;

namespace SiegfriedWagner.Singletons.Exceptions
{
	/// <summary>
	///     Exception thrown when factory method is unable to create instance of singleton.
	/// </summary>
	public sealed class UnableToResolveException : Exception
	{
		internal UnableToResolveException(string message) : base(message)
		{
		}
	}
}