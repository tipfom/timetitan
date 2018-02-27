using System;
using System.Collections.Generic;
using Core;
using Universal.Graphics.Renderer;
using Universal.UI.Layout;

namespace Universal.UI {
    public abstract class Element : IDisposable {
        public delegate void HandleItemClick( );
        public event HandleItemClick Click;
        public event HandleItemClick Release;
        public event HandleItemClick Leave;
        public bool Clicked { get { return (clickCount > 0); } }
        private bool multiClick;
        private int clickCount;

        public readonly Container Container;

        private bool _Visible = true;
        public bool Visible { get { return Screen.IsActive && _Visible; } set { if(_Visible != value) IsDirty = true; _Visible = value; } }

        private int _Depth;
        public int Depth { get { return _Depth; } set { _Depth = value; IsDirty = true; } }

        protected bool IsDirty;

        public readonly Screen Screen;

        public Element(Screen owner, Container container, int depth, bool multiclick = false) {
            Screen = owner;

            this.multiClick = multiclick;
            this._Depth = depth;
            this.Container = container;
            UIRenderer.Add(owner, this);

            Container.Initialize(this);
            Container.UpdateRequired += ( ) => IsDirty = true;
        }

        public virtual bool HandleTouch(Touch.Action action, Touch touch) {
            if (!Visible) return false;
            switch (action) {
                case Touch.Action.Begin:
                case Touch.Action.Enter:
                    if (!Clicked || multiClick) {
                        clickCount++;
                        Click?.Invoke( );
                    }
                    break;
                case Touch.Action.End:
                    if (Clicked) {
                        clickCount--;
                        if (!Clicked)
                            Release?.Invoke( );
                    }
                    break;
                case Touch.Action.Leave:
                    if (Clicked) {
                        clickCount--;
                        if (!Clicked)
                            Leave?.Invoke( );
                    }
                    break;
            }
            return true;
        }

        public bool Collides(Vector2 touchPosition) {
            return Container.Box.Collides(touchPosition);
        }

        public virtual void Update(DeltaTime dt) {
            if (IsDirty) {
                if (Container.IsDirty) {
                    Container.Refresh( );
                }
                UIRenderer.Update(this);
                IsDirty = false;
            }
        }

        public virtual void Dispose( ) {
            UIRenderer.Remove(this);
        }

        public abstract IEnumerable<RenderableElement> Draw( );
    }
}