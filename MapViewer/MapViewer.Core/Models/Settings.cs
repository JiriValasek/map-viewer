using MapViewer.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewer.Core.Models
{
    public class Settings : BaseViewModel
    {
        // UI settings
        public Color MinAltitudeColor { get; }
        public Color MaxAltitudeColor { get; }
        public Color CircleColor { get; }
        public Color CenterColor { get; }

        // Models settings
        public float LineWidth { get; }
        public float CenterSize { get; }
        public int SegmentCount { get; }

        // Controls settings
        public float ZoomSensitivity { get; }
        public float RotationStep { get; }
        public float MovementStep { get; }

        public Settings(
            Color minAltitudeColor,
            Color maxAltitudeColor,
            Color circleColor,
            Color centerColor,
            float lineWidth,
            float centerSize,
            int segmentCount,
            float zoomSensitivity,
            float rotationStep,
            float movementStep
            )
        {
            MinAltitudeColor = minAltitudeColor;
            MaxAltitudeColor = maxAltitudeColor;
            CircleColor = circleColor;
            CenterColor = centerColor;
            LineWidth = lineWidth;
            CenterSize = centerSize;
            SegmentCount = segmentCount;
            ZoomSensitivity = zoomSensitivity;
            RotationStep = rotationStep;
            MovementStep = movementStep;
        }

        public Settings(
            string minAltitudeColor,
            string maxAltitudeColor,
            string circleColor,
            string centerColor,
            string lineWidth,
            string centerSize,
            string segmentCount,
            string zoomSensitivity,
            string rotationStep,
            string movementStep
            )
        {
            MinAltitudeColor = ConvertStringToColor(minAltitudeColor);
            MaxAltitudeColor = ConvertStringToColor(maxAltitudeColor);
            CircleColor = ConvertStringToColor(circleColor);
            CenterColor = ConvertStringToColor(centerColor);
            LineWidth = Single.Parse(lineWidth);
            CenterSize = Single.Parse(centerSize);
            SegmentCount = Int32.Parse(segmentCount);
            ZoomSensitivity = Single.Parse(zoomSensitivity);
            RotationStep = Single.Parse(rotationStep);
            MovementStep = Single.Parse(movementStep);

        }

        private Color ConvertStringToColor(string colorString)
        {
            Color color = Color.FromName(colorString); // Does not throw error for invalid names
            if (color.IsKnownColor) return color;
            int a, r, g, b;
            switch (colorString.Length)
            {
                case 3:
                    a = 0xff;
                    r = 0xff * Convert.ToInt32(colorString[0].ToString(), 16) / 0xf;
                    g = 0xff * Convert.ToInt32(colorString[1].ToString(), 16) / 0xf;
                    b = 0xff * Convert.ToInt32(colorString[2].ToString(), 16) / 0xf;
                    break;
                case 4:
                    a = 0xff * Convert.ToInt32("0x" + colorString[0], 16) / 0xf;
                    r = 0xff * Convert.ToInt32("0x" + colorString[1], 16) / 0xf;
                    g = 0xff * Convert.ToInt32("0x" + colorString[2], 16) / 0xf;
                    b = 0xff * Convert.ToInt32("0x" + colorString[3], 16) / 0xf;
                    break;
                case 6:
                    a = 0xff;
                    r = Convert.ToInt32("0x" + colorString.Substring(0, 2), 16);
                    g = Convert.ToInt32("0x" + colorString.Substring(2, 2), 16);
                    b = Convert.ToInt32("0x" + colorString.Substring(4, 2), 16);
                    break;
                case 8:
                    a = Convert.ToInt32("0x" + colorString.Substring(0, 2), 16);
                    r = Convert.ToInt32("0x" + colorString.Substring(2, 2), 16);
                    g = Convert.ToInt32("0x" + colorString.Substring(4, 2), 16);
                    b = Convert.ToInt32("0x" + colorString.Substring(6, 2), 16);
                    color = Color.FromArgb(a, r, g, b); // Does throw errors for invalid values
                    break;
                default:
                    a = -1;
                    r = -1;
                    g = -1;
                    b = -1;
                    break;
            }
            color = Color.FromArgb(a, r, g, b);
            return color;
        }

    }
}
