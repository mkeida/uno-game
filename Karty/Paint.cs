using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UNO
{
    /// <summary>
    /// Help class
    /// </summary>
    public class Paint
    {
        /// <summary>
        /// Draws rounded rectangle
        /// </summary>
        public static GraphicsPath RoundedRectangle(Rectangle bounds, int radius)
        {
            // Variables
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            // Returns plain rectangle if corner radius is zero
            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // Top left arc  
            path.AddArc(arc, 180, 90);

            // Top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // Bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // Bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            // Closes path
            path.CloseFigure();

            // Returns path
            return path;
        }

        /// <summary>
        /// Returns ellipse. Zero position is center instead of top left corner.
        /// </summary>
        public static GraphicsPath Ellipse(Point center, int width, int height)
        {
            // Creates new path
            GraphicsPath path = new GraphicsPath();

            // Calculate ellipse path
            path.AddEllipse(new Rectangle(center.X - width, center.Y - height, width * 2, height * 2));

            // Returns path
            return path;
        }
    }
}
