using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    private void Update()
    {
        if (Player.Instance != null)
            transform.position = new Vector3(Player.Instance.transform.position.x, Player.Instance.transform.position.y, -10);
    }
}
