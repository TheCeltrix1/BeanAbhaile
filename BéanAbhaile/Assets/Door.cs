using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    private Vector3 _initialPosition;

    public float doorSfxVolume = 2f;
    public float offset;
    public Direction direction;
    public float speed;
    public AudioClip[] soundEffects;

    private Vector3 _target;
    private bool _used;
    private bool _open;

    public bool locked = false;
    public Key unlocker;

    public bool rotate;
    public Vector3 rotationTarget;

    private AudioSource _source;
    private Vector3 _rotationTarget;
    private Vector3 _initialEulerAngles;
    private Vector3 _currentEulerAngles;

    public override void OnEnable()
    {
        base.OnEnable();
        _initialPosition = transform.position;
        _initialEulerAngles = transform.localEulerAngles;
        if(!(_source = GetComponent<AudioSource>()))
            _source = GetComponentInChildren<AudioSource>();
    }

    public override void OnUse()
    {
        //if (_used) return;
        if (locked) {
            if (unlocker && unlocker.PickedUp)
                OnDoorUnlocked();
            else
            {
                OnDoorLocked();
                return;
            }
        }
        if (!rotate)
            _target = !_open ? GetOffsetPositionRelativeToDirection(direction, offset) : _initialPosition;
        else
            _rotationTarget = !_open ? (_used ? _currentEulerAngles : transform.localEulerAngles + rotationTarget) : _initialEulerAngles;

        _open = !_open;
        if (!_used && _open)
            _currentEulerAngles = transform.localEulerAngles + rotationTarget;
        _used = true;
        PlaySoundEffect(0);
        StopAllCoroutines();
        StartCoroutine(ToggleState());
    }

    private void OnDoorLocked()
    {
        PlaySoundEffect(1);
    }

    private void OnDoorUnlocked()
    {

    }

    private void OnDoorShut() {
        PlaySoundEffect(2);
    }

    private void OnDoorOpened() {

    }

    private Vector3 GetOffsetPositionRelativeToDirection(Direction direction, float offset) {
        switch (direction) {
            case Direction.Forward:
                return transform.position + transform.forward * offset;
            case Direction.Backwards:
                return transform.position + (-transform.forward) * offset;
            case Direction.Left:
                return transform.position + (-transform.right) * offset;
            case Direction.Right:
                return transform.position + transform.right * offset;
            case Direction.Up:
                return transform.position + transform.up * offset;
            case Direction.Down:
                return transform.position + -(transform.up) * offset;
        }
        return _target;
    }

    private IEnumerator ToggleState() {
        while ((transform.position != _target && !rotate) || (rotate && transform.localEulerAngles != _rotationTarget)) {
            yield return null;
            if (!rotate)
                transform.position = Vector3.MoveTowards(transform.position, _target, speed * Time.deltaTime);
            else
                transform.localEulerAngles = Vector3.MoveTowards(transform.localEulerAngles, _rotationTarget, speed * Time.deltaTime);
        }
      
        _used = false;
        if (!_open)
            OnDoorShut();
        else
            OnDoorOpened();
    }

    private void PlaySoundEffect(int index) {
        if (index < soundEffects.Length) {
            if (_source)
                _source.PlayOneShot(soundEffects[index], doorSfxVolume);
        }
    }
}
