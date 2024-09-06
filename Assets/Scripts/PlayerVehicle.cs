using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVehicle : MonoBehaviour
{
    [SerializeField] private Transform[] _firePositions = Array.Empty<Transform>();
    [SerializeField] private Transform _laserPosition;
    [SerializeField] private LayerMask _attackMask;
    [SerializeField] private LineRenderer _lineRenderer;

    public IReadOnlyList<Transform> FirePositions => _firePositions;
    public Transform LaserPosition => _laserPosition;
    public int AttackMaskValue => _attackMask.value;
    public LineRenderer LineRenderer => _lineRenderer;
}
