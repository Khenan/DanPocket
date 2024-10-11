using System.Collections.Generic;
using Umeshu.Uf;
using UnityEngine;

namespace Umeshu.USystem.VFX
{
    public class ParticleSystemManager : GameSystem<ParticleSystemManager>
    {
        #region Vars 

        public List<ParticleSystem> particleSystems = new();
        private readonly List<ParticleSystem> instantiatedParticleSystems = new();

        private readonly Dictionary<string, List<ParticleSystem>> particleSystemPools = new();

        private readonly List<string> loggedMissingParticleSystems = new();

        #endregion

        public static ParticleSystem PlayParticleSystem(ParticleSystemSettings _particleSystemSettings) => Instance.ActuallyPlayParticleSystem(_particleSystemSettings);

        private ParticleSystem ActuallyPlayParticleSystem(ParticleSystemSettings _particleSystemSettings)
        {
            
            ParticleSystem _particleSystem = GetParticleSystem(_particleSystemSettings);
            if (_particleSystem == null)
            {
                return null;
            }
            if (_particleSystemSettings.parent != null)
            {
                _particleSystem.transform.SetParent(_particleSystemSettings.parent);
            }
            else
            {
                _particleSystem.transform.SetParent(transform);
            }
            _particleSystemSettings.ApplyMethods(_particleSystem);

            
            //now we do children
            for (int i = 0; i < _particleSystem.transform.childCount; i++)
            {
                if (_particleSystem.transform.GetChild(i).GetComponent<ParticleSystem>() == null) return null;
                _particleSystemSettings.ApplyMethods(_particleSystem.transform.GetChild(i).GetComponent<ParticleSystem>());
            }


            if (!_particleSystemSettings.isImmortal)
            {
                SetStopAction(_particleSystem, _particleSystemSettings);
            }

            _particleSystem.Play();
            if (!_particleSystemSettings.isImmortal)
            {
                instantiatedParticleSystems.Add(_particleSystem);
            }
            return _particleSystem;
        }

        #region Main Methods

        public static void ResetParticleSystems()
        {
            foreach (ParticleSystem _particleSystem in Instance.instantiatedParticleSystems)
            {
                if (_particleSystem != null)
                {
                    _particleSystem.Stop();
                    _particleSystem.Clear();
                }
            }
        }

        ParticleSystem GetParticleSystem(ParticleSystemSettings _particleSystemSettings)
        {
            if (TryFindParticleSystem(_particleSystemSettings.ParticleSystemName, out ParticleSystem _particleSystemPrefab))
            {
                ParticleSystem _particleSystem = null;
                List<ParticleSystem> _existingPool = null;
                if (particleSystemPools.ContainsKey(_particleSystemSettings.ParticleSystemName) && _particleSystemSettings.usePool)
                {
                    if (particleSystemPools.TryGetValue(_particleSystemSettings.ParticleSystemName, out List<ParticleSystem> _instantiatedParticleSystems))
                    {
                        _existingPool = _instantiatedParticleSystems;
                        foreach (ParticleSystem _existingParticleSystem in _instantiatedParticleSystems)
                        {
                            if (_existingParticleSystem != null && !_existingParticleSystem.gameObject.activeInHierarchy)
                            {
                                _particleSystem = _existingParticleSystem;
                                ResetFromPool(_particleSystem, _particleSystemSettings);
                                break;
                            }
                        }
                    }
                }
                if (_particleSystem == null)
                {
                    _particleSystem = Instantiate(_particleSystemPrefab);
                    if (_particleSystemSettings.usePool)
                    {
                        bool _willAddList = _existingPool == null;
                        if (_willAddList) _existingPool = new List<ParticleSystem>();
                        _existingPool.Add(_particleSystem);
                        _particleSystem.name = _particleSystem.name + "_" + _existingPool.Count;
                        if (_willAddList) particleSystemPools.Add(_particleSystemSettings.ParticleSystemName, _existingPool);
                    }
                }
                return _particleSystem;
            }
            return null;
        }


        public bool TryFindParticleSystem(string _particleSystemName, out ParticleSystem _particleSystem)
        {
            foreach (ParticleSystem _particleSystemPrefab in particleSystems)
            {
                if (_particleSystemPrefab.name == _particleSystemName)
                {
                    _particleSystem = _particleSystemPrefab;
                    return true;
                }
            }
            _particleSystem = null;

            if (!loggedMissingParticleSystems.Contains(_particleSystemName))
            {
                loggedMissingParticleSystems.Add(_particleSystemName);
                ("Did not found fx named : " + _particleSystemName).Log();
            }
            return false;
        }

        #endregion

        #region Reset From Pool
        private void ResetFromPool(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings) => UfObject.DoEffectOnComponentAndChildsWithParameter(_particleSystem, _particleSystemSettings, ResetFromPool_Method);

        private void ResetFromPool_Method(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings)
        {
            _particleSystem.Stop();
            _particleSystem.gameObject.SetActive(true);
        }
        #endregion

        #region Set Stop Action
        private void SetStopAction(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings) => UfObject.DoEffectOnComponentAndChildsWithParameter(_particleSystem, _particleSystemSettings, SetStopAction_Method);
        private void SetStopAction_Method(ParticleSystem _particleSystem, ParticleSystemSettings _particleSystemSettings)
        {
            ParticleSystem.MainModule _main = _particleSystem.main;
            _main.stopAction = _particleSystemSettings.usePool ? ParticleSystemStopAction.Disable : ParticleSystemStopAction.Destroy;
        }
        #endregion

        protected override void SystemFirstInitialize() { }

        protected override void SystemEnableAndReset() { }

        protected override void SystemPlay() { }

        protected override void SystemUpdate() { }

        public override void OnEnterGameMode(GameModeKey _gameMode)
        {
            ResetParticleSystems();
            base.OnEnterGameMode(_gameMode);
        }
    }
}
