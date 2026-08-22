using System.Collections;
using System.Collections.Generic;
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

        [FormerlySerializedAs("_livesList")]
        [HideInInspector]
        public List<GameObject> LivesList;

        private ParticleSpawner _particleSpawner;
        private FeaturesManager _featuresManager;
        private Player _player;
        private Atom _atom;
        private bool _needToRemove;
        private bool _isBlackHoleActive = false;
        private float _liveStepCanvas = 0.1f;

        public List<GameObject> ParticlesCounter => _particleSpawner.ParticlesCounter;
        public List<GameObject> BlackHolesCounter => _particleSpawner.BlackHolesCounter;
        public List<GameObject> TimeFastCounter => _particleSpawner.TimeFastCounter;
        public List<GameObject> TimeSlowCounter => _particleSpawner.TimeSlowCounter;
        public List<GameObject> FieldBiggerCounter => _particleSpawner.FieldBiggerCounter;
        public List<GameObject> FieldSmallerCounter => _particleSpawner.FieldSmallerCounter;
        public List<GameObject> NeutronFastCounter => _particleSpawner.NeutronFastCounter;
        public List<GameObject> LivesCounter => _particleSpawner.LivesCounter;

        public List<GameObject> ParticleCounter => ParticlesCounter;

        public bool IsBlackHoleActive => _isBlackHoleActive;

        private void Awake()
        {
            _featuresManager = FindObjectOfType<FeaturesManager>();
            _atom = FindObjectOfType<Atom>();
            _particleSpawner = new ParticleSpawner(this, _featuresManager, _atom, _particleParent,
                _neutron, _electron, _proton, _blackHole, _timeFastParticle, _timeSlowParticle,
                _fieldBiggerParticle, _fieldSmallerParticle, _fastNeutronParticle, _lives,
                _maxSpecialAmount, _particleRenewTime, _maxLivesInField, _livesTimeAppear);
        }
        void Start()
        {
            LivesList = new List<GameObject>();
            _player = FindObjectOfType<Player>();
            LifesCreation(_livesAmount);
        }

        void Update()
        {
            _particleSpawner.ParticleSpawn();
            _particleSpawner.SpecialParticleSpawn();
            _particleSpawner.ParticleRenew();
        }

        public void ReturnGeneralParticle(GeneralParticleType type, GameObject instance)
        {
            _particleSpawner.ReturnGeneralParticle(type, instance);
        }

        public void ReturnSpecialParticle(SpecialParticleType type, GameObject instance)
        {
            _particleSpawner.ReturnSpecialParticle(type, instance);
        }

        public Vector3 GetRandomPosition(float leftBoarder, float rightBoarder, float topBoarder, float bottomBoarder)
        {
            float x = UnityEngine.Random.Range(leftBoarder, rightBoarder);
            float y = UnityEngine.Random.Range(bottomBoarder, topBoarder);
            float z = -1;

            return new Vector3(x, y, z);
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
                _particleSpawner.ResetLivesTimer();
            }
        }

        //renew all particles after event start/end (returned to the pool, not destroyed)
        private void RemoveForEvent(bool isSpecials, List<GameObject> list)
        {
            _particleSpawner.RemoveForEvent(isSpecials, list);
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
