using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MadKart
{
    public class KartController : MonoBehaviour
    {
        #region Private fields & Properties

        [Serializable]
        private struct SerializedData
        {
            [Header("Physics settings")]
            public LayerMask DrivableSurfacesLayer;
            public Rigidbody RigidBody;
            public float RadiusForOverlapSphere;

            // motor...
            public float MinSpeed;
            public float MaxSpeedHorizontalSurface;
            public float MaxSpeedVerticalSurface;
            public float MaxSpeedChangeDampTime;
            public float SpeedGainPerSecond;

            // steering...
            public float MaxSteeringAngle;
            public float SteeringAngleChangeDampTime;

            [Header("Visual settings")]
            public Transform KartVisual;
            public Transform FrontLeftWheelSteering;
            public Transform FrontRightWheelSteering;
            public Transform FrontLeftWheelRotation;
            public Transform FrontRightWheelRotation;
            public Transform BackLeftWheelRotation;
            public Transform BackRightWheelRotation;
            public float FrontWheelsRadius; // world space...
            public float BackWheelsRadius; // world space...
            public float KartVisualRotationChangeDampTime;
            public float KartVisualPositionChangeDampTime;
        }

        [SerializeField] private SerializedData _serializedData;

        private readonly Collider[] _results = new Collider[256];
        private Vector3 _lastKartVisualPosition;
        private Quaternion _lastKartVisualRotation;
        private Vector3 _smoothDampVelocity;
        private Vector3 _lastRigidBodyToContactPoint;
        private float _lastAllowedMaxSpeed;
        private float _steeringAngle;

        private Rigidbody RigidBody => _serializedData.RigidBody;

        #endregion

        #region Unity messages

        private void Start()
        {
            RigidBody.velocity = Vector3.forward * _serializedData.MinSpeed;

            _smoothDampVelocity = Vector3.zero;
            _steeringAngle = 0f;
            _lastAllowedMaxSpeed = _serializedData.MaxSpeedHorizontalSurface;
            _lastRigidBodyToContactPoint = Vector3.up * -0.5f;
            _lastKartVisualRotation = Quaternion.identity;
            _lastKartVisualPosition = RigidBody.position + _lastRigidBodyToContactPoint;
        }

        private void FixedUpdate()
        {
            // make sure that the rigid body velocity is not zero.
            // Debug.Log(RigidBody.velocity.magnitude);
            if (RigidBody.velocity == Vector3.zero)
            {
                RigidBody.velocity = Quaternion.AngleAxis(Random.Range(0, 360f), Vector3.up) * Vector3.forward * _serializedData.MinSpeed;
            }

            Vector3 rigidBodyPosition = RigidBody.position;

            int collidersHitCount = Physics.OverlapSphereNonAlloc(rigidBodyPosition,
                _serializedData.RadiusForOverlapSphere, _results, _serializedData.DrivableSurfacesLayer.value);
            bool isGrounded = collidersHitCount > 0;

            // Debug.Log(_isGrounded);
            if (isGrounded)
            {
                float nearestDistance = float.MaxValue;
                Vector3 contactPoint = default;

                for (int i = 0; i < collidersHitCount; i++)
                {
                    Vector3 colliderClosestPoint = _results[i].ClosestPoint(rigidBodyPosition);
                    float distanceToCollider = (rigidBodyPosition - colliderClosestPoint).magnitude;

                    if (distanceToCollider < nearestDistance)
                    {
                        nearestDistance = distanceToCollider;
                        contactPoint = colliderClosestPoint;
                    }
                }

                _lastRigidBodyToContactPoint = contactPoint - rigidBodyPosition;
                Vector3 surfaceNormal = (-_lastRigidBodyToContactPoint).normalized;

                // depending on the surface normal, we have different max speeds...
                float newAllowedMaxSpeedRatio = Mathf.Abs(Vector3.Cross(surfaceNormal, Vector3.up).magnitude);
                float newAllowedMaxSpeed = Mathf.Lerp(_serializedData.MaxSpeedHorizontalSurface,
                    _serializedData.MaxSpeedVerticalSurface, newAllowedMaxSpeedRatio);
                _lastAllowedMaxSpeed = Mathf.Lerp(_lastAllowedMaxSpeed, newAllowedMaxSpeed,
                    Time.fixedDeltaTime / _serializedData.MaxSpeedChangeDampTime);

                // calculate the tangent on the surface following the rigid body's velocity direction...
                Vector3 surfaceTangent = Vector3.ProjectOnPlane(RigidBody.velocity.normalized, surfaceNormal).normalized;

                // apply some kart motor effect to the rigid body's velocity...
                RigidBody.velocity += surfaceTangent * Time.fixedDeltaTime * _serializedData.SpeedGainPerSecond;
                RigidBody.velocity = Mathf.Clamp(RigidBody.velocity.magnitude, 0, _lastAllowedMaxSpeed) * RigidBody.velocity.normalized;

                // apply some kart steering effect to the rigid body's velocity...
                // from the steering angle, calculate the turning radius...
                // Formula from: https://en.wikipedia.org/wiki/Turning_radius
                float k1 = 1f; // wheelbase.
                float k2 = 0f; // tire width;
                float turningRadius = (k1 / Mathf.Sin(Mathf.Deg2Rad * _steeringAngle)) + (k2 / 2f);
                
                // from the turning radius, calculate the angular velocity and the instant angle change...
                float angularVelocity = RigidBody.velocity.magnitude / turningRadius;
                float rotationAngleDegrees = angularVelocity * Time.fixedDeltaTime * Mathf.Rad2Deg;

                // finally, apply the steering effect to the velocity...
                RigidBody.velocity = Quaternion.AngleAxis(rotationAngleDegrees, surfaceNormal) * RigidBody.velocity;
            }
            else
            {
                // let the rigid body follow a free fall dynamic.
                _lastAllowedMaxSpeed = RigidBody.velocity.magnitude;
            }
        }

        private void Update()
        {
            UpdateKartVisual();
        }

        #endregion

        #region Private implementation

        private void UpdateKartVisual()
        {
            // update the steering angle, according to player input...
            float targetSteeringAngle = 0f;
            if (Input.GetKey(KeyCode.A))
            {
                targetSteeringAngle = -_serializedData.MaxSteeringAngle;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                targetSteeringAngle = _serializedData.MaxSteeringAngle;
            }
            _steeringAngle = Mathf.Lerp(_steeringAngle, targetSteeringAngle, Time.deltaTime / _serializedData.SteeringAngleChangeDampTime);

            // calculate a smooth position for the kart visual...
            Vector3 targetPosition = RigidBody.position + _lastRigidBodyToContactPoint;
            _lastKartVisualPosition = Vector3.SmoothDamp(_lastKartVisualPosition, targetPosition, ref _smoothDampVelocity,
                Time.deltaTime * _serializedData.KartVisualPositionChangeDampTime);
            _serializedData.KartVisual.position = _lastKartVisualPosition;

            // calculate a smooth rotation for the kart visual...
            Vector3 targetUp = (-_lastRigidBodyToContactPoint).normalized;
            Quaternion targetRotation = RigidBody.velocity == Vector3.zero ?
                                            _lastKartVisualRotation :
                                            Quaternion.LookRotation(RigidBody.velocity.normalized, targetUp);
            _lastKartVisualRotation = Quaternion.Slerp(_lastKartVisualRotation, targetRotation,
                Time.deltaTime / _serializedData.KartVisualRotationChangeDampTime);
            _serializedData.KartVisual.rotation = _lastKartVisualRotation;

            // update the steering angle of the two front wheels...
            _serializedData.FrontLeftWheelSteering.localRotation = Quaternion.AngleAxis(_steeringAngle, Vector3.up);
            _serializedData.FrontRightWheelSteering.localRotation = _serializedData.FrontLeftWheelSteering.localRotation;

            // update the rotation of the four wheels depending on the velocity of the kart...
            float travelledDistanceDuringFrame = Time.deltaTime * RigidBody.velocity.magnitude;
            
            float frontWheelsPerimeter = 2f * Mathf.PI * _serializedData.FrontWheelsRadius;
            float frontWheelsRotationAngle = 360f * travelledDistanceDuringFrame / frontWheelsPerimeter;
            _serializedData.FrontLeftWheelRotation.Rotate(Vector3.up, frontWheelsRotationAngle, Space.Self);
            _serializedData.FrontRightWheelRotation.Rotate(Vector3.up, frontWheelsRotationAngle, Space.Self);

            float backWheelsPerimeter = 2f * Mathf.PI * _serializedData.BackWheelsRadius;
            float backWheelsRotationAngle = 360f * travelledDistanceDuringFrame / backWheelsPerimeter;
            _serializedData.BackLeftWheelRotation.Rotate(Vector3.up, backWheelsRotationAngle, Space.Self);
            _serializedData.BackRightWheelRotation.Rotate(Vector3.up, backWheelsRotationAngle, Space.Self);
        }

        #endregion
    }
}