using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Banshee : MonoBehaviour
{
    /*
    Banshee must wail every 3-7 seconds;
    -Follow a random path through the house (visiting important sites more often)
    If hears the player heads roughly in their direction, the closer the player the mosre accurate the direction.
    check a scaled sphere around the player and finds a random point in it that's on the navmesh.
    player awareness metre.
    */

    #region Variables
    private float _neededProximity = 1;

    //Objects
    public GameObject player;
    public AudioSource huntWailSound;
    public AudioSource painWailSound;
    private Player _playerController;
    private BoxCollider _playerSightTrigger;
    public GameObject deathLocation;

    private string _state;
    private string _previousState;
    private bool _nearDeathLocation;

    private float _moveSpeed = 2;
    public float _playerAwareness;
    private float _playerAwarenessMax = 75;
    private float _playerAwarenessProximityModifier = 0.25f;

    private Vector3 _destination;
    private Vector3 _previousLocation;
    public Vector3[] _keyPlaces;
    private NavMeshAgent _meshAgent;
    private NavMeshPath _navMeshPath;
    private RaycastHit _hit;

    //Senses
    private float _playerNoise;
    private float _playerDistance;
    private float _sightRange = 10;
    private bool _inSightRange = false;

    //Timers
    private float _randomDestinationTimer;
    private float _huntWailTimer = 0;
    private float _painWailTimer = 5;
    private float _playerAwarenessDecayRate = 2;
    #endregion

    void Start()
    {
        _playerSightTrigger = this.gameObject.AddComponent<BoxCollider>();
        _playerSightTrigger.isTrigger = true;
        _playerSightTrigger.size = new Vector3(3, 3, _sightRange);
        _playerSightTrigger.center = this.transform.forward * (_sightRange / 2);
        _playerController = player.GetComponent<Player>();
        _meshAgent = this.GetComponent<NavMeshAgent>();
        _randomDestinationTimer = Random.Range(20, 30);
        _painWailTimer = Random.Range(4, 7);
        GenerateDestination();
    }

    void Update()
    {
        //Variable Updates
        _playerDistance = Vector3.Distance(this.transform.position, player.transform.position);

        //update awareness
        PlayerAwarenessChange();

        //update meshAgent Movement
        //_meshAgent.CalculatePath(_destination, _navMeshPath);
        //_meshAgent.path =_navMeshPath;
        _meshAgent.SetDestination(_destination);
        _meshAgent.speed = _moveSpeed;
        Debug.Log(_destination);

        PlayerAwarenessChange();

        HuntWail();
        PainWail();
    }

    private void GenerateDestination()
    {
        Vector3 tempVector = _keyPlaces[Random.Range(0, _keyPlaces.Length)];
        if (Vector3.Distance(tempVector, _destination) >= _neededProximity && _previousLocation != tempVector)
        {
            _destination = tempVector;
        }
    }

    private void GeneratePositionNearPlayer()
    {
        Vector3 randomPos = Random.insideUnitSphere * ((_playerAwarenessMax - _playerAwareness) * _playerAwarenessProximityModifier) + player.transform.position;

        NavMeshHit hit;

        NavMesh.SamplePosition(randomPos, out hit, ((_playerAwarenessMax - _playerAwareness) * _playerAwarenessProximityModifier), NavMesh.AllAreas);
        Debug.Log($"Hit pos: {hit.position} {_state}");
        _destination = hit.position;
    }

    #region Senses
    //Sight & LOS
    private float Sight()
    {
        Vector3 playerDirection = (player.transform.position - this.transform.position).normalized;
        Physics.Raycast(this.transform.position, playerDirection, out _hit);
        if (_hit.transform != null && _hit.transform.tag == "Player")
        {
            if (_inSightRange == true)
            {
                Debug.Log("BEEP BEEP YOU SAD FUCK");
                return 50;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return 0;
        }
    }

    //Hearing
    private float NoiseLevels()
    {
        float noise = (_playerController.noise / Vector3.Distance(player.transform.position, this.transform.position)) * Time.deltaTime;
        return noise;
    }

    #endregion

    private void PlayerAwarenessChange()
    {
        float _playerAwarenessMinimum = 0;

        _playerAwarenessMinimum += NoiseLevels();
        _playerAwarenessMinimum += Sight();
        //set value to minimum or reduce it over time.
        _playerAwareness = (_playerAwareness < _playerAwarenessMinimum) ? _playerAwareness = _playerAwarenessMinimum : _playerAwareness - (_playerAwarenessDecayRate * Time.deltaTime);

        PlayerAwareness();
    }

    private void PlayerAwareness()
    {
        if (_playerAwareness > _playerAwarenessMax)
        {
            _playerAwareness = _playerAwarenessMax;
        }
        //adjust player awarenss, needs revision

        //Get awareness levels and behaviours
        if (_playerAwareness <= 10)
        {
            _state = "Roam";
        }
        else if (_playerAwareness <= 25)
        {
            _state = "Aware";
        }
        else if (_playerAwareness <= _playerAwarenessMax)
        {
            _state = "Hunt";
        }

        //universal movement checks
        //if arrived at location
        if (Vector3.Distance(transform.position, _destination) <= _neededProximity)
        {
            _previousLocation = _destination;
            if (_state == "Roam")
            {
                GenerateDestination();
            }
            else if (_state == "Aware")
            {
                GeneratePositionNearPlayer();
            }
        }
        //Debug.Log(_state);

        switch (_state)
        {
            case "Roam":
                _randomDestinationTimer -= Time.deltaTime;
                if (_randomDestinationTimer <= 0)
                {
                    GenerateDestination();
                    _randomDestinationTimer = Random.Range(20, 40);
                }
                break;

            case "Aware":
                break;

            case "Hunt":
                _destination = player.transform.position;
                break;
        }
    }

    Vector3 GetRandomLocation()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        // Pick the first indice of a random triangle in the nav mesh
        int t = Random.Range(0, navMeshData.indices.Length - 3);

        // Select a random point on it
        Vector3 point = Vector3.Lerp(navMeshData.vertices[navMeshData.indices[t]], navMeshData.vertices[navMeshData.indices[t + 1]], Random.value);
        Vector3.Lerp(point, navMeshData.vertices[navMeshData.indices[t + 2]], Random.value);

        return point;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _inSightRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            _inSightRange = false;
        }
    }

    private void HuntWail()
    {
        if (_state == "Hunt")
        {
            if (_previousState != "Hunt")
            {
                //Debug.Log("FUCKING SCREECH");
                _huntWailTimer = Random.Range(3,5);
                huntWailSound.Play();
            }
            _huntWailTimer -= Time.deltaTime;
            if (_huntWailTimer <= 0)
            {
                _huntWailTimer = Random.Range(3, 5);
                huntWailSound.Play();
            }
        }
        _previousState = _state;
    }

    private void PainWail()
    {
        _painWailTimer -= Time.deltaTime;
        if (_painWailTimer <= 0)
        {
            painWailSound.Play();
        }
        if (Vector3.Distance(player.transform.position, deathLocation.transform.position) <= 5 && !_nearDeathLocation)
        {
            //Debug.Log("YOU BITCH");
            _nearDeathLocation = true;
            painWailSound.Play();
        }
        if (Vector3.Distance(player.transform.position, deathLocation.transform.position) >= 6)
        {
            _nearDeathLocation = false;
        }
    }
}
