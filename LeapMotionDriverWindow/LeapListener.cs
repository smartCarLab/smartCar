using System;
using Leap;
using System.Windows;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace LeapMotionDriverWindow
{
    class LeapListener: Leap.Listener
    {
        public delegate void GripMoveHandler(System.Windows.Vector position);        
        public delegate void FingerPointerHandler(Point pointer);
        public delegate void SwipeDirectionHandler(System.Windows.Vector direction);
        public event FingerPointerHandler OnFingerPointer;
        public event GripMoveHandler OnGripMove;
        public event EventHandler OnGriped;
        public event EventHandler OnGripReleased;
        public event EventHandler OnPalmOpened;
        public event SwipeDirectionHandler OnSwiped;
        private readonly Point EMPTY_POINT = new Point(-999, -999);
        private Point lastHandGrabPosition;        

        public LeapListener()
        {
            lastHandGrabPosition = EMPTY_POINT;
        }
        public override void OnInit(Controller cntrlr)
        {
            Console.WriteLine("Initialized");
        }
        public override void OnConnect(Controller cntrlr)
        {
            Console.WriteLine("Connected");
        }
        public override void OnDisconnect(Controller cntrlr)
        {
            Console.WriteLine("Disconnected");
        }
        public override void OnExit(Controller cntrlr)
        {
            Console.WriteLine("Exited");
        }
        public override void OnFrame(Controller controller)
        {
            Frame currentFrame = controller.Frame();
            GripTranslation(controller, currentFrame);
            FingerPointer(controller, currentFrame);
            GestureRecognize(currentFrame);
            CheckPalmOpen(currentFrame);
        }
        private void GestureRecognize(Frame currentFrame)
        {
            for (int g = 0; g < currentFrame.Gestures().Count; g++)
            {
                switch (currentFrame.Gestures()[g].Type)
                {
                    case Gesture.GestureType.TYPE_CIRCLE:
                        //Handle circle gestures
                        break;
                    case Gesture.GestureType.TYPE_KEY_TAP:
                        //Handle key tap gestures
                        break;
                    case Gesture.GestureType.TYPE_SCREEN_TAP:
                        //Handle screen tap gestures
                        break;
                    case Gesture.GestureType.TYPE_SWIPE:
                        HandleSwipe(new SwipeGesture(currentFrame.Gestures()[g]));
                        break;
                    default:
                        //Handle unrecognized gestures
                        break;
                }
            }
        }
        private void HandleSwipe(SwipeGesture swipe)
        {
            switch (swipe.State)
            {
                case Gesture.GestureState.STATE_START:                    
                    break;
                case Gesture.GestureState.STATE_UPDATE:                    
                    break;
                case Gesture.GestureState.STATE_STOP:
                    if (this.OnSwiped != null)
                        this.OnSwiped(new System.Windows.Vector(swipe.Direction.x, swipe.Direction.y));
                    break;
                default:                    
                    break;
            }
        }
        private void FingerPointer(Controller controller, Frame currentFrame)
        {
            if (currentFrame.Hands.IsEmpty) return;
            if (currentFrame.Fingers.Count < 1) return;
            var finger = currentFrame.Fingers[0];
            var screen = controller.LocatedScreens.ClosestScreenHit(finger);
            if (screen != null && screen.IsValid)
            {
                var tipVelocity = (int)finger.TipVelocity.Magnitude;
                if (tipVelocity > 25)
                {
                    var xScreenIntersect = screen.Intersect(finger, true).x;
                    var yScreenIntersect = screen.Intersect(finger, true).y;
                    if (xScreenIntersect.ToString() != "NaN")
                    {
                        var x = (int)(xScreenIntersect * screen.WidthPixels);
                        var y = (int)(screen.HeightPixels - (yScreenIntersect * screen.HeightPixels));
                        if (this.OnFingerPointer != null)
                            this.OnFingerPointer(new Point(x, y));
                    }
                }
            }
        }
        private void CheckPalmOpen(Frame currentFrame)
        {
            if (currentFrame.Hands.Count > 0 && currentFrame.Hands[0].Direction.y > 0)
            {
                if (this.OnPalmOpened != null) this.OnPalmOpened(this, EventArgs.Empty);
            }
        }
        private void GripTranslation(Controller controller, Frame currentFrame)
        {
            if (currentFrame.Hands.Count > 0 && currentFrame.Hands[0].GrabStrength > 0.9)
            {
                var grabHand = currentFrame.Hands[0];
                if (lastHandGrabPosition == EMPTY_POINT)
                {
                    lastHandGrabPosition = new Point(grabHand.StabilizedPalmPosition.x, grabHand.StabilizedPalmPosition.y);
                    if (this.OnGriped != null)
                        this.OnGriped(this, EventArgs.Empty);
                }
                else
                {
                    var screen = controller.LocatedScreens.ClosestScreenHit(grabHand.StabilizedPalmPosition, grabHand.Direction);
                    if (screen != null && screen.IsValid)
                    {
                        var xScreenIntersect = screen.Intersect(grabHand.StabilizedPalmPosition, grabHand.Direction, true).x;
                        var yScreenIntersect = screen.Intersect(grabHand.StabilizedPalmPosition, grabHand.Direction, true).y;
                        if (xScreenIntersect.ToString() != "NaN")
                        {
                            var x = (int)(xScreenIntersect * screen.WidthPixels);
                            var y = (int)(screen.HeightPixels - (yScreenIntersect * screen.HeightPixels));
                            if (this.OnGripMove != null)
                                this.OnGripMove(new System.Windows.Vector(x, y));
                        }
                        lastHandGrabPosition = new Point(grabHand.StabilizedPalmPosition.x, grabHand.StabilizedPalmPosition.y);
                    }
                }
            }
            else
            {
                if (lastHandGrabPosition == EMPTY_POINT) return;
                lastHandGrabPosition = EMPTY_POINT;
                if (this.OnGripReleased != null)
                    this.OnGripReleased(this, EventArgs.Empty);
            }
        }   
    }
}
