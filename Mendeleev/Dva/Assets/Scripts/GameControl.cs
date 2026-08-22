using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dva
{
    //main game mechanics
    public class GameControl : MonoBehaviour
    {
        [Header("Balance values")]
        [SerializeField] private Transform _particleParent;
        [FormerlySerializedAs("_maxParticleAmount")]
        [SerializeField] public int MaxParticleAmount = 10;
        [SerializeField] private int _maxSpecialAmount = 2;
        [SerializeField] private int _particleRenewTime = 15;
        [SerializeField] private int _livesAmount = 3;
        [SerializeField] private int _maxLivesInField = 1;
        [SerializeField] private float _livesTimeAppear = 30f;

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
        [SerializeField] private SpecialParticle _lives;
        [SerializeField] private GameObject _livesCanvas;
        [SerializeField] private Transform _livesHolder;
        [SerializeField] private GameObject _endMenu;

        private float _count;
        private float _livesTimeRest = 0f;

        [FormerlySerializedAs("_particlesCounter")]
        [HideInInspector]
        public List<GameObject> ParticlesCounter;
        [FormerlySerializedAs("_blackHolesCounter")]
        [HideInInspector]
        public List<GameObject> BlackHolesCounter;
        [FormerlySerializedAs("_timeFastCounter")]
        [HideInInspector]
        public List<GameObject> TimeFastCounter;
        [FormerlySerializedAs("_timeSlowCounter")]
        [HideInInspector]
        public List<GameObject> TimeSlowCounter;
        [FormerlySerializedAs("_fieldBiggerCounter")]
        [HideInInspector]
        public List<GameObject> FieldBiggerCounter;
        [FormerlySerializedAs("_fieldSmallerCounter")]
        [HideInInspector]
        public List<GameObject> FieldSmallerCounter;
        [FormerlySerializedAs("_neutronFastCounter")]
        [HideInInspector]
        public List<GameObject> NeutronFastCounter;
        [FormerlySerializedAs("_livesCounter")]
        [HideInInspector]
        public List<GameObject> LivesCounter;
        [FormerlySerializedAs("_livesList")]
        [HideInInspector]
        public List<GameObject> LivesList;

        private FeaturesManager _featuresManager;
        private Player _player;
        private Atom _atom;
        private bool _needToRemove;
        private bool _isBlackHoleActive = false;
        private float _liveStepCanvas = 0.1f;

        public List<GameObject> ParticleCounter => ParticlesCounter;

        public bool IsBlackHoleActive => _isBlackHoleActive;

        private void Awake()
        {
            _featuresManager = FindObjectOfType<FeaturesManager>();
            _atom = FindObjectOfType<Atom>();
        }
        void Start()
        {
            ParticlesCounter = new List<GameObject>();
            BlackHolesCounter = new List<GameObject>();
            TimeFastCounter = new List<GameObject>();
            TimeSlowCounter = new List<GameObject>();
            FieldBiggerCounter = new List<GameObject>();
            FieldSmallerCounter = new List<GameObject>();
            NeutronFastCounter = new List<GameObject>();
            LivesList = new List<GameObject>();
            _player = FindObjectOfType<Player>();
            _count = _particleRenewTime;
            LifesCreation(_livesAmount);
        }

        void Update()
        {
            ParticleSpawn();
            SpecialParticleSpawn();
            ParticleRenew();
            _livesTimeRest -= Time.deltaTime;
        }

        //spawn particle on random places on field
        private void ParticleSpawn()
        {
            if (ParticlesCounter.Count >= MaxParticleAmount) return;

            InstantiateParticle(GetRandomParticle().gameObject, ParticlesCounter);
        }

        //special particle spawn after reaching certain level of atom
        private void SpecialParticleSpawn()
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

        //create particle
        private void InstantiateParticle(GameObject particleType, List<GameObject> particleList)
        {
            GameObject particle = Instantiate(particleType, GetRandomPosition(_featuresManager.LeftBoarder.position.x, _featuresManager.RightBoarder.position.x,
                _featuresManager.TopBoarder.position.y, _featuresManager.BottomBoarder.position.y), particleType.transform.rotation, _particleParent);
            particleList.Add(particle);
        }


        public Vector3 GetRandomPosition(float leftBoarder, float rightBoarder, float topBoarder, float bottomBoarder)
        {
            float x = UnityEngine.Random.Range(leftBoarder, rightBoarder);
            float y = UnityEngine.Random.Range(bottomBoarder, topBoarder);
            float z = -1;

            return new Vector3(x, y, z);
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

        //destroy random particle
        private void ParticleRenew()
        {
            _count -= Time.deltaTime;

            if(_count <= 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, MaxParticleAmount - 1);
                GameObject particleToDestroy = ParticleCounter[randomIndex];
                ParticleCounter.RemoveAt(randomIndex);
                Destroy(particleToDestroy);
                _count = _particleRenewTime;
            }
        }

        //special particle eventcall
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
            else if (particle == SpecialParticleType.Lives)
            {
                LivesEvent();
            }

        }

        //creatinf a zone around the atom where all particles draging into atom
        private IEnumerator BlackHoleEvent()
        {
          
            for (int i = _featuresManager.EventCountDown*10; i > 0;)
            {
                _isBlackHoleActive = true;

                Collider2D[] colliders = Physics2D.OverlapCircleAll(_player.transform.position, _featuresManager.FOVRange);

                if (colliders.Length > 0)
                {
                    foreach (Collider2D collider in colliders)
                    {
                        if (collider.gameObject.TryGetComponent(out GeneralParticle particle))
                        {
                            particle._toPatrol = false;
                            particle._toBlackHole = true;
                        }
                    }
                }
                yield return new WaitForSeconds(1f);
                i--;
            }

            _atom.EventAtomUpdate(true);
            _isBlackHoleActive = false;
        }

        //increase/decrease speed of particles for certain amount of time
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

        //increase/decrease the size of the field for certain amount of time
        private IEnumerator FieldSizeChangeEvent(SpecialParticleType particle)
        {
            //need to destroy the old particles and respawn them, both now and again once the event ends
            float sizeMultiplier;
            float left = _featuresManager.LeftBoarder.transform.position.x;
            float right = _featuresManager.RightBoarder.transform.position.x;
            float top = _featuresManager.TopBoarder.transform.position.y;
            float bottom = _featuresManager.BottomBoarder.transform.position.y;
            Animator _fieldAnimator = _featuresManager.Field.GetComponent<Animator>();

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
                    _featuresManager.ScaleBorders(left, right, top, bottom, sizeMultiplier);

                    RemoveForEvent(false, ParticlesCounter);
                    RemoveForEvent(true, TimeFastCounter);
                    RemoveForEvent(true, TimeSlowCounter);
                    RemoveForEvent(true, BlackHolesCounter);
                    RemoveForEvent(true, FieldBiggerCounter);
                    RemoveForEvent(true, FieldSmallerCounter);
                    RemoveForEvent(true, NeutronFastCounter);

                    _needToRemove = false;
                }

                yield return new WaitForSeconds(1f);
                i--;
            }


            _featuresManager.ScaleBorders(left, right, top, bottom, 1f / sizeMultiplier);

            if (particle == SpecialParticleType.FieldRise)
            {

                _player.gameObject.transform.position = new Vector3(_player.gameObject.transform.position.x / sizeMultiplier, _player.gameObject.transform.position.y, _player.gameObject.transform.position.z / sizeMultiplier);
                _fieldAnimator.SetBool("Bigger", false);
                _needToRemove = true;

                if (_needToRemove)
                {
                    RemoveForEvent(false, ParticlesCounter);
                    RemoveForEvent(true, TimeFastCounter);
                    RemoveForEvent(true, TimeSlowCounter);
                    RemoveForEvent(true, BlackHolesCounter);
                    RemoveForEvent(true, FieldBiggerCounter);
                    RemoveForEvent(true, FieldSmallerCounter);
                    RemoveForEvent(true, NeutronFastCounter);

                    _needToRemove = false;
                }
            }
            else
            {
                _fieldAnimator.SetBool("Smaller", false);
            }

        }

        //hit the atom by the fast neutron (just making the atom to decay)
        private void FastNeutronEvent()
        {
            _atom.EventAtomUpdate(false);
        }

        private void LivesEvent()
        {

            if (LivesList.Count < 5)
            {
                Transform lastLifePosition = LivesList[LivesList.Count - 1].gameObject.transform;
                GameObject life = Instantiate(_livesCanvas, new Vector3(lastLifePosition.position.x + _liveStepCanvas, _livesHolder.position.y, _livesHolder.position.z)
                        , _livesCanvas.transform.rotation, _livesHolder);

                LivesList.Add(life);
                _livesTimeRest = _livesTimeAppear;
            }
        }

        //renew all particles after event start/end
        private void RemoveForEvent(bool isSpecials, List<GameObject> list)
        {
            if (!isSpecials)
            {
                List<GameObject> particles = FindObjectsOfType<GeneralParticle>().Select(stat => stat.gameObject).ToList();
                foreach (GameObject particle in particles)
                {
                    list.Remove(particle.gameObject);
                    Destroy(particle.gameObject);
                }
            }
            else
            {
                List<GameObject> specials = FindObjectsOfType<SpecialParticle>().Select(stat => stat.gameObject).ToList();
                foreach (GameObject special in specials)
                {
                    list.Remove(special.gameObject);
                    Destroy(special.gameObject);
                }
            }
        }

        //add lives to the field
        private void LifesCreation(int lifeAmount)
        {
            for(int i=0; i< lifeAmount; i++)
            {
                GameObject life = Instantiate(_livesCanvas, new Vector3(_livesHolder.position.x + i * _liveStepCanvas, _livesHolder.position.y, _livesHolder.position.z)
                    , _livesCanvas.transform.rotation, _livesHolder);

                LivesList.Add(life);
            }
        }

        //ending game if amount of lives = 0
        public void EndGame()
        {
            Time.timeScale = 0f;
            _endMenu.SetActive(true);
            int maxScore = PlayerPrefs.GetInt("Statistic");
            if (_atom.StatisticScore > maxScore)
            {
                PlayerPrefs.SetInt("Statistic", _atom.StatisticScore);
                _endMenu.transform.GetChild(0).GetComponent<TMP_Text>().text = "New Score: " + _atom.StatisticScore;
            }
            else
            {
                _endMenu.transform.GetChild(0).GetComponent<TMP_Text>().text = "Score: " + _atom.StatisticScore;
            }
        }
    }
}