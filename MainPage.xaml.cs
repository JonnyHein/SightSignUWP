using System;
using Windows.Devices.Input.Preview;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.Foundation;
using System.Collections.Generic;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using System.IO;
using Windows.UI.Input.Inking;
using Windows.UI.Input.Inking.Analysis;
using Windows.UI.Xaml.Shapes;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Data;
using System.Globalization;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SightSignUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private InkStroke _strokeBeingAnimated;
        private int _currentAnimatedStrokeIndex;
        private int _currentAnimatedPointIndex;
        public RobotArm RobotArm { get; }
        private readonly Settings _settings;
        private DispatcherTimer _dispatcherTimerDotAnimation;
        private bool _inTimer = false;

        private bool _stampInProgress;

        /// <summary>
        /// Initialize the app.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            // Set application window to full screen & get its dimensions.
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
            var bounds = Window.Current.Bounds;
            double appHeight = bounds.Height;
            double appWidth = bounds.Width;

            // instantiate Robot Arm & settings
            RobotArm = new RobotArm(
                appWidth / 2.0,
                appHeight / 2.0,
                Math.Min(appWidth, appHeight) / 2.0,
                new UArmSwiftPro());

            _settings = new Settings(RobotArm);
            DataContext = _settings;

            if (_settings.RobotControl)
            {
                RobotArm.Connect();
                RobotArm.ArmDown(false); // Lift the arm.
            }

            // set the default drawing attributes
            InkDrawingAttributes drawingAttributes = new InkDrawingAttributes();
            SetDrawingAttributesFromSettings(drawingAttributes);
            inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);

            inkCanvas.InkPresenter.InputDeviceTypes =
                    CoreInputDeviceTypes.Mouse |
                    CoreInputDeviceTypes.Pen |
                    CoreInputDeviceTypes.Touch;

            LoadInkOnStartup();
        }

        private void SetDrawingAttributesFromSettings(InkDrawingAttributes defaultDrawingAttributes)
        {
            defaultDrawingAttributes.Color = Windows.UI.Colors.DarkOliveGreen;

            // set Stroke Size through with same width and height
            Size strokeSize;
            strokeSize.Width = strokeSize.Height = _settings.InkWidth;
            defaultDrawingAttributes.Size = strokeSize;

            defaultDrawingAttributes.PenTip = PenTipShape.Circle;
        }



        /// <summary>
        /// There is more function in the WPF before this one...
        /// Just integrating this in so that it can load in a signature from a file.
        /// </summary>

        // Show the dot at the start of the ink, and when that's clicked, animate the dot through
        // the entire signature, sending the point data to the robot as the dot progresses.
        // TODO: Create the stamp button in the UI and connect it to this function.
        private void StampButton_Click(object sender, RoutedEventArgs e)
        {
            // Stop any in-progress wrting visuals.
            ResetWriting();

            _stampInProgress = true;

            WriteSignature();
        }

        ///<summary>
        /// Creating the Write button functionality [[Testing]]
        ///</summary>
        private void WriteButton_Click(object sender, RoutedEventArgs e)
        { 
            // Stop any in-progress writing.
            ResetWriting();

            WriteSignature();
        }
        
        // Resets visuals associated with dot animation and tracing out ink.
        // Note that this does not have any effect on the robot.
        private void ResetWriting()
        {
            _stampInProgress = false;

            if (_dispatcherTimerDotAnimation != null)
            {
                _dispatcherTimerDotAnimation.Stop();
                _dispatcherTimerDotAnimation = null;
            }

            _currentAnimatedPointIndex = 0;
            _currentAnimatedStrokeIndex = 0;

            inkCanvasAnimations.InkPresenter.StrokeContainer.Clear();
            inkCanvasAnimations.Visibility = Visibility.Collapsed;

            dot.Visibility = Visibility.Collapsed;

            foreach (var stroke in inkCanvas.InkPresenter.StrokeContainer.GetStrokes())
            {
                stroke.DrawingAttributes.Color = Windows.UI.ColorHelper.FromArgb(
                    _settings.InkColor.A,
                    _settings.InkColor.R,
                    _settings.InkColor.G,
                    _settings.InkColor.B);
            }
        }

        private void WriteSignature()
        {
            // TODO ::refactor this code later
            var inkStrokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();

            // ensure we actually have strokes, and we are not in the middle of a signature
            if (inkStrokes.Count == 0 || _dispatcherTimerDotAnimation != null)
            {
                return;
            }

            // making tracing dot visible now
            dot.Visibility = Visibility.Visible;
            dot.Opacity = 1.0;

            // Show the animation canvas and delete the previously-traced ink
            inkCanvasAnimations.Visibility = Visibility.Visible;
            inkCanvasAnimations.InkPresenter.StrokeContainer.Clear();

            // The dot moves form point to point along each stroke being traced out.
            _currentAnimatedPointIndex = 0;
            _currentAnimatedStrokeIndex = 0;

            // Lift robot arm
            RobotArm.ArmDown(false);

            // Move to the start of the signature
            var inkPointFirst = inkStrokes[0].GetInkPoints()[0];
            MoveDotAndRobotToInkPoint(inkPointFirst);

            // We'll create the animation stroke once the animation timer has fired.
            _strokeBeingAnimated = null;

            // Begin the timer used for the animation
            _dispatcherTimerDotAnimation = new DispatcherTimer();
            _dispatcherTimerDotAnimation.Tick += dispatcherTimerDotAnimation_Tick;
            _dispatcherTimerDotAnimation.Interval = new TimeSpan(0, 0, 0, 0, _settings.AnimationInterval);

            // TODO :: implement this function after the arm writes one of the strokes
            // TODO :: refactor this code.
            //using (IEnumerator<InkStroke> inkStrokesEnum = inkStrokes.GetEnumerator())
            //{
            //    while (inkStrokesEnum.MoveNext())
            //    {
            //        InkStroke stroke = inkStrokesEnum.Current;
            //        var inkPoints = stroke.GetInkPoints();
            //        MoveDotAndRobotToInkPoint(inkPoints[0]);
            //    }
            //}
        }

        private void MoveDotAndRobotToInkPoint(InkPoint inkPoint)
        {
            Point pt = inkPoint.Position;
            RobotArm.Move(pt);
        }

        private void dispatcherTimerDotAnimation_Tick(object sender, object e)
        {
            if (_inTimer)
            {
                return;
            }

            _inTimer = true;

            // Have we created a new stroke for this animation yet? 
            if (_strokeBeingAnimated == null)
            {
                // No, so create the first stroke and add the first dot to it.
                var firstPt = inkCanvas.InkPresenter.StrokeContainer.GetStrokes()[_currentAnimatedStrokeIndex].GetInkPoints()[0];

                RobotArm.ArmDown(true);

                AddFirstPointToNewStroke(firstPt);
            }

            // Move to the next point along the stroke.
            ++_currentAnimatedPointIndex;

            // Have we reached the end of a stroke? 
            if (_currentAnimatedPointIndex >= 
                inkCanvas.InkPresenter.StrokeContainer.GetStrokes().Count)
            {
                // If the stroke is really short, we'll not ask the user to click the dot
                // at both the start and end of the stroke. Instead once the dot is clicked
                // at the start of the stroke, it will animate to the end of it, and then
                // automatically move to the start of the next stroke.
                var shortStroke = (_currentAnimatedPointIndex < 3);

                // Should the dot automatically move to the start of the next stroke?
                if (_stampInProgress || shortStroke)
                {
                    // Yes, so the next animation will be at the start of the stroke.
                    _currentAnimatedPointIndex = 0;

                    // Move to the next stroke
                    ++_currentAnimatedStrokeIndex;

                    // Do we have any more strokes to write?
                    if (_currentAnimatedStrokeIndex < inkCanvas.InkPresenter.StrokeContainer.GetStrokes().Count)
                    {
                        // Yes. So move along to the start of the next stroke.
                        MoveToNextStroke();

                        // If we've completed a short stroke, and are to wait for the user
                        // to click the dot, make the dot opaque and wait for the click.
                        if (!_stampInProgress)
                        {
                            dot.Opacity = 1.0;

                            LiftArmAndStopAnimationTimer();
                        }
                    }
                    else
                    {
                        // We've reached the end of the last stroke.
                        _currentAnimatedStrokeIndex = 0;

                        // Hide the dot now that the entire signature's been written
                        dot.Visibility = Visibility.Collapsed;

                        LiftArmAndStopAnimationTimer();

                        _dispatcherTimerDotAnimation = null;

                        _stampInProgress = false;
                    }
                }
                else
                {
                    // The dot is to wait at the end of the stroke until it's clicked.
                    // So stop the animation timer.
                    LiftArmAndStopAnimationTimer();

                    // If we've not reached end of the last stroke, Show an opaque dot
                    // to indicate that it's waiting to be clicked.
                    if (_currentAnimatedStrokeIndex < inkCanvas.InkPresenter.StrokeContainer.GetStrokes().Count - 1)
                    {
                        dot.Opacity = 1.0;
                    }
                    else
                    {
                        // We've reached the end of the last stroke so hide the dot.
                        dot.Visibility = Visibility.Collapsed;

                        _dispatcherTimerDotAnimation = null;
                    }
                }
            }
            else
            {
                // We're continuing to animate the stroke that we are already on.

                var inkPt = inkCanvas.InkPresenter.StrokeContainer.GetStrokes()[_currentAnimatedStrokeIndex].GetInkPoints()[_currentAnimatedPointIndex];
                var inkPtPrevious = inkCanvas.InkPresenter.StrokeContainer.GetStrokes()[_currentAnimatedStrokeIndex].GetInkPoints()[_currentAnimatedPointIndex - 1];

                // Move to a point that's sufficiently far from the point that the dot's currently at.
                const int threshold = 1;

                while ((Math.Abs(inkPt.Position.X - inkPtPrevious.Position.X) < threshold) &&
                       (Math.Abs(inkPt.Position.Y - inkPtPrevious.Position.Y) < threshold))
                {
                    ++_currentAnimatedPointIndex;

                    if (_currentAnimatedPointIndex >= inkCanvas.InkPresenter.StrokeContainer.GetStrokes()[_currentAnimatedStrokeIndex].GetInkPoints().Count)
                    {
                        break;
                    }

                    inkPt = inkCanvas.InkPresenter.StrokeContainer.GetStrokes()[_currentAnimatedStrokeIndex].GetInkPoints()[_currentAnimatedPointIndex];
                }

                // Leave the arm in its current down state.
                MoveDotAndRobotToInkPoint(inkPt);

                // add all ink points from animated stroke to list
                List<InkPoint> updatedPtCollection = new List<InkPoint>();
                foreach (InkPoint point in _strokeBeingAnimated.GetInkPoints())
                {
                    updatedPtCollection.Add(point);
                }
                updatedPtCollection.Add(inkPt);  // append new inkPt to list

                // update animated stroke
                _strokeBeingAnimated = CreateStroke(updatedPtCollection);


                
                // Extend the ink stroke being drawn out to include the point where the dot is now.
                // TODO: FIGURE OUT HOW TO APPEND AN INK POINT TO THE INK POINT LIST OF THE STROKE (READONLY)
                
            }

            _inTimer = false;
        }

        private void LiftArmAndStopAnimationTimer()
        {
            _dispatcherTimerDotAnimation.Stop();

            RobotArm.ArmDown(false);
        }

        private void AddFirstPointToNewStroke(InkPoint pt)
        {
            // Create a new stroke for the continuing animation.
            var ptCollection = new List<InkPoint>();
            ptCollection.Add(pt);

            _strokeBeingAnimated = CreateStroke(ptCollection);

            SetDrawingAttributesFromSettings(_strokeBeingAnimated.DrawingAttributes);

            inkCanvasAnimations.InkPresenter.StrokeContainer.AddStroke(_strokeBeingAnimated);
        }

        // Create an InkStroke from a list of InkPoints.
        private InkStroke CreateStroke(List<InkPoint> ptCollection)
        {
            var strokeBuilder = new InkStrokeBuilder();
            var matrix = System.Numerics.Matrix3x2.Identity;
            return strokeBuilder.CreateStrokeFromInkPoints(ptCollection, matrix);
        }

        private void MoveToNextStroke()
        {
            // Move to the next stroke
            var stylusPtNext = 
                inkCanvas.InkPresenter.StrokeContainer.GetStrokes()[_currentAnimatedStrokeIndex].GetInkPoints()[_currentAnimatedPointIndex];

            // We'll create the animation stroke after the first interval.
            _strokeBeingAnimated = null;

            // Lift the arm up before moving the dot to the start of the next stroke.
            RobotArm.ArmDown(false);

            MoveDotAndRobotToInkPoint(stylusPtNext);
        }

        #region ButtonClickHandlers
        // When the Edit button is clicked, the user can ink directly in the app.
        // TODO: Create the buttons in the UI that are used in the commented code below.
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            ResetWriting();

            // TODO: Uncomment once buttons have been created in the UI
            /*if (StampButton.Visibility == Visibility.Visible)
            {
                EditButton.Content = "Done";

                StampButton.Visibility = Visibility.Collapsed;
                ClearButton.Visibility = Visibility.Visible;

                inkCanvas.IsEnabled = true;
            }
            else
            {
                EditButton.Content = "Edit";

                StampButton.Visibility = Visibility.Visible;
                ClearButton.Visibility = Visibility.Collapsed;

                inkCanvas.IsEnabled = false;
            }

            WriteButton.Visibility = StampButton.Visibility;

            SaveButton.Visibility = ClearButton.Visibility;
            LoadButton.Visibility = ClearButton.Visibility;*/
        }

        // Clear all ink from the app.
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.InkPresenter.StrokeContainer.Clear();
            inkCanvasAnimations.InkPresenter.StrokeContainer.Clear();
        }

        // Save whatever ink is shown in the InkCanvas that the user can ink on, to an ISF file.
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        // Load up ink from an ISF file.
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO. Also, need to implement the settings window.
        }

        private void Dot_OnClick(object sender, RoutedEventArgs e)
        {
            // get all strokes from inkCanvas
            var inkStrokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();


            if (_dispatcherTimerDotAnimation != null)
            {
                // Only react to the click on the dot if the the timer's not currently running.
                // If the timer is running, then the dot's already being animated.
                if (!_dispatcherTimerDotAnimation.IsEnabled)
                {
                    // Are we at the end of a stroke?
                    if (_currentAnimatedPointIndex >=
                        inkStrokes[_currentAnimatedStrokeIndex].GetInkPoints().Count - 1)
                    {
                        // Make sure the robot arm is raised at the end of the stroke.
                        RobotArm.ArmDown(false);

                        // If this isn't the last stroke, move to the next stroke.
                        if (_currentAnimatedStrokeIndex < inkStrokes.Count - 1)
                        {
                            // Move to the start of the next stroke.
                            _currentAnimatedPointIndex = 0;

                            ++_currentAnimatedStrokeIndex;

                            MoveToNextStroke();
                        }
                    }
                    else
                    {
                        // We're at the start of a stroke, so start animating the dot.
                        _dispatcherTimerDotAnimation.Start();

                        // Show a translucent dot while it's being animated. If a high contrast theme
                        // is active, keep the dot at 100% opacity to keep it high contrast against
                        // its background.
                        //if (!SystemParameters.HighContrast)
                        //{
                        //  dot.Opacity = 0.5;
                        //}
                        dot.Opacity = 0.5;
                    }
                }
            }
        }

        #endregion ButtonClickHandlers

        // Load up ink based on the ink that was shown when the app was last run.
        private void LoadInkOnStartup()
        {
            var filename = Settings1.Default.LoadedInkLocation;
            if (string.IsNullOrEmpty(filename))
            {
                // Look for default ink if we can find it in the same folder as the exe.
                filename = AppDomain.CurrentDomain.BaseDirectory + "Signature.isf";
            }

            if (File.Exists(filename))
            {
                AddInkFromFile(filename);
            }
        }


        /// <summary>
        /// Add ink to the InkCanvas, based on the contents of the supplied ISF file.
        /// </summary>
        private async void AddInkFromFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return;
            }

            // Remove any existing ink first.
            inkCanvas.InkPresenter.StrokeContainer.Clear();

            // Assume the file is valid and accessible
            var file = new FileStream(filename, FileMode.Open, FileAccess.Read); // new FileStream(filename, FileMode.Open, FileAccess.Read);
            var stream = file.AsInputStream();
            await inkCanvas.InkPresenter.StrokeContainer.LoadAsync(stream);


            //if (strokeCollection.Count > 0)
            //{
            //    // Add ink to the InkCanvas, similar to the ink loaded from the supplied file,
            //    // but with evenly distributed points along the strokes.
            //    GenerateStrokesWithEvenlyDistributedPoints(strokeCollection);

            //    ApplySettingsToInk();
            //}
        }
    }


    //public class ArmStateToDotWidthConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter,String culture)
    //    {
    //        var armDown = (bool)value;

    //        return (armDown ? Settings1.Default.DotDownWidth : Settings1.Default.DotWidth);
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, String culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}