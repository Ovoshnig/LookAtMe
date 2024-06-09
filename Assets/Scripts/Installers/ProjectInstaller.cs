using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private AudioMixerGroup _audioMixerGroup;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<GameState>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<DataSaver>().FromNew().AsSingle().NonLazy();
        Container.Bind<AudioMixerGroup>().FromInstance(_audioMixerGroup).AsSingle().NonLazy();
        BindSettings();
    }

    private void BindSettings()
    {
        Container.BindInterfacesAndSelfTo<LevelSwitch>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LookTuner>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AudioTuner>().FromNew().AsSingle().NonLazy();
    }
}
