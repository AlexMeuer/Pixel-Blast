using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Devices;

namespace PixelBlastFree
{
    //------------------------------------------//
    //  TODO:                                   //
    //      - remove offscreen enemies          //
    //      - check collision                   //
    //      - implement more enemy types        //
    //------------------------------------------//

    class Wave
    {
        List<Enemy> enemyList;  //the enemies to be spawned
        int index;  //the index of the next enemy to spawn
        TimeSpan delay; //the delay between each eney spawn
        TimeSpan timeSinceLastSpawn;    //the amount of time since an enemy was spawned
        bool finished; //whether all enemies are gone and wave is done

        /// <summary>
        /// Creates a new Wave instance.
        /// NOTE: Enemies will not be spawned until Start() is called.
        /// </summary>
        /// <param name="xmlPath">the path to the xml document to load the wave from</param>
        /// <param name="difficultyFactor">the difficulty to scale the enemies by</param>
        /// <param name="delay">how long to wait between spawning enemies</param>
        public Wave(string xmlPath, float difficultyFactor, TimeSpan delay)
        {
            enemyList = LoadFromXML(xmlPath, difficultyFactor);
            this.delay = delay;
            index = 0;
            timeSinceLastSpawn = TimeSpan.Zero;
        }

        /// <summary>
        /// Starts the wave
        /// </summary>
        public void Start()
        {
            timeSinceLastSpawn = TimeSpan.MaxValue;
        }

        /// <summary>
        /// Loads enemies from an xml file
        /// </summary>
        /// <param name="xmlPath">the path to the xml file</param>
        /// <param name="difficultyFactor">the difficulty to scale the enemies by</param>
        /// <returns>a list of enemies</returns>
        private List<Enemy> LoadFromXML(string xmlPath, float difficultyFactor)
        {
            XDocument doc = XDocument.Load(xmlPath);

            var data = from query in doc.Descendants("enemy")
                       select new Enemy
                       (
                            (int)query.Element("x"),
                            (int)query.Element("y"),
                            (int)query.Element("frames"),
                            (string)query.Element("texture"),
                            difficultyFactor
                       );

            return data.ToList<Enemy>();
        }

        /// <summary>
        /// Updates all the enemies in the wave
        /// </summary>
        /// <param name="elapsedTime">time since last update</param>
        public void Update(TimeSpan elapsedTime)
        {
            //only concern ourselves with spawning new enemies if there are any left to spawn
            if (index < enemyList.Count)
            {
                if (timeSinceLastSpawn >= delay)
                {
                    index++;
                    timeSinceLastSpawn = TimeSpan.Zero;
                }
                else
                {
                    timeSinceLastSpawn += elapsedTime;
                }
            }

            if (enemyList.Count > 0)
            {
                for (int i = 0; i <= index; i++)
                {
                    enemyList[i].Update();
                }
            }
            else
            {
                finished = true;
            }
        }


        #region Entity & Collision Handling (enemies, bullets, etc)
        private void HandleBullets(ref List<Bullet> playerBullets, ref List<Bullet> enemyBullets)
        {
            for (int i = 0; i < playerBullets.Count; i++)
            {
                playerBullets[i].Update();
                if (OffScreen.Check((int)GamePage.screen.Y, (int)GamePage.screen.X, playerBullets[i].BoundingBox) || playerBullets[i].FlaggedForRemoval == true)
                {
                    playerBullets.RemoveAt(i);
                    i--;//dont want to skip anyhting in the list
                }
            }
            for (int i = 0; i < enemyBullets.Count; i++)
            {
                //if (enemyBullets[i] != null) --> no longer using array (see top of class)
                //{
                enemyBullets[i].Update();
                if (OffScreen.Check((int)GamePage.screen.Y, (int)GamePage.screen.X, enemyBullets[i].BoundingBox) || enemyBullets[i].FlaggedForRemoval == true)
                {
                    enemyBullets.RemoveAt(i);
                    i--;//dont want to skip anything in the list!
                    //   }
                }//end null check
            }
            //TODO: handle other bullet types
            //TODO: include offscreen check
        }

