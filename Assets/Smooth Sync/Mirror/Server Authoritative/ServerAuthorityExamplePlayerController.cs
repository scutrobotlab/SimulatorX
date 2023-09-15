using UnityEngine;
using Mirror;
using Smooth;

public class ServerAuthorityExamplePlayerController : NetworkBehaviour
{
    Rigidbody rb;

    /// <summary>
    /// The speed to move per second when there is no rigidbody component on the player
    /// </summary>
    public float transformMovementSpeed = 30.0f;

    /// <summary>
    /// The force to add on key up when there is a rigidbody component on the player
    /// </summary>
    public float rigidbodyMovementForce = 500;

    SmoothSyncMirror smoothSync;

	void Awake ()
    {
        rb = GetComponent<Rigidbody>();
        smoothSync = GetComponent<SmoothSyncMirror>();
    }

    public override void OnStartServer()
    {
        rb.isKinematic = false;
        base.OnStartServer();
    }

    void Update ()
    {
        if (hasAuthority)
        {
            // Add forces to the parent rigidbody
            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                CmdMove(KeyCode.DownArrow);
            }
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                CmdMove(KeyCode.UpArrow);
            }
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                CmdMove(KeyCode.LeftArrow);
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                CmdMove(KeyCode.RightArrow);
            }
            if (Input.GetKeyUp(KeyCode.T))
            {
                CmdTeleport();
            }
        }
    }

    [Command]
    void CmdTeleport()
    {
        smoothSync.teleportAnyObjectFromServer(transform.position + Vector3.right * 5, transform.rotation, transform.localScale);
    }

    [Command]
    void CmdMove(KeyCode keyCode)
    {
        switch (keyCode)
        {
            case KeyCode.DownArrow: 
                rb.AddForce(new Vector3(0, -1.5f, -1) * rigidbodyMovementForce);
                break;
            case KeyCode.UpArrow:
                rb.AddForce(new Vector3(0, 1.5f, 1) * rigidbodyMovementForce);
                break;
            case KeyCode.LeftArrow:
                rb.AddForce(new Vector3(-1, 0, 0) * rigidbodyMovementForce);
                break;
            case KeyCode.RightArrow: 
                rb.AddForce(new Vector3(1, 0, 0) * rigidbodyMovementForce);
                break;
        }
    }

}
