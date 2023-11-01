public class CollectWallJump : Collectable
{
    protected override void OnPickupAction()
    {
        base.OnPickupAction();
        player.canWallJump = true;
    }
}
