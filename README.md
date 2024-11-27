# Universal Player and AI State Machine

### About the project
In this project I wanted to showcase the use of state machine in player and non-playable character behavior.

### What does it do
Working this way protects the game from attempting to use behaviors it shouldn't have access to at this point. For example player is not allowed to attack while blocking, player has to first leave the blocking state to be able to hit the enemy.

### How does it work
```csharp
public class PlayerTargetingState : PlayerBaseState
{
    private readonly int TargetingBlendtreeHash = Animator.StringToHash("TargetingBlendTree");
    private readonly int TargetingForwardHash = Animator.StringToHash("TargetingForward");
    private readonly int TargetingRightHash = Animator.StringToHash("TargetingRight");
    public PlayerTargetingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.InputReader.CancelEvent += OnCancel;
        stateMachine.Animator.CrossFadeInFixedTime(TargetingBlendtreeHash, 0.1f);
    }

    public override void Tick(float deltaTime)
    {
        if (stateMachine.InputReader.IsAttacking)
        {
            stateMachine.SwitchState(new PlayerAttackingState(stateMachine, 0));
            return;
        }

        if (stateMachine.InputReader.IsBlocking)
        {
            stateMachine.SwitchState(new PlayerBlockingState(stateMachine));
            return;
        }
        if (stateMachine.Targeter.CurrentTarget == null)
        {
            stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
            return;
        }

        Vector3 movement = CalculateMovement();
        Move(movement * stateMachine.TargetingMovementSpeed, deltaTime);
        UpdateAnimator(deltaTime);
        FaceTarget();
    }

    public override void Exit()
    {
        stateMachine.InputReader.CancelEvent -= OnCancel;
    }

    private void OnCancel()
    {
        stateMachine.Targeter.Cancel();
        stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
    }

    private Vector3 CalculateMovement()
    {
        Vector3 movement = new Vector3();

        movement += stateMachine.transform.right * stateMachine.InputReader.MovementValue.x;
        movement += stateMachine.transform.forward * stateMachine.InputReader.MovementValue.y;

        return movement;
    }

    private void UpdateAnimator(float deltaTime)
    {
        if (stateMachine.InputReader.MovementValue.x == 0)
        {
            stateMachine.Animator.SetFloat(TargetingForwardHash, 0, 0.1f, deltaTime);
        }
        else
        {
            float value = stateMachine.InputReader.MovementValue.x > 0 ? 1f : -1f;
            stateMachine.Animator.SetFloat(TargetingForwardHash, value, 0.1f, deltaTime);
        }
        
        if (stateMachine.InputReader.MovementValue.y == 0)
        {
            stateMachine.Animator.SetFloat(TargetingRightHash, 0, 0.1f, deltaTime);
        }
        else
        {
            float value = stateMachine.InputReader.MovementValue.y > 0 ? 1f : -1f;
            stateMachine.Animator.SetFloat(TargetingRightHash, value, 0.1f, deltaTime);
        }
        
        
    }
}
```
 This example state is responsible for focusing the camera and player character on an enemy.
- When entering the state player character subscribes to the event of cancelling the focus when player clicks escape button. The animation gets cross faded between the previous player character's animation and focus mode animations.
- The tick method is being called with the update method - every frame the game checks for the requirements for switching the state. From the focus state the accessible states are: free look, attacking and blocking.
- Tick is also responsible for movement, animating the character and making sure the character is always facing the focused target.
- When the state machine switches states from this one it has to unsubscribe from the event responsible for cancelling the current state.
- Further in the script are methods required for this state to work, while the Enter, Tick and Exit methods are shared across other states.
- OnCancel method, responsible for removing the current target from the list and leaving the current state.
- CalculateMovement method responsible for translating InputReader data to this states script.
- UpdateAnimator method responsible for setting the animation blend tree to proper floats depending on player characters movement.
