public class ShottingPaddle : Collectable
{
    protected override void ApplyEffect()
    {
        Paddle.Instance.StartShooting();
    }
}