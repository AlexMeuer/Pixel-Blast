using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PixelBlastFree
{
    public abstract class ModifyTexture2D
    {
        /// <summary>
        /// Crops a source texture to a specified rectangle.
        /// Does NOT change original texture.
        /// </summary>
        /// <param name="sourceTexture">The texture to be cropped</param>
        /// <param name="cropRectangle">The area to crop the texture to</param>
        /// <returns></returns>
        public static Texture2D Crop(Texture2D sourceTexture, Rectangle cropRectangle)
        {
            //create a new texture the size of the cropRectangle
            Texture2D croppedTexture = new Texture2D(sourceTexture.GraphicsDevice, cropRectangle.Width, cropRectangle.Height);

            //create two arrays to hold the color data of the original texture and the new texture
            Color[] sourceData = new Color[sourceTexture.Width * sourceTexture.Height];  
            Color[] cropData = new Color[croppedTexture.Width * croppedTexture.Height];  
  
            //pass the source color data to the source color array
            sourceTexture.GetData<Color>(sourceData);  
  
            //for every pixel in the new texture, take source color data and pass it to the cropped array
            //(at this point we already have the dimensions of the new texture and we want the color now)
            int index = 0;
            for (int y = cropRectangle.Y; y < cropRectangle.Y + cropRectangle.Height; y++)  //all y values in new texture
            {  
                for (int x = cropRectangle.X; x < cropRectangle.X + cropRectangle.Width; x++)   //all x values in new texture
                {  
                    cropData[index] = sourceData[x + (y * sourceTexture.Width)];  //pass the color data from sourceData to cropData
                    index++;//go to the next position on the croppedData array
                }  
            }
  
            croppedTexture.SetData<Color>(cropData);    //apply the new color data to the blank cropped texture

            return croppedTexture;
        }

        /// <summary>
        /// Crops a texture to a specified rectangle.
        /// The original texture IS MODIFIED!
        /// </summary>
        /// <param name="sourceTexture">The texture to be cropped</param>
        /// <param name="cropRectangle">The rectangle used to crop the texture</param>
        /// <returns></returns>
        public static Texture2D PermaCrop(ref Texture2D sourceTexture, Rectangle cropRectangle)
        {
            //create a new texture the size of the cropRectangle
            Texture2D croppedTexture = new Texture2D(sourceTexture.GraphicsDevice, cropRectangle.Width, cropRectangle.Height);

            //create two arrays to hold the color data of the original texture and the new texture
            Color[] sourceData = new Color[sourceTexture.Width * sourceTexture.Height];
            Color[] cropData = new Color[croppedTexture.Width * croppedTexture.Height];

            //pass the source color data to the source color array
            sourceTexture.GetData<Color>(sourceData);

            //for every pixel in the new texture, take source color data and pass it to the cropped array
            //(at this point we already have the dimensions of the new texture and we want the color now)
            int index = 0;
            for (int y = cropRectangle.Y; y < cropRectangle.Y + cropRectangle.Height; y++)  //all y values in new texture
            {
                for (int x = cropRectangle.X; x < cropRectangle.X + cropRectangle.Width; x++)   //all x values in new texture
                {
                    cropData[index] = sourceData[x + (y * sourceTexture.Width)];  //pass the color data from sourceData to cropData
                    index++;//go to the next position on the croppedData array
                }
            }

            croppedTexture.SetData<Color>(cropData);    //apply the new color data to the blank cropped texture

            sourceTexture = croppedTexture;
            return croppedTexture;  //return the texture in case it is used in a statement/assignment
        }

        /// <summary>
        /// Rotates a 2D texture by a given angle. Intended for collision purposes.
        /// Rotation for drawing purposes should instead be done with SpriteBatch.Draw();
        /// -------
        /// DOES NOT MODIFY ORIGINAL TEXTURE
        /// </summary>
        /// <param name="texture">texture to rotate</param>
        /// <param name="angleInRadians">angle to rotate the image by, given in radians</param>
        /// <returns></returns>
        //public static Texture2D Rotate(Texture2D texture, float angleInRadians)
        //{
        //    Texture2D rotatedTexture;

        //    rotatedTexture = Matrix.CreateRotationZ(angleInRadians);

        //    return rotatedTexture;
        //}
    }
}
