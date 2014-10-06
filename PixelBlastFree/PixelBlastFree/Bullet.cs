using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PixelBlastFree
{
    class Bullet
    {
        protected Texture2D texture;
        protected Vector2 location;
        protected sbyte direction;    //-1 = up, 1= down, 0 = no movement(shouldnt happen)
        protected const byte MaxSpeed = 10;
        protected sbyte damage;
        protected bool flagRemove;    //whether or not to remove this object at the end of Game1.Update()

        #region CONSTRUCTOR(S)
        public Bullet(Vector2 startPos, Texture2D tex, sbyte yChoice)
        {
            location = startPos;
            direction = yChoice;
            texture = tex;
            damage = 100;
        }
        #endregion

        #region METHODS
        public virtual void Update()
        {
            location.Y += direction * MaxSpeed;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, location, Color.White);
        }
        #endregion

        #region PROPERTIES
        public sbyte Damage
        {
            get { return damage; }
            set { damage = value; }
        }
        public Rectangle BoundingBox
        {
            get { return new Rectangle((int)location.X, (int)location.Y, texture.Width, texture.Height); }
            //no set - meant of bounding box collision
        }
        public Texture2D Texture
        {
            get { return texture; }
            //no set - only meant for per pixel collision
        }
        public Vector2 Location
        {
            get { return location; }
            set { location = value; }
        }
        #endregion

        public bool FlaggedForRemoval
        {
            get { return flagRemove; }
            set { flagRemove = value; }
        }
    }
}
