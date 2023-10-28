using Godot;

public partial class pathFinding : CharacterBody3D {

    public Vector3 currentAgentPosition;
    public Vector3 nextPathPosition;

    private NavigationAgent3D navigationAgent;

    private float movementSpeed = 2.0f;
    private Vector3 movementTargetPosition = new(0, 0, 0);

    public Vector3 MovementTarget {
        get { return navigationAgent.TargetPosition; }
        set { navigationAgent.TargetPosition = value; }
    }

    public override void _Ready() {
        base._Ready();

        navigationAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");

        // These values need to be adjusted for the actor's speed
        // and the navigation layout.
        navigationAgent.PathDesiredDistance = 2f;
        navigationAgent.TargetDesiredDistance = 2f;
    }
    public override void _PhysicsProcess(double delta) {
        if (currentAgentPosition != navigationAgent.TargetPosition) {
            currentAgentPosition = GlobalTransform.Origin;
            nextPathPosition = navigationAgent.GetNextPathPosition();

            Vector3 newVelocity = (nextPathPosition - currentAgentPosition).Normalized();
            newVelocity *= movementSpeed;
            Velocity = newVelocity;
            MoveAndSlide();
        }
    }

    private async void ActorSetup() {
        // Wait for the first physics frame so the NavigationServer can sync.
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        // Now that the navigation map is no longer empty, set the movement target.
        MovementTarget = movementTargetPosition;
    }

    public void CalculatemovementTarget(Vector3 targetPosition) {
        movementTargetPosition = targetPosition;
        Callable.From(ActorSetup).CallDeferred();

        currentAgentPosition = GlobalTransform.Origin;
        nextPathPosition = navigationAgent.GetNextPathPosition();

        Vector3 newVelocity = (nextPathPosition - currentAgentPosition).Normalized();
        newVelocity *= movementSpeed;
        Velocity = newVelocity;
    }
}
