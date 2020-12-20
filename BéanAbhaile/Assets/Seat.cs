using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : Interactable
{
    public float sitSpeed = 200f;
    private Quaternion _lastRotation;
    private Vector3 _lastPosition;
    public float heightOffset;
    public Direction direction = Direction.Down;

    public override void OnUse()
    {
        if (!Player.instance || Player.instance.SittingTransition) return;
        if (Player.instance.CurrentSeat && Player.instance.CurrentSeat != this) return;

        if (!Player.instance.Sitting)
        {
            _lastRotation = Player.instance.cameraRef.rotation;
            _lastPosition = Player.instance.transform.position;
            Player.instance.Sit(transform.position + (Vector3.up * heightOffset), Quaternion.LookRotation(GetDirection()), sitSpeed, this);
        }
        else
            Player.instance.Sit(_lastPosition, _lastRotation, sitSpeed, null);
    }

    private void Update()
    {
        if (!Player.instance) return;
        selectionColorChangeEnabled = !Player.instance.Sitting || Player.instance.CurrentSeat == this;
    }

    private Vector3 GetDirection()
    {
        switch (direction) {
            case Direction.Forward:
                return transform.forward;
            case Direction.Backwards:
                return -transform.forward;
            case Direction.Up:
                return transform.up;
            case Direction.Down:
                return -transform.up;
            case Direction.Left:
                return -transform.right;
            case Direction.Right:
                return transform.right;
            default:
                return Vector3.zero;
        }
    }
}
