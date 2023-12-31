using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAgent : MonoBehaviour
{
    public AIStateMachine stateMachine;
    public AIStateID initialState;
    public AIAgentConfig config;


   
    // Start is called before the first frame update
    void Start()
    {
        stateMachine = new AIStateMachine(this);
        stateMachine.RegisterState(new AIEnemyWanderState());
        stateMachine.RegisterState(new AIMeleeAttackState());
        stateMachine.RegisterState(new AIRangedAttackState());
        stateMachine.ChangeState(initialState);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

 
    public void ChangeState()
    {
        stateMachine.ChangeState(config.newState);
    }
    
}
