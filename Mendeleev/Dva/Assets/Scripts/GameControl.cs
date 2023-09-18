using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dva
{
    public class GameControl : MonoBehaviour
    {
        [Header("Balance values")]
        [SerializeField] private Transform _particleParent;
        [SerializeField] private int _maxParticleAmount = 10;
        [SerializeField] private int _maxSpecialAmount = 2;
        [SerializeField] private int _particleRenewTime = 15;
        [SerializeField] private float _particleSpeed = 0.01f;

        [Header("Particles")]
        [SerializeField] private GeneralParticle _neutron;
        [SerializeField] private GeneralParticle _electron;
        [SerializeField] private GeneralParticle _proton;
        [SerializeField] private SpecialParticle _blackHole;
        [SerializeField] private SpecialParticle _timeFastParticle;
        [SerializeField] private SpecialParticle _timeSlowParticle;
        [SerializeField] private SpecialParticle _fieldBiggerParticle;
        [SerializeField] private SpecialParticle _fieldSmallerParticle;
        [SerializeField] private SpecialParticle _fastNeutronParticle;

        private float _count;

        public List<GameObject> _particlesCounter;
        public List<GameObject> _blackHolesCounter;
        public List<GameObject> _timeFastCounter;
        public List<GameObject> _timeSlowCounter;
        public List<GameObject> _fieldBiggerCounter;
        public List<GameObject> _fieldSmallerCounter;
        public List<GameObject> _neutronFastCounter;
        private FeaturesManager _featuresManager;
        private Player _player;
        private Atom _atom;
        private bool _needToRemove;
        private bool _isBlackHoleActive = false;

        public List<GameObject> ParticleCounter => _particlesCounter;
        public float ParticleSpeed => _particleSpeed;

        public bool IsBlackHoleActive => _isBlackHoleActive;

        private void Awake()
        {
            _featuresManager = FindObjectOfType<FeaturesManager>();
            _atom = FindObjectOfType<Atom>();
        }
        void Start()
        {
            _particlesCounter = new List<GameObject>();
            _blackHolesCounter = new List<GameObject>();
            _timeFastCounter = new List<GameObject>();
            _timeSlowCounter = new List<GameObject>();
            _fieldBiggerCounter = new List<GameObject>();
            _fieldSmallerCounter = new List<GameObject>();
            _neutronFastCounter = new List<GameObject>();
            _player = FindObjectOfType<Player>();
            _count = _particleRenewTime;
        }

        void Update()
        {
            ParticleSpawn(GetRandomParticle().gameObject);
            SpecialParticleSpawn();
            ParticleRenew();
        }

        private void ParticleSpawn(GameObject particle)
        {
            if (_particlesCounter.Count >= _maxParticleAmount) return;

            InstantiateParticle(particle, _particlesCounter);
        }

        private void SpecialParticleSpawn()
        { 
            int level = (((_atom.AtomID - 1000000000) % 1000000) % 1000);

            if(level >= 20)
            {
                if (_neutronFastCounter.Count < _maxSpecialAmount)
                {
                    InstantiateParticle(_fastNeutronParticle.gameObject, _neutronFastCounter);
                }

                if (level >= 40)
                {
                    if (_timeFastCounter.Count < _maxSpecialAmount)
                    {
                        InstantiateParticle(_timeFastParticle.gameObject, _timeFastCounter);
                    }
                    if (_timeSlowCounter.Count < _maxSpecialAmount)
                    {
                        InstantiateParticle(_timeSlowParticle.gameObject, _timeSlowCounter);
                    }

                    if (level >= 60)
                    {
                        if (_fieldBiggerCounter.Count < _maxSpecialAmount)
                        {
                            InstantiateParticle(_fieldBiggerParticle.gameObject, _fieldBiggerCounter);
                        }
                        if (_fieldSmallerCounter.Count < _maxSpecialAmount)
                        {
                            InstantiateParticle(_fieldSmallerParticle.gameObject, _fieldSmallerCounter);
                        }

                        if(level >= 80)
                        {
                            if (_blackHolesCounter.Count < _maxSpecialAmount)
                            {
                                InstantiateParticle(_blackHole.gameObject, _blackHolesCounter);
                            }
                        }
                    }
                }
            }

        }

        private void InstantiateParticle(GameObject particleType, List<GameObject> particleList)
        {
            GameObject particle = Instantiate(particleType, GetRandomPosition(_featuresManager.LeftBoarder.position.x, _featuresManager.RightBoarder.position.x,
                _featuresManager.TopBoarder.position.z, _featuresManager.BottomBoarder.position.z), transform.rotation, _particleParent);
            particleList.Add(particle);
        }

        public Vector3 GetRandomPosition(float leftBoarder, float rightBoarder, float topBoarder, float bottomBoarder)
        {
            float x = UnityEngine.Random.Range(leftBoarder, rightBoarder);
            float y = 0.2f;
            float z = UnityEngine.Random.Range(bottomBoarder, topBoarder);

            return new Vector3(x, y, z);
        }

        private GeneralParticle GetRandomParticle()
        {
            Array values = Enum.GetValues(typeof(GeneralParticleType));
            System.Random random = new System.Random();
            GeneralParticleType randomParticle = (GeneralParticleType)values.GetValue(random.Next(values.Length));

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

        private void ParticleRenew()
        {
            _count -= Time.deltaTime;

            if(_count <= 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, _maxParticleAmount - 1);
                GameObject particleToDestroy = ParticleCounter[randomIndex];
                ParticleCounter.RemoveAt(randomIndex);
                Destroy(particleToDestroy);
                _count = _particleRenewTime;
            }
        }


        public void EventCall(SpecialParticleType particle)
        {
            if (particle == SpecialParticleType.BlackHole)
            {
                StartCoroutine(BlackHoleEvent());
            }
            else if (particle == SpecialParticleType.TimeFast || particle == SpecialParticleType.TimeSlow)
            {
                StartCoroutine(SpeedChangeEvent(particle));
            }
            else if (particle == SpecialParticleType.FieldRise || particle == SpecialParticleType.FiledShrink)
            {
                StartCoroutine(FieldSizeChangeEvent(particle));
            }
            else if (particle == SpecialParticleType.FastNeutron)
            {
                FastNeutronEvent();
            }
        }

        private IEnumerator BlackHoleEvent()
        {
          
            for (int i = _featuresManager.EventCountDown*10; i > 0;)
            {
                _isBlackHoleActive = true;

                Collider[] colliders = Physics.OverlapSphere(_player.transform.position, _featuresManager.FOVRange);
                
                if (colliders.Length > 0)
                {
                    foreach (Collider collider in colliders)
                    {
                        if (collider.gameObject.GetComponent<GeneralParticle>() != null)
                        {
                            collider.gameObject.GetComponent<GeneralParticle>()._toPatrol = false;
                            collider.gameObject.GetComponent<GeneralParticle>()._toBlackHole = true;

                        }
                    }
                }
                yield return new WaitForSeconds(1f);
                i--;
            }

            _atom.EventAtomUpdate(true);
            _isBlackHoleActive = false;
        }


        private IEnumerator SpeedChangeEvent(SpecialParticleType particle)
        {
            float startSpeed = _featuresManager.ParticleSpeed;
            float multiplier;
            if (particle == SpecialParticleType.TimeSlow)
            {
                multiplier = 0.7f;
            }
            else
            {
                multiplier = 10f;
            }
            for (int i = _featuresManager.EventCountDown; i >= 0;)
            {
                _featuresManager.ParticleSpeed = startSpeed * multiplier;
                yield return new WaitForSeconds(1f);
                i--;
            }
            _featuresManager.ParticleSpeed = _featuresManager.ParticleSpeed / multiplier;
        }

        private IEnumerator FieldSizeChangeEvent(SpecialParticleType particle)
        {
            //нужно разрушить старые частицы и заново сделать и также после завершения
            float sizeMultiplier;
            Vector3 startField = _featuresManager.Field.localScale;
            float left = _featuresManager.LeftBoarder.transform.position.x;
            float right = _featuresManager.RightBoarder.transform.position.x;
            float top = _featuresManager.TopBoarder.transform.position.z;
            float bottom = _featuresManager.BottomBoarder.transform.position.z;
            Animator _fieldAnimator = _featuresManager.Field.GetComponent<Animator>();
            GeneralParticle[] _particlesToRemove = new GeneralParticle[10];
            SpecialParticle[] specialsToRemove = new SpecialParticle[2];

            if (particle == SpecialParticleType.FiledShrink)
            {
                sizeMultiplier = 0.7f;
            }
            else
            {
                sizeMultiplier = 1.5f;
            }

            _needToRemove = true;

            for (int i = _featuresManager.EventCountDown * 3; i > 0;)
            {
                if (_needToRemove)
                {
                    if (particle == SpecialParticleType.FiledShrink)
                    {
                        _player.gameObject.transform.position = new Vector3(_player.gameObject.transform.position.x * sizeMultiplier, _player.gameObject.transform.position.y, _player.gameObject.transform.position.z * sizeMultiplier);
                        _fieldAnimator.SetBool("Smaller", true);
                    }
                    else
                    {
                        _fieldAnimator.SetBool("Bigger", true);
                    }
    
                    //_featuresManager.Field.localScale = new Vector3(startField.x * sizeMultiplier, startField.y, startField.z * sizeMultiplier);
                    _featuresManager.LeftBoarder.transform.position = new Vector3(left * sizeMultiplier, _featuresManager.LeftBoarder.transform.position.y, _featuresManager.LeftBoarder.transform.position.z);
                    _featuresManager.RightBoarder.transform.position = new Vector3(right * sizeMultiplier, _featuresManager.RightBoarder.transform.position.y, _featuresManager.RightBoarder.transform.position.z);
                    _featuresManager.TopBoarder.transform.position = new Vector3(_featuresManager.TopBoarder.transform.position.x, _featuresManager.TopBoarder.transform.position.y, top * sizeMultiplier);
                    _featuresManager.BottomBoarder.transform.position = new Vector3(_featuresManager.BottomBoarder.transform.position.x, _featuresManager.BottomBoarder.transform.position.y, bottom * sizeMultiplier);


                    RemoveForEvent(_particlesToRemove, specialsToRemove, false, _particlesCounter);
                    RemoveForEvent(_particlesToRemove, specialsToRemove, true, _timeFastCounter);
                    RemoveForEvent(_particlesToRemove, specialsToRemove, true, _timeSlowCounter);
                    RemoveForEvent(_particlesToRemove, specialsToRemove, true, _blackHolesCounter);
                    RemoveForEvent(_particlesToRemove, specialsToRemove, true, _fieldBiggerCounter);
                    RemoveForEvent(_particlesToRemove, specialsToRemove, true, _fieldSmallerCounter);
                    RemoveForEvent(_particlesToRemove, specialsToRemove, true, _neutronFastCounter);

                    _needToRemove = false;
                }

                yield return new WaitForSeconds(1f);
                i--;
            }


           // _featuresManager.Field.localScale = startField;
            _featuresManager.LeftBoarder.transform.position = new Vector3(left / sizeMultiplier, _featuresManager.LeftBoarder.transform.position.y, _featuresManager.LeftBoarder.transform.position.z);
            _featuresManager.RightBoarder.transform.position = new Vector3(right / sizeMultiplier, _featuresManager.RightBoarder.transform.position.y, _featuresManager.RightBoarder.transform.position.z);
            _featuresManager.TopBoarder.transform.position = new Vector3(_featuresManager.TopBoarder.transform.position.x, _featuresManager.TopBoarder.transform.position.y, top / sizeMultiplier);
            _featuresManager.BottomBoarder.transform.position = new Vector3(_featuresManager.BottomBoarder.transform.position.x, _featuresManager.BottomBoarder.transform.position.y, bottom / sizeMultiplier);

            if (particle == SpecialParticleType.FieldRise)
            {

                _player.gameObject.transform.position = new Vector3(_player.gameObject.transform.position.x / sizeMultiplier, _player.gameObject.transform.position.y, _player.gameObject.transform.position.z / sizeMultiplier);
                _fieldAnimator.SetBool("Bigger", false);
                _needToRemove = true;

                if (_needToRemove)
                {
                    RemoveForEvent(_particlesToRemove, specialsToRemove, false, _particlesCounter);
                    RemoveForEvent(_particlesToRemove, specialsToRemove, true, _timeFastCounter);
                    RemoveForEvent(_particlesToRemove, specialsToRemove, true, _timeSlowCounter);
                    RemoveForEvent(_particlesToRemove, specialsToRemove, true, _blackHolesCounter);
                    RemoveForEvent(_particlesToRemove, specialsToRemove, true, _fieldBiggerCounter);
                    RemoveForEvent(_particlesToRemove, specialsToRemove, true, _fieldSmallerCounter);
                    RemoveForEvent(_particlesToRemove, specialsToRemove, true, _neutronFastCounter);

                    _needToRemove = false;
                }
            }
            else
            {
                _fieldAnimator.SetBool("Smaller", false);
            }

        }

        private void FastNeutronEvent()
        {
            _atom.EventAtomUpdate(false);
        }

        private void RemoveForEvent(GeneralParticle[] particles, SpecialParticle[] specials, bool isSpecials, List<GameObject> list)
        {
            if (!isSpecials)
            {
                particles = FindObjectsOfType<GeneralParticle>();
                foreach (GeneralParticle particle in particles)
                {
                    list.Remove(particle.gameObject);
                    Destroy(particle.gameObject);
                }
            }
            else
            {
                specials = FindObjectsOfType<SpecialParticle>();
                foreach (SpecialParticle special in specials)
                {
                    list.Remove(special.gameObject);
                    Destroy(special.gameObject);
                }
            }
        }
    }
}