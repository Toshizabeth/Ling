using UnityEngine;
using Zenject;


namespace Ling.Common
{
	public class CommonInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container
				.Bind<Scene.IExSceneManager>()
				.To<Scene.ExSceneManager>()
				.FromComponentInHierarchy()
				.AsSingle();
		}
	}
}