using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject focusPoint;
    [SerializeField] private bool spawnDebugSphere;
    [SerializeField] private GameObject playerCamera;

    [SerializeField] private float cameraSpeed = 1f;
    [SerializeField] private float cameraZoomSpeed = 1f;
    [SerializeField] private float cameraRotateSpeed = 1f;

    private Transform _cameraStart;

    private void Start()
    {
        if (focusPoint == null)
        {
            focusPoint = GameObject.FindGameObjectWithTag("FocusPoint");
        }

        if (playerCamera == null)
        {
            if (Camera.main != null) playerCamera = Camera.main.gameObject;
        }

        if (spawnDebugSphere)
        {
            SpawnDebugSphere();
        }
        
        _cameraStart = playerCamera.transform;
        
        WorldInfo.Instance.OnLoadAction += ResetCamera;
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput() //Kaulo scuffed
    {
        Move();
        
        KeepInBounds();

        Zoom();

        Rotate();

    }

    private void Move()
    {
        Vector3 targetPos = focusPoint.transform.position;
        if (Input.GetKey(KeyCode.W))
        {
            targetPos += focusPoint.transform.forward * (cameraSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            targetPos -= focusPoint.transform.right * (cameraSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            targetPos -= focusPoint.transform.forward * (cameraSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            targetPos += focusPoint.transform.right * (cameraSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            targetPos.y += cameraSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            targetPos.y -= cameraSpeed * Time.deltaTime;
        }

        focusPoint.transform.position = targetPos;
    }

    private void KeepInBounds()
    {
        Vector3 targetPos = focusPoint.transform.position;
        if (targetPos.z > WorldInfo.Instance.GetMaxBounds().z)
        {
            targetPos.z = WorldInfo.Instance.GetMaxBounds().z;
        }
        if(targetPos.z < WorldInfo.Instance.GetMinBounds().z)
        {
            targetPos.z = WorldInfo.Instance.GetMinBounds().z;
        }
        
        if (targetPos.x > WorldInfo.Instance.GetMaxBounds().x)
        {
            targetPos.x = WorldInfo.Instance.GetMaxBounds().x;
        }
        if(targetPos.x < WorldInfo.Instance.GetMinBounds().x)
        {
            targetPos.x = WorldInfo.Instance.GetMinBounds().x;
        }
        
        if (targetPos.y > WorldInfo.Instance.GetMaxBounds().y)
        {
            targetPos.y = WorldInfo.Instance.GetMaxBounds().y;
        }
        if(targetPos.y < WorldInfo.Instance.GetMinBounds().y)
        {
            targetPos.y = WorldInfo.Instance.GetMinBounds().y;
        }
        
        focusPoint.transform.position = targetPos;

    }

    private void Zoom()
    {
        Vector3 cameraPos = playerCamera.transform.position;

        if (Input.mouseScrollDelta.y != 0)
        {
            cameraPos += playerCamera.transform.forward * (Input.mouseScrollDelta.y * (cameraZoomSpeed * Time.deltaTime));
        }
        
        playerCamera.transform.position = cameraPos;
    }

    private void Rotate()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            focusPoint.transform.RotateAround(focusPoint.transform.position, Vector3.up, cameraRotateSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            focusPoint.transform.RotateAround(focusPoint.transform.position, Vector3.up, -cameraRotateSpeed * Time.deltaTime);
        }
    }

    private void ResetCamera()
    {
        playerCamera.transform.position = _cameraStart.position;
        playerCamera.transform.rotation = _cameraStart.rotation;
    }
    
    private void SpawnDebugSphere()
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.position = focusPoint.transform.position;
        go.transform.SetParent(focusPoint.transform);
    }
    
    private void OnDestroy()
    {
        WorldInfo.Instance.OnLoadAction -= ResetCamera;
    }
    
}
