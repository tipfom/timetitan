using System.Collections.Generic;
using Android.Views;
using Core;
using Universal.Graphics.Renderer;
using Universal.UI;
using Window = Universal.Graphics.Window;

namespace Android {
    public class TouchHandler : Java.Lang.Object, View.IOnTouchListener {
        const int MAX_TOUCH_COUNT = 4;

        public static TouchHandler Instance { get; } = new TouchHandler( );

        private List<Touch> activeTouches = new List<Touch>( );
        
        public bool OnTouch (global::Android.Views.View v, MotionEvent e) {
            int pointerIndex = ((int)(e.Action & MotionEventActions.PointerIdMask) >> (int)MotionEventActions.PointerIdShift);
            int pointerId = e.GetPointerId(pointerIndex);

            switch (e.Action & MotionEventActions.Mask) {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    // user touched the screen
                    if (activeTouches.Count < MAX_TOUCH_COUNT) {
                        Touch touch = new Touch(pointerId, new Vector2(e.GetX(pointerIndex), e.GetY(pointerIndex)));
                        activeTouches.Add(touch);

                        for(int i = 0; i< UIRenderer.Current.Count; i++) {
                            Element item = UIRenderer.Current[i];
                            if (item.Collides(touch.RelativePosition) && item.HandleTouch(Touch.Action.Begin, touch)) {
                                break;
                            }
                        }
                    }
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                case MotionEventActions.PointerUp:
                    // user lifted the finger of the screen
                    int touchIndex = activeTouches.FindIndex((Touch touch) => touch.ID == pointerId);
                    if (touchIndex != -1) {
                        for (int i = 0; i < UIRenderer.Current.Count; i++) {
                            Element item = UIRenderer.Current[i];
                            if (item.Collides(activeTouches[touchIndex].RelativePosition) && item.HandleTouch(Touch.Action.End, activeTouches[touchIndex])) {
                                break;
                            }
                        }
                        activeTouches.RemoveAt(touchIndex);
                    }
                    break;
                case MotionEventActions.Move:
                    // user moved the finger
                    for (int i = 0; i < activeTouches.Count; i++) {
                        int activePointerIndex = e.FindPointerIndex(activeTouches[i].ID);
                        Vector2 activeTouchPosition = new Vector2(e.GetX(activePointerIndex), e.GetY(activePointerIndex));
                        if (activeTouches[i].Position - activeTouchPosition != Vector2.Zero) {
                            // touch moved
                            Vector2 activeTouchRelativePosition = new Vector2((activeTouchPosition.X / Window.Size.Width - 0.5f) * 2 * Window.Ratio, (activeTouchPosition.Y / Window.Size.Height - 0.5f) * -2);
                            for (int j = 0; j < UIRenderer.Current.Count; j++) {
                                Element item = UIRenderer.Current[j];

                                bool current = item.Collides(activeTouchRelativePosition);
                                bool last = item.Collides(activeTouches[i].RelativePosition);

                                if (current && !last) {
                                    // collides with the current position, but not with the last
                                    if (item.HandleTouch(Touch.Action.Enter, activeTouches[i])) {
                                        for(int k = j+1; k < UIRenderer.Current.Count; k++) {
                                            Element kitem = UIRenderer.Current[k];
                                            if (kitem.Collides(activeTouches[i].RelativePosition)) {
                                                kitem.HandleTouch(Touch.Action.Leave, activeTouches[i]);
                                            }
                                        }
                                    }
                                } else if (!current && last) {
                                    // collides with the last position, but not with the current -> touch moved out of UIelement
                                    if(item.HandleTouch(Touch.Action.Leave, activeTouches[i])) {
                                        for (int k = j + 1; k < UIRenderer.Current.Count; k++) {
                                            Element kitem = UIRenderer.Current[k];
                                            if (kitem.Collides(activeTouchRelativePosition)) {
                                                kitem.HandleTouch(Touch.Action.Enter, activeTouches[i]);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                } else if (current) {
                                    // touch moved inside UI
                                    if (item.HandleTouch(Touch.Action.Move, activeTouches[i])) break;
                                }
                            }

                            activeTouches[i].Position = activeTouchPosition;
                            activeTouches[i].RelativePosition = activeTouchRelativePosition;
                        }
                    }
                    break;
            }

            return true;
        }
    }
}