        /// <summary>
        /// Calls or handles all the updating and spawning for all enemies in the game
        /// </summary>
        private void HandleEnemies(TimeSpan time, ref Player player, ref List<Bullet> enemyBullets)
        {

            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList[i].IsBoss)       //try
                {                           //only works for boss, else will throw NotImplementedException, which will be caught and enemy will update normally
                    enemyList[i].Update(ref enemyBullets, player.Centre);  //tell enemy to update
                }
                else //enemy is normal      //catch //(NotImplementedException ex) 
                {
                    enemyList[i].Update();
                }
                //remove the enemy if is is dead or if it is off the screen and it's allowance is up
                if (enemyList[i].Health <= 0 || (OffScreen.Check((int)GamePage.screen.Y, (int)GamePage.screen.X, enemyList[i].BoundingBox) == true && enemyList[i].OffScreenAllowance <= 0))
                {
                    if (enemyList[i].Health <= 0)
                    {   //if player killed the enemy, then add particle effects
                        Vector2 origin = new Vector2(enemyList[i].Location.X + enemyList[i].CollisionMask.Bounds.Center.X, enemyList[i].Location.Y + enemyList[i].CollisionMask.Bounds.Center.Y);
                        Explosion newExplosion = new Explosion(origin, enemyList[i].ExplosionSize, 5, 3, time);
                        particleEffectsList.Add(newExplosion);
                        player.EnemiesKilled++;
                    }
                    enemyList.RemoveAt(i);
                    i--;//we dont want to skip anything in the list
                }
            }//end for
        }//end HandleEnemies()

        private void HandleCollision(TimeSpan time, ref Player player, ref List<Bullet> playerBullets, ref List<Bullet> enemyBullets, ref VibrateController vibrate, ref List<Explosion> particleEffectsList)
        {
            for (int enemyIndex = 0; enemyIndex < enemyList.Count; enemyIndex++)  //for every single enemy that exists
            {
                for (int bulletIndex = 0; bulletIndex < playerBullets.Count; bulletIndex++)  //for every single bullet that exists
                {
                    #region Standard Collision
                    if (enemyList[enemyIndex].FloatRotation == 0) //changed from: try
                    {
                        if (Collision.BoundingBox(playerBullets[bulletIndex].BoundingBox, enemyList[enemyIndex].BoundingBox))  //check is bounding boxes intersect
                        {
                            if (Collision.PerPixel(playerBullets[bulletIndex].BoundingBox, playerBullets[bulletIndex].Texture, enemyList[enemyIndex].BoundingBox, enemyList[enemyIndex].CollisionMask))  //per pixel collision to check if they Actually collided
                            {
                                //subtract bullet power from enemy health
                                //then remove the bullet and increase player score and shield
                                enemyList[enemyIndex].Health -= playerBullets[bulletIndex].Damage;
                                playerBullets[bulletIndex].FlaggedForRemoval = true;
                                player.Score += 5;
                                player.RestoreShield(5);
                            }
                        }//end if
                    }
                    #endregion
                    #region Rotated Collision
                    else //changed from: catch //(InvalidOperationException ex)    //this exception will be thrown if we tried to get the bounding box of an enemy that had a rotation
                    {                                       //(normal bounding box will not work with rotation)
                        if (Distance(playerBullets[bulletIndex].Location, enemyList[enemyIndex].Location) < 100)
                        {
                            Matrix bulletTransform = Matrix.CreateTranslation(new Vector3(playerBullets[bulletIndex].Location, 0.0f));
                            Matrix enemyTransform = enemyList[enemyIndex].Rotation;

                            //try the other overload for PerPixel collision, seeing as how the last one threw this exception
                            if (Collision.PerPixel(playerBullets[bulletIndex].Texture, bulletTransform, enemyList[enemyIndex].CollisionMask, enemyTransform))
                            {
                                //subtract bullet power from enemy health
                                //then remove the bullet and increase player score and shield
                                enemyList[enemyIndex].Health -= playerBullets[bulletIndex].Damage;
                                playerBullets[bulletIndex].FlaggedForRemoval = true;
                                player.Score += 5;
                                player.RestoreShield(5);
                            }
                        }
                    #endregion
                    }
                }//end bullet-enemy collision check

                #region Standard Collision
                if (enemyList[enemyIndex].FloatRotation == 0)
                {
                    if (Collision.BoundingBox(player.BoundingBox, enemyList[enemyIndex].BoundingBox))  //check if bounding box of enemy inersects player's bounding box
                    {   //if the bounding boxes intersect, do perPixel collision
                        if (Collision.PerPixel(player.BoundingBox, player.CollisionMask, enemyList[enemyIndex].BoundingBox, enemyList[enemyIndex].CollisionMask)) //width and texture passed sepaerately because multiple frames in one texture
                        {
                            if (!player.IsShielding)
                            {
                                //hurt player if not shielding
                                player.Health -= 5;
                            }
                            //hurt enemy
                            enemyList[enemyIndex].Health -= 10;
                            vibrate.Start(TimeSpan.FromMilliseconds(100));
                        }
                    }//end enemy-player collision check
                }
                #endregion
                #region Rotated Collision
                else //changed from: catch //(InvalidOperationException ex)    //this exception will be thrown if we tried to get the bounding box of an enemy that had a rotation
                {                                       //(normal bounding box will not work with rotation)
                    if (Distance(player.Centre, enemyList[enemyIndex].Location) < 100)
                    {
                        Matrix playerTransform = Matrix.CreateTranslation(new Vector3(player.Location, 0.0f));
                        Matrix enemyTransform = enemyList[enemyIndex].Rotation;

                        //try the other overload for PerPixel collision, seeing as how the last one threw this exception
                        if (Collision.PerPixel(player.CollisionMask, playerTransform, enemyList[enemyIndex].CollisionMask, enemyTransform))
                        {
                            if (!player.IsShielding)
                            {
                                //hurt player if not shielding
                                player.Health -= 5;
                            }
                            //hurt enemy
                            enemyList[enemyIndex].Health -= 10;
                            vibrate.Start(TimeSpan.FromMilliseconds(100));
                        }
                    }
                }
                #endregion
            }//finished enemy collision checking

            for (int i = 0; i < enemyBullets.Count; i++)
            {
                //    if (enemyBullets[i] != null) --> no longer using array (see top of class)
                //    {
                if (Collision.BoundingBox(player.BoundingBox, enemyBullets[i].BoundingBox))  //check if bounding boxes intersect
                {
                    if (Collision.PerPixel(enemyBullets[i].BoundingBox, enemyBullets[i].Texture, playerOne.BoundingBox, playerOne.CollisionMask))  //per pixel collision to check if they Actually collided
                    {
                        if (!player.IsShielding)
                        {
                            //subtract bullet power from health
                            //then remove the bullet
                            player.Health -= enemyBullets[i].Damage;
                            enemyBullets[i].FlaggedForRemoval = true;
                            player.RestoreShield(5); //intentional
                        }

                        //create a particle effect to show that the the bullet has it the player
                        particleEffectsList.Add(new Explosion(player.Centre, 5, 2, 2, time));
                    }//end if
                }//end if
                //   }//end null check
            }//finished enemyBullet - player collision
        }//end HandleAllCollision()
        #endregion

        private double Distance(Vector2 locationA, Vector2 locationB)
        {
            double distance;

            double deltaX, deltaY;
            deltaX = locationB.X - locationA.X;
            deltaY = locationB.Y - locationA.Y;

            distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            return distance;
        }
    }
}
