using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dva
{
    // Owns spawning/renewing regular and special particles. Particle counts can run into the
    // hundreds at high levels, so instances are pooled per-prefab and reused instead of destroyed.
    public class ParticleSpawner
    {
        private readonly GameControl _gameControl;
        private readonly FeaturesManager _featuresManager;
        private readonly Atom _atom;
        private readonly Transform _particleParent;

        private readonly GeneralParticle _neutron;
        private readonly GeneralParticle _electron;
        private readonly GeneralParticle _proton;
        private readonly SpecialParticle _blackHole;
        private readonly SpecialParticle _timeFastParticle;
        private readonly SpecialParticle _timeSlowParticle;
        private readonly SpecialParticle _fieldBiggerParticle;
        private readonly SpecialParticle _fieldSmallerParticle;
        private readonly SpecialParticle _fastNeutronParticle;
        private readonly SpecialParticle _lives;

        private readonly int _maxSpecialAmount;
        private readonly int _particleRenewTime;
        private readonly int _maxLivesInField;
        private readonly float _livesTimeAppear;
        private readonly float _spawnExclusionRadius;

        private float _count;
        private float _livesTimeRest;

        public List<GameObject> ParticlesCounter { get; } = new List<GameObject>();
        public List<GameObject> BlackHolesCounter { get; } = new List<GameObject>();
        public List<GameObject> TimeFastCounter { get; } = new List<GameObject>();
        public List<GameObject> TimeSlowCounter { get; } = new List<GameObject>();
        public List<GameObject> FieldBiggerCounter { get; } = new List<GameObject>();
        public List<GameObject> FieldSmallerCounter { get; } = new List<GameObject>();
        public List<GameObject> NeutronFastCounter { get; } = new List<GameObject>();
        public List<GameObject> LivesCounter { get; } = new List<GameObject>();

        //particles are Instantiate/Destroy-churned constantly (pickups, periodic renewal, field-resize
        //events) and their count can run into the hundreds at high levels, so reuse instances per-prefab
        //instead of destroying them
        private readonly Dictionary<GameObject, Queue<GameObject>> _particlePools = new Dictionary<GameObject, Queue<GameObject>>();

        public ParticleSpawner(GameControl gameControl, FeaturesManager featuresManager, Atom atom, Transform particleParent,
            GeneralParticle neutron, GeneralParticle electron, GeneralParticle proton,
            SpecialParticle blackHole, SpecialParticle timeFastParticle, SpecialParticle timeSlowParticle,
            SpecialParticle fieldBiggerParticle, SpecialParticle fieldSmallerParticle,
            SpecialParticle fastNeutronParticle, SpecialParticle lives,
            int maxSpecialAmount, int particleRenewTime, int maxLivesInField, float livesTimeAppear,
            float spawnExclusionRadius)
        {
            _gameControl = gameControl;
            _featuresManager = featuresManager;
            _atom = atom;
            _particleParent = particleParent;
            _neutron = neutron;
            _electron = electron;
            _proton = proton;
            _blackHole = blackHole;
            _timeFastParticle = timeFastParticle;
            _timeSlowParticle = timeSlowParticle;
            _fieldBiggerParticle = fieldBiggerParticle;
            _fieldSmallerParticle = fieldSmallerParticle;
            _fastNeutronParticle = fastNeutronParticle;
            _lives = lives;
            _maxSpecialAmount = maxSpecialAmount;
            _particleRenewTime = particleRenewTime;
            _maxLivesInField = maxLivesInField;
            _livesTimeAppear = livesTimeAppear;
            _spawnExclusionRadius = spawnExclusionRadius;
            _count = particleRenewTime;
        }

        //spawn particle on random places on field
        public void ParticleSpawn()
        {
            if (ParticlesCounter.Count >= _gameControl.MaxParticleAmount) return;

            InstantiateParticle(GetRandomParticle().gameObject, ParticlesCounter);
        }

        //special particle spawn after reaching certain level of atom
        public void SpecialParticleSpawn()
        {
            AtomId.Decode(_atom.AtomID, out _, out _, out int level);

            if(level >= 20)
            {
                if (NeutronFastCounter.Count < _maxLivesInField)
                {
                    InstantiateParticle(_fastNeutronParticle.gameObject, NeutronFastCounter);
                }

                if (LivesCounter.Count < _maxLivesInField)
                {
                    if (_livesTimeRest <= 0)
                    {
                        InstantiateParticle(_lives.gameObject, LivesCounter);
                    }
                }

                if (level >= 40)
                {
                    if (TimeFastCounter.Count < _maxSpecialAmount)
                    {
                        InstantiateParticle(_timeFastParticle.gameObject, TimeFastCounter);
                    }
                    if (TimeSlowCounter.Count < _maxSpecialAmount)
                    {
                        InstantiateParticle(_timeSlowParticle.gameObject, TimeSlowCounter);
                    }

                    if (level >= 60)
                    {
                        if (FieldBiggerCounter.Count < _maxSpecialAmount)
                        {
                            InstantiateParticle(_fieldBiggerParticle.gameObject, FieldBiggerCounter);
                        }
                        if (FieldSmallerCounter.Count < _maxSpecialAmount)
                        {
                            InstantiateParticle(_fieldSmallerParticle.gameObject, FieldSmallerCounter);
                        }

                        if(level >= 80)
                        {
                            if (BlackHolesCounter.Count < _maxSpecialAmount)
                            {
                                InstantiateParticle(_blackHole.gameObject, BlackHolesCounter);
                            }
                        }
                    }
                }
            }
        }

        //create particle (pulled from the pool for its prefab when possible, instantiated on a pool miss)
        private void InstantiateParticle(GameObject particleType, List<GameObject> particleList)
        {
            Vector3 position = _gameControl.GetRandomPosition(_featuresManager.LeftBoarder.position.x, _featuresManager.RightBoarder.position.x,
                _featuresManager.TopBoarder.position.y, _featuresManager.BottomBoarder.position.y,
                _atom.transform.position, _spawnExclusionRadius);
            GameObject particle = SpawnFromPool(particleType, position);
            particleList.Add(particle);
        }

        private GameObject GetGeneralPrefab(GeneralParticleType type)
        {
            switch (type)
            {
                case GeneralParticleType.Electron: return _electron.gameObject;
                case GeneralParticleType.Proton: return _proton.gameObject;
                default: return _neutron.gameObject;
            }
        }

        private GameObject GetSpecialPrefab(SpecialParticleType type)
        {
            switch (type)
            {
                case SpecialParticleType.BlackHole: return _blackHole.gameObject;
                case SpecialParticleType.TimeFast: return _timeFastParticle.gameObject;
                case SpecialParticleType.TimeSlow: return _timeSlowParticle.gameObject;
                case SpecialParticleType.FieldRise: return _fieldBiggerParticle.gameObject;
                case SpecialParticleType.FiledShrink: return _fieldSmallerParticle.gameObject;
                case SpecialParticleType.FastNeutron: return _fastNeutronParticle.gameObject;
                default: return _lives.gameObject;
            }
        }

        public void ReturnGeneralParticle(GeneralParticleType type, GameObject instance)
        {
            ReturnToPool(GetGeneralPrefab(type), instance);
        }

        public void ReturnSpecialParticle(SpecialParticleType type, GameObject instance)
        {
            ReturnToPool(GetSpecialPrefab(type), instance);
        }

        private void ReturnToPool(GameObject prefab, GameObject instance)
        {
            instance.SetActive(false);

            if (!_particlePools.TryGetValue(prefab, out Queue<GameObject> pool))
            {
                pool = new Queue<GameObject>();
                _particlePools[prefab] = pool;
            }
            pool.Enqueue(instance);
        }

        private GameObject SpawnFromPool(GameObject prefab, Vector3 position)
        {
            GameObject instance;
            if (_particlePools.TryGetValue(prefab, out Queue<GameObject> pool) && pool.Count > 0)
            {
                instance = pool.Dequeue();
                instance.transform.SetPositionAndRotation(position, prefab.transform.rotation);
            }
            else
            {
                instance = UnityEngine.Object.Instantiate(prefab, position, prefab.transform.rotation, _particleParent);
            }

            instance.SetActive(true);
            ResetForSpawn(instance);
            return instance;
        }

        //pooled particles can carry a disabled collider (from being picked up) and leftover animator
        //state (mid/end of the removal animation) from their previous life - reset both before reuse
        private void ResetForSpawn(GameObject instance)
        {
            if (instance.TryGetComponent(out Collider2D particleCollider))
            {
                particleCollider.enabled = true;
            }

            if (instance.TryGetComponent(out Animator animator))
            {
                animator.Rebind();
                animator.Update(0f);
            }

            if (instance.TryGetComponent(out GeneralParticle generalParticle))
            {
                generalParticle._toPatrol = true;
                generalParticle._toBlackHole = false;
            }
        }

        private static readonly GeneralParticleType[] s_generalParticleTypes = (GeneralParticleType[])Enum.GetValues(typeof(GeneralParticleType));

        private GeneralParticle GetRandomParticle()
        {
            GeneralParticleType randomParticle = s_generalParticleTypes[UnityEngine.Random.Range(0, s_generalParticleTypes.Length)];

            if (randomParticle == GeneralParticleType.Electron)
            {
                return _electron;
            }
            else if (randomParticle == GeneralParticleType.Proton)
            {
                return _proton;
            }
            else
            {
                return _neutron;
            }
        }

        //renew random particle (returned to the pool, not destroyed)
        public void ParticleRenew()
        {
            _count -= Time.deltaTime;

            if(_count <= 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, _gameControl.MaxParticleAmount - 1);
                GameObject particleToRenew = ParticlesCounter[randomIndex];
                ParticlesCounter.RemoveAt(randomIndex);
                if (particleToRenew.TryGetComponent(out GeneralParticle generalParticle))
                {
                    ReturnGeneralParticle(generalParticle.GeneralType, particleToRenew);
                }
                _count = _particleRenewTime;
            }
        }

        //renew all particles after event start/end (returned to the pool, not destroyed)
        public void RemoveForEvent(bool isSpecials, List<GameObject> list)
        {
            if (!isSpecials)
            {
                foreach (GeneralParticle particle in UnityEngine.Object.FindObjectsOfType<GeneralParticle>())
                {
                    list.Remove(particle.gameObject);
                    ReturnGeneralParticle(particle.GeneralType, particle.gameObject);
                }
            }
            else
            {
                foreach (SpecialParticle special in UnityEngine.Object.FindObjectsOfType<SpecialParticle>())
                {
                    list.Remove(special.gameObject);
                    ReturnSpecialParticle(special.SpecialType, special.gameObject);
                }
            }
        }

        //called when a lives pickup is collected, so another one doesn't spawn immediately
        public void ResetLivesTimer()
        {
            _livesTimeRest = _livesTimeAppear;
        }
    }
}
