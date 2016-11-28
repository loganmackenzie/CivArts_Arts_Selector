/*
    The Civ-Arts Arts Selector is a picture study tool to help people memorize pieces of art.
    Copyright (C) {2016}  {Logan A. MacKenzie}

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 * Project: Civ-Arts Arts Selector
 * Author: Logan MacKenzie
 * Date:  May 2016 - June 2016
 * Description: The Civ-Arts Arts Selector is a study tool to assist in studying
 *          pieces of art. Specifically, this tool will assist in identifying 
 *          specific pieces of art allowing the user to better remember them.
 * 
 * Updated: November 26, 2016
 * Updater: Logan MacKenzie
 * Description: The original Civ-Arts Arts Selector simply drew images from a 
 *          pre-prepared folder of images. This update added additional code 
 *          permitted the user to chose specific images from the folder to study.
 *          This allows for studying for specific tests, before having been 
 *          introduced to all the images.
 *              Additionally, this update allows the user to find and modify the 
 *          the folder to suite the specific class.
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CivArts_ArtsSelector
{
    public partial class Form1 : Form
    {
        Random random = new Random(); // Random number generator. Time dependent seed
        Image image;         // Holds the current image to display
        Rectangle rectangle; // Outlines the selection
        string[] dir;        // Holds the list of files in the Pictures folder.
        int current_image;   // Holds the current index in the directory of the current image.

        public Form1()
        {
            InitializeComponent(); // Initialize buttons, picture boxes, etc.
            
            // Get a listing of all the images in the Pictures directory!
            dir = System.IO.Directory.GetFiles("Pictures");  

            // Call next_Click to initialize the image and display a segment
            next_Click(new Object(), new EventArgs()); 
        }

        // next_Click randomly chooses the next image and reads it in from the file. 
        // It randomly selects a portion of the image to display and clears the title and full-image picture box.
        private void next_Click(object sender, EventArgs e)
        {
            // Maximaze the pictureBox before inserting image.
            pictureBox1.Height = splitContainer2.Height; 
            pictureBox1.Width = splitContainer2.SplitterDistance;

            // Randomly choose a image and import it.
            current_image = random.Next(dir.Length);    // Choose a random image
            image = Image.FromFile(dir[current_image]); // Import from the file.
            
            // Randomly choose a width and a starting location. The height is proportional to the width. 
            rectangle.Width = Convert.ToInt32(image.Width / (2 * (random.NextDouble() + 1)));
            rectangle.Height = Convert.ToInt32(image.Height * rectangle.Width / image.Width);
            rectangle.X = random.Next(image.Width - rectangle.Width - image.Width / 250 - 1); // Leave some space around the edges to display the rectangle
            rectangle.Y = random.Next(image.Height - rectangle.Height - image.Width / 250 - 1); // Leave some space around the edges to display the rectangle

            // Display the selected region of the image
            Bitmap sourceBitmap = new Bitmap(image); // Convert the image to Bitmap and then clone the selection into the pictureBox.
            pictureBox1.Image = sourceBitmap.Clone(rectangle, sourceBitmap.PixelFormat);

            // Hide the full image picture box and the title.
            pictureBox2.Visible = false; // Hide the full-image box.
            title_label.Text = "";       // Clear title of work
            
            // Collect garbage. The images are large and can crash the program
            // If care is not taken to clean up frequently.
            GC.Collect();   // Clean up the garbage (makes sure the program does not run out of memory).
        }

        // Displays the title of the piece of art.
        // show_title_Click gets the image title (just the filename) for
        // the current image. It then removes the file path (if present)
        // and displays the title along the bottom of the screen.
        private void show_title_Click(object sender, EventArgs e)
        {
            string long_title = dir[current_image];
            int pos = long_title.LastIndexOf('\\');
            //int pos = long_title.IndexOf("Pictures\\");
            if (pos == -1)
                title_label.Text = long_title;
            else
                title_label.Text = long_title.Remove(0, pos + 1); // Remove the file path from the name.
        }

        // Displays the full image 
        // show_image_Click just reveals the entire image allowing the user to 
        // study it and figure out where the selection was taken from.
        private void show_image_Click(object sender, EventArgs e)
        {
            // Reset the size of the picture box in case it was shrunk for 
            // a previous image.
            pictureBox2.Height = splitContainer2.Height;
            pictureBox2.Width = splitContainer2.Width - splitContainer2.SplitterDistance;
            // Display the image.
            pictureBox2.Image = image;
            pictureBox2.Visible = true;
        }

        // Displays where the selection was taken from the image.
        // mark_location_Click extracts a Bitmap image from the original image
        // and calls AddRectangle to show where the selected portion was taken
        // from. Then it makes pictureBox2 visible to display the entire image.
        private void mark_location_Click(object sender, EventArgs e)
        {
            Bitmap sourceBitmap = new Bitmap(image);
            pictureBox2.Image = AddRectangle(sourceBitmap, rectangle);
            pictureBox2.Visible = true;
            GC.Collect(); // Clean up (or else the program crashes after running out of memory)
        }

        // Sketches the borders of the selected rectangle of the image.
        // AddRectangle essetially sketches the border in red by changing every pixel along
        // the borders to be red. This method was chosen over just sketching a rectangle on
        // the image because scaling the rectangle was difficult to keep consistent with 
        // changing screen sizes.
        private Bitmap AddRectangle(Bitmap bmap, Rectangle rect)
        {
            int line_width = bmap.Width / 250; // Scale border width to size of image so it appears consistent.
            for (int i = 0; i < line_width; i++)
            {
                for (int x = rect.X; x < rect.X + rect.Width; x++) // Top and bottom borders
                {   // Set all the pixels along the top and bottom borders to be red.
                    bmap.SetPixel(x, rect.Y+i, Color.Red); // Top border
                    bmap.SetPixel(x, rect.Y+i + rect.Height, Color.Red); // Bottom border
                }
                for (int y = rect.Y; y < rect.Y + rect.Height; y++) // Left and right borders
                {   // Set all the pixels along the left and right borders to be red.
                    bmap.SetPixel(rect.X + i, y, Color.Red); // Left border
                    bmap.SetPixel(rect.X + i + rect.Width, y, Color.Red); // Right border
                }
            }
            return bmap;
        }

        // select_images_button_Click opens a standard Windows file explorer window
        // allowing the user to select all the desired images to study.
        private void select_images_button_Click(object sender, EventArgs e)
        {
            picture_file_selector.InitialDirectory = System.IO.Directory.GetCurrentDirectory() + "\\Pictures";
            picture_file_selector.ShowDialog();
            next_Click(new Object(), new EventArgs()); // Call next_Click to select a new picture.
            GC.Collect();
        }

        // After the user has selected all the desired images and clicks Ok, this
        // function runs. picture_file_selector_FileOk extracts the file names selected
        // by the user and appends "Pictures\\" to them so the program can find the
        // pictures in their folder. It sets the pictures directory from which all
        // images are drawn to consist of the file selected.
        // Note: This only works within the pre-supplied folder due to the folder path.
        private void picture_file_selector_FileOk(object sender, CancelEventArgs e)
        {
            OpenFileDialog opfldlg = (OpenFileDialog)sender;
            dir = opfldlg.SafeFileNames; // Get the selected image file names.
            for (int i = 0; i < opfldlg.SafeFileNames.Length; i++)
            {   // Append the folder name on the front of the image file name.
                dir[i] = "Pictures\\" + dir[i];
            }
        }
    }
}
