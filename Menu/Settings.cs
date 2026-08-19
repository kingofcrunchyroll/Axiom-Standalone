using Axiom.Classes;
using Axiom.Menu;
using UnityEngine;

namespace Axiom
{
    public class Settings
    {
        /*
         * These are the settings for the menu.
         * 
         * To change the colors, you need to modify the ExtGradient variables.
         * Here are some examples on how to use ExtGradient:
         * 
         * Solid Color:
         *  new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) }
         *  
         * Simple Gradient:
         *  new ExtGradient { colors = ExtGradient.GetSimpleGradient(Color.black, Color.white) }
         * 
         * Rainbow Color:
         *   new ExtGradient { rainbow = true }
         *   
         * Epileptic Color (random color every frame):
         *   new ExtGradient { epileptic = true }
         *   
         * Self Color:
         *   new ExtGradient { copyRigColor = true }
         *   
         * To change the font, you may use the following code:
         *   Font.CreateDynamicFontFromOSFont("Comic Sans MS", 24)
         */

        public static ExtGradient backgroundColor = new ExtGradient { rainbow = true};
        public static ExtGradient[] buttonColors = new ExtGradient[]
        {
            new ExtGradient { colors = ExtGradient.GetSolidGradient(Color.black) }, // Disabled
            new ExtGradient { rainbow = true } // Enabled
        };
        public static ExtGradient[] textColors = {
            new ExtGradient // Title
            {
                colors = ExtGradient.GetSolidGradient(Color.white)
            },
            new ExtGradient // Button Released
            {
                colors = ExtGradient.GetSolidGradient(Color.white)
            },
            new ExtGradient // Button Clicked
            {
                colors = ExtGradient.GetSolidGradient(Color.white)
            }
        };

        public static Font currentFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

        public static bool fpsCounter = true;
        public static bool disconnectButton = true;
        public static bool rightHanded;
        public static bool disableNotifications;

        public static KeyCode keyboardButton = KeyCode.Q;

        public static Vector3 menuSize = new Vector3(0.1f, 1f, 1f); // Depth, width, height
        public static int buttonsPerPage = 8;

        public static float gradientSpeed = 0.5f; // Speed of colors

        public static int flySpeedIndex = 2;
        public static float flySpeed = 15f;

        public static void ChangeFlySpeed(bool increment)
        {
            string[] speedNames = new string[] { "Very Slow", "Slow", "Normal", "Fast", "Very Fast", "Extreme" };
            float[] speedValues = new float[] { 5f, 10f, 15f, 20f, 30f, 50f };

            if (increment)
                flySpeedIndex++;
            else
                flySpeedIndex--;

            flySpeedIndex %= speedNames.Length;

            Buttons.GetIndex("Change Fly Speed").overlapText = $"Change Fly Speed [{speedNames[flySpeedIndex]}]";
        }
    }
}
