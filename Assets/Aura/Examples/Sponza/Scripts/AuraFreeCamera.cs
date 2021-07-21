using UnityEngine;

namespace AuraAPI //Original from Unity, changed namespace to avoid conflicts when importing official packages
{
    public class AuraFreeCamera : MonoBehaviour // Renamed from FreeCamera because it still generated conflicts when adding the FreeCamera(the one from the global context) component via script, even though the one from Aura was encapsulated in the AuraAPI namespace
    {
        public bool freeLookEnabled = false;
        public bool showCursor = false;

        public float lookSpeed = 5f;
        public float moveSpeed = 5f;
        public float sprintSpeed = 50f;

        float m_yaw;
        float m_pitch;

        void Start()
        {
            m_yaw = transform.rotation.eulerAngles.y;
            m_pitch = transform.rotation.eulerAngles.x;
            Cursor.visible = showCursor;
        }

        void Update()
        {
            if(!freeLookEnabled)
                return;

            m_yaw = (m_yaw + lookSpeed * Input.GetAxis("Mouse X")) % 360f;
            m_pitch = (m_pitch - lookSpeed * Input.GetAxis("Mouse Y")) % 360f;
            transform.rotation = Quaternion.AngleAxis(m_yaw, Vector3.up) * Quaternion.AngleAxis(m_pitch, Vector3.right);

            var speed = Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : moveSpeed);
            var forward = speed * Input.GetAxis("Vertical");
            var right = speed * Input.GetAxis("Horizontal");
            var up = speed * ((Input.GetKey(KeyCode.Q) ? 1f : 0f) - (Input.GetKey(KeyCode.E) ? 1f : 0f));
            transform.position += transform.forward * forward + transform.right * right + Vector3.up * up;
        }
    }
}
