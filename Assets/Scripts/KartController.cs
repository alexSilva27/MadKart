using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MadKart
{
    public class KartController : MonoBehaviour
    {
        #region Private fields

        [Serializable]
        private struct SerializedData
        {
            [Header("Physics settings")]
            public LayerMask DrivableSurfacesLayer;
            public Rigidbody RigidBody;
            public SphereCollider KartCollider;
            public float RadiusForOverlapSphere;

            // motor...
            public float MinSpeed;
            public float MaxSpeedHorizontalSurface;
            public float MaxSpeedVerticalSurface;
            public float MaxSpeedChangeDampTime;
            public float SpeedGainPerSecond;

            // steering...
            public float MaxSteeringAngle;
            public float SteeringOnAngleChangeDampTime;
            public float SteeringOffAngleChangeDampTime;

            [Header("Visual settings")]
            public Transform KartVisualTopLevelTransform;
            public Transform FrontLeftWheelSteering;
            public Transform FrontRightWheelSteering;
            public Transform FrontLeftWheelRotation;
            public Transform FrontRightWheelRotation;
            public Transform BackLeftWheelRotation;
            public Transform BackRightWheelRotation;
            public float FrontWheelsRadius; // world space...
            public float BackWheelsRadius; // world space...
            public float KartVisualRotationChangeDampTime;
            public float KartVisualPositionChangeSmoothTime;

            [Header("Temp debug")]
            public ButtonTest TurnLeftButton;
            public ButtonTest TurnRightButton;
        }

        [SerializeField] private SerializedData _serializedData;

        private readonly Collider[] _results = new Collider[256];
        private Vector3 _lastKartVisualPosition;
        private Quaternion _lastKartVisualRotation;
        private Quaternion _lastKartTargetVisualRotation;
        private Vector3 _smoothDampVelocity;
        private Vector3 _lastSurfaceNormal; // last surface normal when the kart was grounded...
        private float _lastAllowedMaxSpeed;
        private bool _isGrounded;
        private float _steeringAngle;

        #endregion

        #region Public properties

        public Rigidbody RigidBody => _serializedData.RigidBody;

        #endregion

        #region Unity messages

        private void Start()
        {
            RigidBody.velocity = Vector3.forward * _serializedData.MinSpeed;

            _smoothDampVelocity = Vector3.zero;
            _steeringAngle = 0f;
            _isGrounded = false;
            _lastAllowedMaxSpeed = _serializedData.MaxSpeedHorizontalSurface;
            _lastSurfaceNormal = Vector3.up;
            _lastKartVisualRotation = Quaternion.identity;
            _lastKartTargetVisualRotation = Quaternion.identity;
            _lastKartVisualPosition = RigidBody.position - _lastSurfaceNormal * _serializedData.KartCollider.radius;
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
            _isGrounded = collidersHitCount > 0;

            // Debug.Log(_isGrounded);
            if (_isGrounded)
            {
                float nearestDistance = float.MaxValue;
                Vector3 contactPoint = default;

                for (int i = 0; i < collidersHitCount; i++)
                {
                    Collider collider = _results[i];
                    Vector3 colliderClosestPoint = default;

                    if (collider is MeshCollider meshCollider && !meshCollider.convex)
                    {
                        // unity Collider.ClosesPoint() does not work for non convex mesh colliders; so we have implemented our own implementation...
                        TileController tile = meshCollider.GetComponent<TileController>();
                        MeshColliderData colliderData = tile.MeshColliderData;
                        colliderClosestPoint = colliderData.ClosestPoint(in rigidBodyPosition);
                    }
                    else
                    {
                        colliderClosestPoint = collider.ClosestPoint(rigidBodyPosition);
                    }

                    float distanceToCollider = (rigidBodyPosition - colliderClosestPoint).magnitude;

                    if (distanceToCollider < nearestDistance)
                    {
                        nearestDistance = distanceToCollider;
                        contactPoint = colliderClosestPoint;
                    }
                }

                // Debug.Log((rigidBodyPosition - contactPoint).magnitude);

                if ((rigidBodyPosition - contactPoint).magnitude > _serializedData.RadiusForOverlapSphere)
                {
                    Debug.LogError("Something is wrong with Physics.OverlapSphereNonAlloc sphere radius or" +
                        "MeshColliderData.ClosestPoint()");
                }

                _lastSurfaceNormal = (rigidBodyPosition - contactPoint).normalized;

                // depending on the surface normal, we have different max speeds...
                float newAllowedMaxSpeedRatio = Mathf.Abs(Vector3.Cross(_lastSurfaceNormal, Vector3.up).magnitude);
                float newAllowedMaxSpeed = Mathf.Lerp(_serializedData.MaxSpeedHorizontalSurface,
                    _serializedData.MaxSpeedVerticalSurface, newAllowedMaxSpeedRatio);
                _lastAllowedMaxSpeed = Mathf.Lerp(_lastAllowedMaxSpeed, newAllowedMaxSpeed,
                    Time.fixedDeltaTime / _serializedData.MaxSpeedChangeDampTime);

                // calculate the tangent on the surface following the rigid body's velocity direction...
                Vector3 surfaceTangent = Vector3.ProjectOnPlane(RigidBody.velocity.normalized, _lastSurfaceNormal).normalized;

                // apply some kart motor effect to the rigid body's velocity...
                RigidBody.velocity += surfaceTangent * Time.fixedDeltaTime * _serializedData.SpeedGainPerSecond;
                RigidBody.velocity = Mathf.Clamp(RigidBody.velocity.magnitude, 0, _lastAllowedMaxSpeed) * RigidBody.velocity.normalized;

                if (_steeringAngle != 0f)
                {
                    // apply some steering effect to the rigid body's velocity...
                    // from the steering angle, calculate the turning radius using formula from: https://en.wikipedia.org/wiki/Turning_radius
                    float k1 = 1f; // wheelbase.
                    float k2 = 0f; // tire width;
                    float turningRadius = (k1 / Mathf.Sin(Mathf.Deg2Rad * _steeringAngle)) + (k2 / 2f);

                    // from the turning radius, calculate the angular velocity and then the angle to be used in this frame...
                    float angularVelocity = RigidBody.velocity.magnitude / turningRadius;
                    float rotationAngleDegrees = angularVelocity * Mathf.Rad2Deg * Time.fixedDeltaTime;
                    RigidBody.velocity = Quaternion.AngleAxis(rotationAngleDegrees, _lastSurfaceNormal) * RigidBody.velocity;
                }
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
            bool steeringOn = false;
            if (Input.GetKey(KeyCode.A) || _serializedData.TurnLeftButton.IsPressed)
            {
                steeringOn = true;
                targetSteeringAngle = -_serializedData.MaxSteeringAngle;
            }
            else if (Input.GetKey(KeyCode.D) || _serializedData.TurnRightButton.IsPressed)
            {
                steeringOn = true;
                targetSteeringAngle = _serializedData.MaxSteeringAngle;
            }

            float steeringDampTime = steeringOn ?
                _serializedData.SteeringOnAngleChangeDampTime : _serializedData.SteeringOffAngleChangeDampTime;
            _steeringAngle = Mathf.Lerp(_steeringAngle, targetSteeringAngle, Time.deltaTime / steeringDampTime);

            // calculate a smooth position for the kart visual...
            Vector3 targetPosition = RigidBody.position - _lastSurfaceNormal * _serializedData.KartCollider.radius;
            _lastKartVisualPosition = Vector3.SmoothDamp(_lastKartVisualPosition, targetPosition, ref _smoothDampVelocity,
                _serializedData.KartVisualPositionChangeSmoothTime);
            _serializedData.KartVisualTopLevelTransform.position = _lastKartVisualPosition;

            // calculate a smooth rotation for the kart visual...
            _lastKartTargetVisualRotation = (RigidBody.velocity == Vector3.zero || !_isGrounded) ?
                                            _lastKartTargetVisualRotation :
                                            Quaternion.LookRotation(RigidBody.velocity.normalized, _lastSurfaceNormal);
            _lastKartVisualRotation = Quaternion.Slerp(_lastKartVisualRotation, _lastKartTargetVisualRotation,
                Time.deltaTime / _serializedData.KartVisualRotationChangeDampTime);
            _serializedData.KartVisualTopLevelTransform.rotation = _lastKartVisualRotation;

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