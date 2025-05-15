using MapViewer.Core.Commands;
using MapViewer.Core.Models;
using MapViewer.Core.Stores;
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
    public class HandleKeyCommand(SettingsStore settingsStore, MapViewModel mapViewModel) : BaseCommand
    {

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
                switch (keyEventArgs.Key)
                {
                    case Key.A: 
                        HorizontalMove(mapViewModel.Camera, -settingsStore.Settings.MovementStep);
                        break;
                    case Key.D:
                        HorizontalMove(mapViewModel.Camera, settingsStore.Settings.MovementStep);
                        break;
                    case Key.S:
                        LateralMove(mapViewModel.Camera, -settingsStore.Settings.MovementStep);
                        break;
                    case Key.W:
                        LateralMove(mapViewModel.Camera, settingsStore.Settings.MovementStep);
                        break;
                    case Key.Q:
                        VerticalMove(mapViewModel.Camera, -settingsStore.Settings.MovementStep);
                        break;
                    case Key.E:
                        VerticalMove(mapViewModel.Camera, settingsStore.Settings.MovementStep);
                        break;
                    case Key.Up:
                        RotateAroundHorizontalAxis(mapViewModel.Camera, settingsStore.Settings.RotationStep);
                        break;
                    case Key.Down:
                        RotateAroundHorizontalAxis(mapViewModel.Camera, -settingsStore.Settings.RotationStep);
                        break;
                    case Key.Right:
                        RotateAroundVerticalAxis(mapViewModel.Camera, settingsStore.Settings.RotationStep);
                        break;
                    case Key.Left:
                        RotateAroundVerticalAxis(mapViewModel.Camera, -settingsStore.Settings.RotationStep);
                        break;
                }
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
        private void HorizontalMove(Camera camera, float amount)
        {
            Vector3 horizontalDirection = Vector3.Cross(camera.LookDirection, camera.UpDirection);
            mapViewModel.Camera = new Camera(
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
        private void VerticalMove(Camera camera, float amount)
        {
            mapViewModel.Camera = new Camera(
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
        private void LateralMove(Camera camera, float amount)
        {
            mapViewModel.Camera = new Camera(
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
        private void RotateAroundHorizontalAxis(Camera camera, float amount)
        {
            Vector3 axis = Vector3.Cross(camera.LookDirection, camera.UpDirection);
            mapViewModel.Camera = new Camera(
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
        private void RotateAroundVerticalAxis(Camera camera, float amount)
        {   
            mapViewModel.Camera = new Camera(
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
