using System;
using UnityEngine;

namespace MadKart
{
    public class KartController : MonoBehaviour
    {
        private void Start()
        {
            _serializedData.RigidBody.velocity = Vector3.right;
        }

        [Serializable]
        private struct SerializedData
        {
            public LayerMask DrivableSurfacesLayer;
            public Rigidbody RigidBody;
            public float RadiusForOverlapSphere;
            public float MaxSpeed;
            public float MaxSpeedGainPerSecond;
            public float SteeringSpeed;

            public Transform CarVisual;
        }

        [SerializeField] private SerializedData _serializedData;

        private readonly Collider[] _results = new Collider[256];
        private Vector3 _lastContactPoint;
        private Vector3 _lastSurfaceNormal;

        private Rigidbody Rigidbody => _serializedData.RigidBody;

        private void FixedUpdate()
        {
            Debug.Log(Rigidbody.velocity.magnitude);

            Vector3 rigidBodyPosition = Rigidbody.position;

            int collidersHitCount = Physics.OverlapSphereNonAlloc(rigidBodyPosition,
                _serializedData.RadiusForOverlapSphere, _results, _serializedData.DrivableSurfacesLayer.value);
            bool isGrounded = collidersHitCount > 0;

            // TODO: what to do when the kart is not grounded?
            // Debug.Log(isGrounded);
            if (!isGrounded) return;

            float nearestDistance = float.MaxValue;

            for (int i = 0; i < collidersHitCount; i++)
            {
                Vector3 colliderClosestPoint = _results[i].ClosestPoint(rigidBodyPosition);
                float distanceToCollider = (rigidBodyPosition - colliderClosestPoint).magnitude;

                if (distanceToCollider < nearestDistance)
                {
                    nearestDistance = distanceToCollider;
                    _lastContactPoint = colliderClosestPoint;
                }
            }

            // we need the surface normal for 2 reasons:
            // 1. When setting the pose of the car model. (Think about the case the car is road is above the car).
            // 2. To get the correct steering direction; which is different depending if the road is below or above the car.
            _lastSurfaceNormal = (rigidBodyPosition - _lastContactPoint).normalized;

            // the tangent on the surface following the rigid body's velocity direction...
            Vector3 surfaceTangent = Vector3.ProjectOnPlane(Rigidbody.velocity.normalized, _lastSurfaceNormal).normalized;

            Rigidbody.velocity += surfaceTangent * Time.fixedDeltaTime * _serializedData.MaxSpeedGainPerSecond;
            Rigidbody.velocity = Mathf.Clamp(Rigidbody.velocity.magnitude, 0, _serializedData.MaxSpeed) * Rigidbody.velocity.normalized;

            float steeringAngle = 0f;
            if (Input.GetKey(KeyCode.A))
            {
                steeringAngle = Time.fixedDeltaTime * -_serializedData.SteeringSpeed;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                steeringAngle = Time.fixedDeltaTime * _serializedData.SteeringSpeed;
            }
            Rigidbody.velocity = Quaternion.AngleAxis(steeringAngle, _lastSurfaceNormal) * Rigidbody.velocity;
        }

        // TODO: check if grounded...
        private void LateUpdate()
        {
            // TODO: smooth raw position and rotation...
            _serializedData.CarVisual.position = _lastContactPoint;
            _serializedData.CarVisual.rotation = Quaternion.LookRotation(Rigidbody.velocity.normalized, _lastSurfaceNormal);
        }
    }
}