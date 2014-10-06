using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PixelBlastFree
{
    /// <remarks>
    /// Contains any and all cross-project collision detection methods that I use.
    /// All methods are static and the class is abstract.
    /// Methods should be called like: "Collision.PerPixel(...)" inside conditional statements.
    /// </remarks>
    public abstract class Collision
    {
        /// <summary>
        /// Checks if two rectangles or bounds are intersecting.
        /// Quicker than PerPixel but not as accurate.
        /// </summary>
        public static bool BoundingBox(Rectangle recOne, Rectangle recTwo)
        {
            bool collision = false;

            if (recOne.Intersects(recTwo))
                collision = true;

            return collision;
        }

        /// <summary>
        /// Checks collision at pixel level. If two non-transparant pixels overlap, then collision occurs.
        /// Slower than bounding box but much more accurate.
        /// Only checks the intersecting area of the textures.
        /// </summary>
        /// <param name="boundsA">the bounding box of first texture</param>
        /// <param name="boundsB">bounding box of second texture</param>
        public static bool PerPixel(Rectangle boundsA, Texture2D textureA, Rectangle boundsB, Texture2D textureB)
        {
            bool collision = false;

            //set up color arrays to hold data for each pixel of both textures
            Color[] colourInfoA = new Color[textureA.Width * textureA.Height];
            Color[] colourInfoB = new Color[textureB.Width * textureB.Height];

            //populate the arrays with the info
            textureA.GetData<Color>(colourInfoA);
            textureB.GetData<Color>(colourInfoB); 


            //find intersecting rectangle
            int top = Math.Max(boundsA.Top, boundsB.Top);
            int bottom = Math.Min(boundsA.Bottom, boundsB.Bottom);
            int left = Math.Max(boundsA.Left, boundsB.Left);
            int right = Math.Min(boundsA.Right, boundsB.Right);

            //check transparency of corresponding pixel in each texture
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    //get colour of both pixels at this position
                    Color colorA = colourInfoA[(x - boundsA.Left) + (y - boundsA.Top) * boundsA.Width];
                    Color colorB = colourInfoB[(x - boundsB.Left) + (y - boundsB.Top) * boundsB.Width];

                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        //if both colour are NOT transparent, we have collision!
                        collision =  true;
                    }
                }//end for
            }//end for
            return collision;
        }

        /// <summary>
        /// Checks colliison at a per-pixel level for two rotated texture2Ds.
        /// </summary>
        /// <param name="textureA">the first texture</param>
        /// <param name="transformA">the matrix transform of the first texture</param>
        /// <param name="textureB">the second texture</param>
        /// <param name="transformB">the matricx transfrom of the second texture</param>
        public static bool PerPixel(Texture2D textureA, Matrix transformA, Texture2D textureB, Matrix transformB)
        {
            bool collision = false;

            #region Set up colour arrays
            //set up color arrays to hold data for each pixel of both textures
            Color[] colourInfoA = new Color[textureA.Width * textureA.Height];
            Color[] colourInfoB = new Color[textureB.Width * textureB.Height];

            //populate the arrays with the info
            textureA.GetData<Color>(colourInfoA);
            textureB.GetData<Color>(colourInfoB); 
            #endregion

            //get a matrix that allows us to think of A in relation to B
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            int xA, yA, xB, yB; //current pixels for textureA and textureB respectively
            Vector2 pixelLocationInTextureB;   //the location of the current pixel from textureA, in terms of textureB

            //for every row of pixels in textureA
            for (yA = 0; yA < textureA.Height; yA++)
            {
                //every pixel in the row
                for (xA = 0; xA < textureA.Width; xA++)
                {
                    //get this pixel's location in relation to B
                    pixelLocationInTextureB = Vector2.Transform(new Vector2(xA, yA), transformAToB);

                    #region Round to nearest pixel and check if inside bounds, then check transparency
                    //round to nearest pixel
                    xB = (int)Math.Round(pixelLocationInTextureB.X);
                    yB = (int)Math.Round(pixelLocationInTextureB.Y);

                    //does pixel lie inside bounds of B?
                    if (0 <= xB && xB < textureB.Width && 0 <= yB && yB < textureB.Height)
                    {
                        #region Get pixel info and check transparency
                        //get colours of overlapping pixels
                        Color a = colourInfoA[xA + yA * textureA.Width];
                        Color b = colourInfoB[xB + yB * textureB.Width];

                        if (a.A != 0 && b.A != 0) //if BOTH colors are NOT transparent, then there is collision
                        {
                            collision = true;
                            break;
                        }//end if(!transparent) 
                        #endregion
                    }//end if(inside bounds B) 
                    #endregion
                }//end for(xA)
            }//end for(yA)

            return collision;
        }//end method
    }
}
