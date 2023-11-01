public class CollectDoubleJump : Collectable
{
    protected override void OnPickupAction()
    {
        base.OnPickupAction();
        player.canDoubleJump = true;
    }
}
