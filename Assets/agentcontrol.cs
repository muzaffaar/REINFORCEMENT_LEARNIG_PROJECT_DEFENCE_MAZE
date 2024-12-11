using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class agentcontrol : Agent
{
    public float speed = 10f;

    public Transform TargetTransform;

    private enum ACTIONS
    {
        LEFT = 0,
        FORWARD = 1,
        RIGHT = 2,
        BACKWARD = 3,
        NOTHING = 4
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(10.31f, 0.46f, 10.73f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // The position of the agent
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.y);


        // The distance between the agent and the treasure
        sensor.AddObservation(Vector3.Distance(TargetTransform.localPosition, transform.localPosition));
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> actions = actionsOut.DiscreteActions;

        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == +1)
        {
            actions[0] = (int)ACTIONS.FORWARD;
        }
        else if (horizontal == -1)
        {
            actions[0] = (int)ACTIONS.BACKWARD;
        }
        else if (vertical == +1)
        {
            actions[0] = (int)ACTIONS.LEFT;
        }
        else if (vertical == -1)
        {
            actions[0] = (int)ACTIONS.RIGHT;
        }
        else
        {
            actions[0] = 4;
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var actionTaken = actions.DiscreteActions[0];

        switch (actionTaken)
        {
            case (int)ACTIONS.FORWARD:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case (int)ACTIONS.LEFT:
                transform.rotation = Quaternion.Euler(0, -90, 0);
                break;
            case (int)ACTIONS.RIGHT:
                transform.rotation = Quaternion.Euler(0, +90, 0);
                break;
            case (int)ACTIONS.BACKWARD:
                transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
        }

        if(actionTaken != (int)ACTIONS.NOTHING)
        transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);

        AddReward(-0.1f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Wall")
        {
            AddReward(-1f);
            EndEpisode();
        }
        else if (collision.collider.tag == "Target")
        {
            AddReward(100);
            EndEpisode();
        }
    }
}