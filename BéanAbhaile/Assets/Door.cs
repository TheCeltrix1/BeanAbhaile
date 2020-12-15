using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    private Vector3 _initialPosition;

    public float offset;
    public Direction direction;
    public float speed;

    private Vector3 _target;
    private bool _used;
    private bool _open;

    public override void OnEnable()
    {
        base.OnEnable();
        _initialPosition = transform.position;
    }

    public override void OnUse()
    {
        if (_used) return;
        _target = !_open ? GetOffsetPositionRelativeToDirection(direction, offset) : _initialPosition;
        _open = !_open;
        _used = true;
        StartCoroutine(ToggleState());
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
        while (transform.position != _target) {
            yield return null;
            transform.position = Vector3.MoveTowards(transform.position, _target, speed * Time.deltaTime);
        }
        _used = false;
    }
}
