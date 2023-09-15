using System;
using System.Collections;
using System.Collections.Generic;
using Infrastructure;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class NavController : NetworkBehaviour
{
    [SerializeField] private StoreBase robot;
    [SerializeField] private NavMeshObstacle obstacle;
    [SerializeField] private NavMeshAgent agent;
    
    private void Start()
    {
        //仅服务端开启寻路
        if (isServer)
        { 
            robot = GetComponent<StoreBase>();
            obstacle = GetComponent<NavMeshObstacle>();
            agent = GetComponent<NavMeshAgent>();
     
            if (robot == null || obstacle == null)
                return;
            
            if (robot.id.role==Identity.Roles.AutoSentinel && agent != null)
            {
                if(robot.id.camp == Identity.Camps.Blue)
                    agent.agentTypeID = NavMesh.GetSettingsByIndex(0).agentTypeID;
                else
                    agent.agentTypeID = NavMesh.GetSettingsByIndex(1).agentTypeID;
                
                agent.enabled = true;
                // agent.updatePosition = false;
            }
            else if(obstacle != null)
                obstacle.enabled = true;
        }
    }

    public void SetTarget(Vector3 input)
    {
        agent.SetDestination(input);
    }

    public Vector3 NextPosition() => agent.nextPosition;

    public bool IsReach()
    {
        var nextPosition = agent.nextPosition;
        var pathEndPosition = agent.pathEndPosition;
        
        return Vector2.Distance(new Vector2(nextPosition.x,nextPosition.z), new Vector2(pathEndPosition.x,pathEndPosition.z)) < 1f;
    }
}
