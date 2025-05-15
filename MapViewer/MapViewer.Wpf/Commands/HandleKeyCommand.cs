using MapViewer.Core.Commands;
using MapViewer.Core.Models;
using MapViewer.Core.ViewModels;
using MapViewer.Wpf.EventArgs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace MapViewer.Wpf.Commands
{

    /// <summary>
    /// Command for handling all map keyboard inputs.
    /// </summary>
    /// <param name="mapViewModel">ViewModel for the map view.</param>
    public class HandleKeyCommand(MapViewModel mapViewModel) : BaseCommand
    {

        /// <summary>
        /// Rotation step in degrees.
        /// </summary>
        private readonly float ROTATION_SENSITIVITY = 0.1f;
        
        /// <summary>
        /// Movement step in map units.
        /// </summary>
        private readonly float MOVEMENT_SENSITIVITY = 1f;

        public override void Execute(object? parameter)
        {
            // Return if missing necessary data
            if (parameter == null || mapViewModel.Camera == null || mapViewModel.Map == null || mapViewModel.Map.Data == null)
            {
                return;
            }

            // Get KeyEventArgs if available
            try
            {
                KeyEventArgs keyEventArgs = (KeyEventArgs)parameter;
                Camera camera = mapViewModel.Camera;
                if (Keyboard.IsKeyDown(Key.A)) camera = HorizontalMove(camera, -MOVEMENT_SENSITIVITY);
                if (Keyboard.IsKeyDown(Key.D)) camera = HorizontalMove(camera, MOVEMENT_SENSITIVITY);
                if (Keyboard.IsKeyDown(Key.S)) camera = LateralMove(camera, -MOVEMENT_SENSITIVITY);
                if (Keyboard.IsKeyDown(Key.W)) camera = LateralMove(camera, MOVEMENT_SENSITIVITY);
                if (Keyboard.IsKeyDown(Key.Q)) camera = VerticalMove(camera, -MOVEMENT_SENSITIVITY);
                if (Keyboard.IsKeyDown(Key.E)) camera = VerticalMove(camera, MOVEMENT_SENSITIVITY);
                if (Keyboard.IsKeyDown(Key.Up)) camera = RotateAroundHorizontalAxis(camera, ROTATION_SENSITIVITY);
                if (Keyboard.IsKeyDown(Key.Down)) camera = RotateAroundHorizontalAxis(camera, -ROTATION_SENSITIVITY);
                if (Keyboard.IsKeyDown(Key.Right)) camera = RotateAroundVerticalAxis(camera, ROTATION_SENSITIVITY);
                if (Keyboard.IsKeyDown(Key.Left)) camera = RotateAroundVerticalAxis(camera, -ROTATION_SENSITIVITY);
                mapViewModel.Camera = camera;
                keyEventArgs.Handled = true;
            }
            catch (InvalidCastException e)
            {
                Debug.WriteLine($"AddCircleCommand called with unsupported parameter. {e}");
                return;
            }
        }

        /// <summary>
        /// Horizontal move of the camera.
        /// </summary>
        /// <param name="camera">Previous camera state.</param>
        /// <param name="amount">Distance to move.</param>
        private static Camera HorizontalMove(Camera camera, float amount)
        {
            Vector3 horizontalDirection = Vector3.Cross(camera.LookDirection, camera.UpDirection);
            return new Camera(
                    camera.Position + amount * horizontalDirection,
                    camera.LookDirection,
                    camera.UpDirection,
                    camera.Width
                );
        }

        /// <summary>
        /// Vertical move of the camera.
        /// </summary>
        /// <param name="camera">Previous camera state.</param>
        /// <param name="amount">Distance to move.</param>
        private static Camera VerticalMove(Camera camera, float amount)
        {
            return new Camera(
                    camera.Position + amount * camera.UpDirection,
                    camera.LookDirection,
                    camera.UpDirection,
                    camera.Width
                );
        }

        /// <summary>
        /// Lateral move of the camera.
        /// </summary>
        /// <param name="camera">Previous camera state.</param>
        /// <param name="amount">Distance to move.</param>
        private static Camera LateralMove(Camera camera, float amount)
        {
            return new Camera(
                    camera.Position + amount * camera.LookDirection,
                    camera.LookDirection,
                    camera.UpDirection,
                    camera.Width
                );
        }


        /// <summary>
        /// Rotate the camera around horizontal camera axis.
        /// </summary>
        /// <param name="camera">Previous camera state.</param>
        /// <param name="amount">Angle in degrees to rotate.</param>
        private static Camera RotateAroundHorizontalAxis(Camera camera, float amount)
        {
            Vector3 axis = Vector3.Cross(camera.LookDirection, camera.UpDirection);
            return new Camera(
                    camera.Position,
                    RotateVectorAroundAxis(camera.LookDirection, axis, amount),
                    RotateVectorAroundAxis(camera.UpDirection, axis, amount),
                    camera.Width
                );
        }


        /// <summary>
        /// Rotate the camera around vertical camera axis.
        /// </summary>
        /// <param name="camera">Previous camera state.</param>
        /// <param name="amount">Angle in degrees to rotate.</param>
        private static Camera RotateAroundVerticalAxis(Camera camera, float amount)
        {   
            return new Camera(
                    camera.Position,
                    RotateVectorAroundAxis(camera.LookDirection, camera.UpDirection, amount),
                    camera.UpDirection,
                    camera.Width
                );
        }


        /// <summary>
        /// Rotate <paramref name="vector"/> by <paramref name="angle"/> around an <paramref name="axis"/>.
        /// This method does expect <paramref name="vector"/> to start at the origin.
        /// </summary>
        /// <param name="vector">Vector to rotate.</param>
        /// <param name="axis">Axist to ratate around.</param>
        /// <param name="angle">Angle to rotate in degrees.</param>
        private static Vector3 RotateVectorAroundAxis(Vector3 vector, Vector3 axis, float angle)
        {
            // Rodrigues' rotation formula
            Vector3 vXa = Vector3.Cross(axis, vector);
            Vector3 vXvXa = Vector3.Cross(axis, vXa);
            return vector 
                + (float)(Math.Sin(angle * Math.PI / 180)) * vXa 
                + (float)(1 - Math.Cos(angle * Math.PI / 180)) * vXvXa;
        }

    }
}
