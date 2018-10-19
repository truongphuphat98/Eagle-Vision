using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EagleVision
{
    [ExecuteInEditMode]
    public class ScanEffect : MonoBehaviour
    {
        public Transform playerScanPosition;
        public Material effectMaterial;

        [Header("Scan Attributes")]
        public float scanDistance;
        public float scanSpeed;

        private Camera _camera;

        // Demo Code
        bool _scanning;
        Scannable[] _scannables;

        void Start()
        {
            _scannables = FindObjectsOfType<Scannable>();
        }

        void Update()
        {
            if (_scanning)
            {
                if (scanDistance < 30)
                {
                    scanDistance += Time.deltaTime * scanSpeed;
                    foreach (Scannable s in _scannables)
                    {
                        if (Vector3.Distance(playerScanPosition.transform.position, s.transform.position) <= scanDistance)
                            s.Ping();
                    }
                    if (scanDistance > 30)
                    {
                        _scanning = false;
                        scanDistance = 0;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                _scanning = true;
                scanDistance = 0;
            }
 
        }

        void OnEnable()
        {
            _camera = GetComponent<Camera>();
            _camera.depthTextureMode = DepthTextureMode.Depth;
        }

        [ImageEffectOpaque]
        void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            effectMaterial.SetVector("_WorldSpaceScannerPos", playerScanPosition.transform.position);
            effectMaterial.SetFloat("_ScanDistance", scanDistance);
            RaycastCornerBlit(src, dst, effectMaterial);
        }

        void RaycastCornerBlit(RenderTexture source, RenderTexture dest, Material mat)
        {
            // Compute Frustum Corners
            float camFar = _camera.farClipPlane;
            float camFov = _camera.fieldOfView;
            float camAspect = _camera.aspect;

            float fovWHalf = camFov * 0.5f;

            Vector3 toRight = _camera.transform.right * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
            Vector3 toTop = _camera.transform.up * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

            Vector3 topLeft = (_camera.transform.forward - toRight + toTop);
            float camScale = topLeft.magnitude * camFar;

            topLeft.Normalize();
            topLeft *= camScale;

            Vector3 topRight = (_camera.transform.forward + toRight + toTop);
            topRight.Normalize();
            topRight *= camScale;

            Vector3 bottomRight = (_camera.transform.forward + toRight - toTop);
            bottomRight.Normalize();
            bottomRight *= camScale;

            Vector3 bottomLeft = (_camera.transform.forward - toRight - toTop);
            bottomLeft.Normalize();
            bottomLeft *= camScale;


            RenderTexture.active = dest;

            mat.SetTexture("_MainTex", source);

            GL.PushMatrix();
            GL.LoadOrtho();

            mat.SetPass(0);

            GL.Begin(GL.QUADS);

            GL.MultiTexCoord2(0, 0.0f, 0.0f);
            GL.MultiTexCoord(1, bottomLeft);
            GL.Vertex3(0.0f, 0.0f, 0.0f);

            GL.MultiTexCoord2(0, 1.0f, 0.0f);
            GL.MultiTexCoord(1, bottomRight);
            GL.Vertex3(1.0f, 0.0f, 0.0f);

            GL.MultiTexCoord2(0, 1.0f, 1.0f);
            GL.MultiTexCoord(1, topRight);
            GL.Vertex3(1.0f, 1.0f, 0.0f);

            GL.MultiTexCoord2(0, 0.0f, 1.0f);
            GL.MultiTexCoord(1, topLeft);
            GL.Vertex3(0.0f, 1.0f, 0.0f);

            GL.End();
            GL.PopMatrix();
        }
    }
}
