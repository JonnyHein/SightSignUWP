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


        ///<summary>
        /// Creating the Write button functionality [[Testing]]
        ///</summary>
        private void WriteButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO ::refactor this code later
            var inkStrokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();

            if (inkStrokes.Count == 0)
            {
                return;
            }

            // TODO :: handle writing ink strokes on canvas by sending 
            //         Gcode to robot arm.

            // Lift robot arm
            RobotArm.ArmDown(false);

            // Move to the start of the signature
            var inkPoints = inkStrokes[0].GetInkPoints();
            MoveDotAndRobotToInkPoint(inkPoints[0]);

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
                }
            }
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

            _strokeBeingAnimated = CreateStroke(ptCollection);

            SetDrawingAttributesFromSettings(_strokeBeingAnimated.DrawingAttributes);

            inkCanvasAnimations.InkPresenter.StrokeContainer.AddStroke(_strokeBeingAnimated);
        }

        // Create an InkStroke from a list of InkPoints.
        private InkStroke CreateStroke(List<InkPoint> ptCollection)
        {
            var strokeBuilder = new InkStrokeBuilder();
            System.Numerics.Matrix3x2 matrix = System.Numerics.Matrix3x2.Identity;
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

        private void Dot_OnClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("clicked");
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