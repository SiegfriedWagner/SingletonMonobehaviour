using System;
using System.Reflection;
using NUnit.Framework;
using SiegfriedWagner.Singletons.Attributes;

namespace SiegfriedWagner.Singletons.Tests.Runtime
{
	public class TestPrefabFactory
	{
		[Test]
		public void TestCreateFromPrefab()
		{
			var instance = PrefabFactorisedSingleton.GetInstance();
			Assert.AreEqual(100, instance.SerializedField);
		}
	}
}