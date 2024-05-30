using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEngine.XR.Content.Interaction
{
    /// <summary>
    /// An interactable lever that snaps into an on or off position by a direct interactor
    /// </summary>
    public class XRLever : XRBaseInteractable
    {
        const float k_LeverDeadZone = 0.1f; // Prevents rapid switching between on and off states when right in the middle

        private Coroutine cubeRoutine = null;
        
        [SerializeField]
        private GameObject cubePrefab; // Assign a prefab of the cube in the inspector

        [SerializeField]
        private GameObject pyramidPrefab; // Assign a prefab of the cube in the inspector

        [SerializeField]
        private GameObject cylinderPrefab; // Assign a prefab of the cube in the inspector

        [SerializeField]
        [Tooltip("The object that is visually grabbed and manipulated")]
        Transform m_Handle = null;

        [SerializeField]
        [Tooltip("The value of the lever")]
        bool m_Value = false;

        [SerializeField]
        [Tooltip("If enabled, the lever will snap to the value position when released")]
        bool m_LockToValue;

        [SerializeField]
        [Tooltip("Angle of the lever in the 'on' position")]
        [Range(-90.0f, 90.0f)]
        float m_MaxAngle = 90.0f;

        [SerializeField]
        [Tooltip("Angle of the lever in the 'off' position")]
        [Range(-90.0f, 90.0f)]
        float m_MinAngle = -90.0f;

        [SerializeField]
        [Tooltip("Events to trigger when the lever activates")]
        UnityEvent m_OnLeverActivate = new UnityEvent();

        [SerializeField]
        [Tooltip("Events to trigger when the lever deactivates")]
        UnityEvent m_OnLeverDeactivate = new UnityEvent();

        IXRSelectInteractor m_Interactor;

        /// <summary>
        /// The object that is visually grabbed and manipulated
        /// </summary>
        public Transform handle
        {
            get => m_Handle;
            set => m_Handle = value;
        }

        /// <summary>
        /// The value of the lever
        /// </summary>
        public bool value
        {
            get => m_Value;
            set => SetValue(value, true);
        }

        /// <summary>
        /// If enabled, the lever will snap to the value position when released
        /// </summary>
        public bool lockToValue { get; set; }

        /// <summary>
        /// Angle of the lever in the 'on' position
        /// </summary>
        public float maxAngle
        {
            get => m_MaxAngle;
            set => m_MaxAngle = value;
        }

        /// <summary>
        /// Angle of the lever in the 'off' position
        /// </summary>
        public float minAngle
        {
            get => m_MinAngle;
            set => m_MinAngle = value;
        }

        /// <summary>
        /// Events to trigger when the lever activates
        /// </summary>
        public UnityEvent onLeverActivate => m_OnLeverActivate;

        /// <summary>
        /// Events to trigger when the lever deactivates
        /// </summary>
        public UnityEvent onLeverDeactivate => m_OnLeverDeactivate;

        void Start()
        {
            SetValue(m_Value, true);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            selectEntered.AddListener(StartGrab);
            selectExited.AddListener(EndGrab);
        }

        protected override void OnDisable()
        {
            selectEntered.RemoveListener(StartGrab);
            selectExited.RemoveListener(EndGrab);
            base.OnDisable();
        }

        void StartGrab(SelectEnterEventArgs args)
        {
            m_Interactor = args.interactorObject;
        }

        void EndGrab(SelectExitEventArgs args)
        {
            SetValue(m_Value, true);
            m_Interactor = null;
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractable(updatePhase);

            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
            {
                if (isSelected)
                {
                    UpdateValue();
                }
            }
        }

        Vector3 GetLookDirection()
        {
            Vector3 direction = m_Interactor.GetAttachTransform(this).position - m_Handle.position;
            direction = transform.InverseTransformDirection(direction);
            direction.x = 0;

            return direction.normalized;
        }

        void UpdateValue()
        {
            var lookDirection = GetLookDirection();
            var lookAngle = Mathf.Atan2(lookDirection.z, lookDirection.y) * Mathf.Rad2Deg;

            if (m_MinAngle < m_MaxAngle)
                lookAngle = Mathf.Clamp(lookAngle, m_MinAngle, m_MaxAngle);
            else
                lookAngle = Mathf.Clamp(lookAngle, m_MaxAngle, m_MinAngle);

            var maxAngleDistance = Mathf.Abs(m_MaxAngle - lookAngle);
            var minAngleDistance = Mathf.Abs(m_MinAngle - lookAngle);

            if (m_Value)
                maxAngleDistance *= (1.0f - k_LeverDeadZone);
            else
                minAngleDistance *= (1.0f - k_LeverDeadZone);

            var newValue = (maxAngleDistance < minAngleDistance);

            SetHandleAngle(lookAngle);

            SetValue(newValue);
        }

        void SetValue(bool isOn, bool forceRotation = false)
        {
            if (m_Value == isOn)
            {
                if (forceRotation)
                    SetHandleAngle(m_Value ? m_MaxAngle : m_MinAngle);

                return;
            }

            m_Value = isOn;

            if (m_Value)
            {
                m_OnLeverActivate.Invoke();           
                if (cubeRoutine != null)
                {
                    StopCoroutine(cubeRoutine);
                    cubeRoutine = null;
                }                
            }
            else
            {
                m_OnLeverDeactivate.Invoke();
                if (cubeRoutine != null)
                    StopCoroutine(cubeRoutine);
                cubeRoutine = StartCoroutine(MoveAndDestroyShape());                               
            }

            if (!isSelected && (m_LockToValue || forceRotation))
                SetHandleAngle(m_Value ? m_MaxAngle : m_MinAngle);
        }

        void SetHandleAngle(float angle)
        {
            if (m_Handle != null)
                m_Handle.localRotation = Quaternion.Euler(angle, 0.0f, 0.0f);
        }

        void OnDrawGizmosSelected()
        {
            var angleStartPoint = transform.position;

            if (m_Handle != null)
                angleStartPoint = m_Handle.position;

            const float k_AngleLength = 0.25f;

            var angleMaxPoint = angleStartPoint + transform.TransformDirection(Quaternion.Euler(m_MaxAngle, 0.0f, 0.0f) * Vector3.up) * k_AngleLength;
            var angleMinPoint = angleStartPoint + transform.TransformDirection(Quaternion.Euler(m_MinAngle, 0.0f, 0.0f) * Vector3.up) * k_AngleLength;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(angleStartPoint, angleMaxPoint);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(angleStartPoint, angleMinPoint);
        }

        void OnValidate()
        {
            SetHandleAngle(m_Value ? m_MaxAngle : m_MinAngle);
        }

        private IEnumerator MoveAndDestroyShape()
        {
            // Randomly select a shape (cube, pyramid, or cylinder)
            GameObject shapePrefab;
            int randomIndex = Random.Range(0, 3); // Generates a random integer between 0 (inclusive) and 3 (exclusive)
            switch(randomIndex)
            {
                case 0:
                    shapePrefab = cubePrefab;
                    break;
                case 1:
                    shapePrefab = pyramidPrefab;
                    break;
                case 2:
                    shapePrefab = cylinderPrefab;
                    break;
                default:
                    shapePrefab = cubePrefab; // Default to cube if something goes wrong
                    break;
            }

            GameObject shape = Instantiate(shapePrefab, new Vector3(4.15f, 1.0f, 3.399f), Quaternion.identity);

            if(shape != null)
            {
                // Move the shape according to its type
                Vector3 endPosition;
                endPosition = shape.transform.position - transform.up * 7.25f;
                yield return StartCoroutine(MoveObject(shape, endPosition, 2.0f));

                endPosition = shape.transform.position - transform.right * 8.25f;
                yield return StartCoroutine(MoveObject(shape, endPosition, 2.0f));

                endPosition = shape.transform.position + transform.up * 1.7f;
                yield return StartCoroutine(MoveObject(shape, endPosition, 2.0f));

                endPosition = shape.transform.position + transform.right * 3.25f;
                yield return StartCoroutine(MoveObject(shape, endPosition, 2.0f));

                // Wait for 4 seconds and then disappear
                // yield return new WaitForSeconds(4);
                // if(shape != null) Destroy(shape); // Ensure the object still exists before destroying it
            }
        }


        private IEnumerator MoveObject(GameObject obj, Vector3 target, float duration)
        {
            float elapsed = 0;
            Vector3 startPos = obj.transform.position;

            while (elapsed < duration && obj != null) // Ensure obj is still valid
            {
                obj.transform.position = Vector3.Lerp(startPos, target, elapsed / duration);
                elapsed += Time.deltaTime;
                
                yield return null;
            }
            if (obj != null) obj.transform.position = target; // Ensure obj is still valid
        }        
    }
}
