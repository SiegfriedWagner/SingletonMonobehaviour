using SiegfriedWagner.Singletons.Attributes;
using UnityEngine;

namespace SiegfriedWagner.Singletons.Tests.Runtime
{
	[InstantiatedFromPrefab("TestPrefab")]
	public class PrefabFactorisedSingleton : SingletonMonoBehaviour<PrefabFactorisedSingleton>
	{
		[field: SerializeField]
		public int SerializedField { get; private set; }
	}
}