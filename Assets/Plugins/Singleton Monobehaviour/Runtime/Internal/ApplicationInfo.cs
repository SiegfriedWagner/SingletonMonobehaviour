using UnityEngine;

namespace SiegfriedWagner.Singletons.Internal
{
	/// <summary>
	/// Stores information about application state 
	/// </summary>
	internal static class ApplicationInfo // TODO: Remove this from singletons ASAP
	{
		/// <summary>
		/// Equivalent of static constructor that is ran at deterministic moment
		/// </summary>
		[RuntimeInitializeOnLoadMethod]
		private static void RunOnStart()
		{
			Application.quitting += () => IsQuiting = true;
		}

		public static bool IsQuiting { get; private set; } = false;
	}
}