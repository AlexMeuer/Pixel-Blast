using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PixelBlastFree
{
    class Boss : Enemy
    {
        byte gunCooldown;
        new  const float MaxSpeed = 0.5f;   //slow moving (new keyword is used because hiding is intentional)
        bool movingDown;    //whether or not we are moving down the screen (as opposed to up the screen)
        Texture2D bulletTexture;
        readonly Vector2[] bulletSpawnPoints;    //the positions that the bullets will start at when we fire (declared as readonly because we dont want to change them after construction)

        public Boss(int xStart, int yStart, byte numberOfFrames, string textureToLoad, float difficultyFactor)
            : base(xStart, yStart, numberOfFrames, textureToLoad, difficultyFactor)
        {
            bulletTexture = content.Load<Texture2D>("defaultBullet");

            health *= 50;
            numberOfParticles = 1000;

            movingDown = true;
            gunCooldown = 15;

            isBoss = true;

            bulletSpawnPoints = new Vector2[3];
            //not absolute, but relative to our location vector
            bulletSpawnPoints[0] = new Vector2(30,60);
            bulletSpawnPoints[1] = new Vector2(55,60);
            bulletSpawnPoints[2] = new Vector2(80,60);
        }

        public override void Update(ref List<Bullet> enemyBullets, Vector2 playerCenter)
        {
            #region Movement
            //hovers between y=20 and y=80
            if (location.Y <= 80 && movingDown)
            {
                location.Y += MaxSpeed;
                //movingDown = true;
            }//end if
            else
            {
                location.Y -= MaxSpeed;
                movingDown = false;

                if (location.Y <= 20)
                    movingDown = true;
            }//end else
            #endregion

            if (gunCooldown == 0)
            {
                //for (int i = 0; i < enemyBullets.Count && gunCooldown == 0; i++)
                //{
                    //if (enemyBullets[i] != null)
                    //{
                foreach (Vector2 spawnPoint in bulletSpawnPoints)
                {
                    enemyBullets.Add(new AimedBullet(location + spawnPoint, playerCenter, bulletTexture));
                }
                    //}
                    gunCooldown = 50;
                   // break;
                //}
            }
            else if (gunCooldown > 0)
            {
                gunCooldown--;
            }
        }
    }
}
