
namespace PixelBlastFree
{
    class DiagonalEnemy : HorizontalEnemy
    {
        public DiagonalEnemy(int xStart, int yStart, int numberOfFrames, string textureToLoad, float difficultyFactor)
            : base(xStart, yStart, numberOfFrames, textureToLoad, difficultyFactor)
        {
            //all handled in base contructor
        }

        public override void Update()
        {
            location.Y += MaxSpeed-1;
            base.Update();
        }
    }
}
