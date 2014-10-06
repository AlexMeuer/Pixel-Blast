using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PixelBlastFree
{
    class AimedBullet : Bullet
    {
        new Vector2 direction; //new keyword is used because hiding is intended

        public AimedBullet(Vector2 startPos, Vector2 target, Texture2D tex)
            :base(startPos, tex, 1)
        {
            //define our direction vector
            direction = target - startPos;
            direction.Normalize();
        }

        public override void Update()
        {
            //base.Update();

            location += direction * MaxSpeed;   //follow our direction vector
        }
    }
}
