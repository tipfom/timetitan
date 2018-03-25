using Core;
using Universal.Graphics;
using System;

namespace Universal.UI.Layout {
    public class Container {
        private static Vector2 REFERENCE_POSITION;
        private static Vector2 REFERENCE_SIZE;

        static Container ( ) {
            Window.Changed += ( ) => {
                REFERENCE_POSITION = new Vector2(-Window.Ratio, 1);
                REFERENCE_SIZE = new Vector2(2 * Window.Ratio, 2);
            };
            REFERENCE_POSITION = new Vector2(-Window.Ratio, 1);
            REFERENCE_SIZE = new Vector2(2 * Window.Ratio, 2);
        }

        public static implicit operator float[ ] (Container layout) {
            return layout.Box.Verticies;
        }

        public event Action Changed;
        public event Action UpdateRequired;

        public Vector2 Location = new Vector2( );
        public Vector2 Size = new Vector2( );
        public bool IsDirty = false;

        public float Width { get { return Size.X; } }
        public float Height { get { return Size.Y; } }
        public float X { get { return Location.X; } }
        public float Y { get { return Location.Y; } }

        private Position _Anchor;
        public Position Anchor { get { return _Anchor; } set { _Anchor = value; PropertyChanged( ); } }

        private Position _Dock;
        public Position Dock { get { return _Dock; } set { _Dock = value; PropertyChanged( ); } }

        private Margin _Margin;
        public Margin Margin { get { return _Margin; } set { _Margin = value; PropertyChanged( ); } }

        private Element _Relative;
        public Element Relative { get { return _Relative; } set { _Relative = value; PropertyChanged( ); } }

        private MarginType _Type;
        public MarginType Type { get { return _Type; } set { _Type = value; PropertyChanged( ); } }

        private float _Rotation;
        public float Rotation { get { return _Rotation; } set { _Rotation = value; PropertyChanged( ); } } // can be done way better, no complete refresh required // bad

        public Box Box;

        private Element element;
        private bool suppressChanges;
        
        public Container (Margin margin, MarginType type, Position anchor = Position.Left | Position.Top, Position dock = Position.Left | Position.Top, Element relative = null) {
            _Margin = margin;
            _Type = type;
            _Anchor = anchor;
            _Dock = dock;
            _Relative = relative;

            if (relative != null) {
                relative.Container.Changed += ( ) => { PropertyChanged( ); };
            } else {
                Window.Changed += ( ) => { PropertyChanged( ); };
            }
            _Margin.Changed += ( ) => { PropertyChanged( ); };
        }

        public void Initialize (Element element) {
            this.element = element;
            Refresh( );
        }

        public void Refresh ( ) {
            IsDirty = false;
            Vector2 relativeLocation = _Relative?.Container.Location ?? REFERENCE_POSITION;
            Vector2 relativeSize = _Relative?.Container.Size ?? REFERENCE_SIZE;
            float marginRight = _Margin.Right, marginLeft = _Margin.Left, marginTop = _Margin.Top, marginBottom = _Margin.Bottom;
            switch (_Type) {
                case MarginType.Relative:
                    marginRight *= relativeSize.X;
                    marginLeft *= relativeSize.X;
                    marginTop *= relativeSize.Y;
                    marginBottom *= relativeSize.Y;
                    break;
                case MarginType.Pixel:
                    marginRight *= relativeSize.X;
                    marginLeft *= relativeSize.X;
                    marginTop *= relativeSize.X;
                    marginBottom *= relativeSize.X;
                    break;
            }

            if ((_Dock & Position.Left) == Position.Left) {
                if ((_Anchor & Position.Left) == Position.Left) {
                    Size.X = marginRight;
                    Location.X = relativeLocation.X + marginLeft;
                } else if ((_Anchor & Position.Right) == Position.Right) {
                    Size.X = marginLeft;
                    Location.X = relativeLocation.X - marginRight - Size.X;
                } else {
                    Size.X = marginLeft + marginRight;
                    Location.X = relativeLocation.X - marginLeft;
                }
            } else if ((_Dock & Position.Right) == Position.Right) {
                if ((_Anchor & Position.Left) == Position.Left) {
                    Size.X = marginRight;
                    Location.X = relativeLocation.X + relativeSize.X + marginLeft;
                } else if ((_Anchor & Position.Right) == Position.Right) {
                    Size.X = marginLeft;
                    Location.X = relativeLocation.X + relativeSize.X - marginRight - Size.X;
                } else {
                    Size.X = marginLeft + marginRight;
                    Location.X = relativeLocation.X + relativeSize.X - marginLeft;
                }
            } else {
                if ((_Anchor & Position.Left) == Position.Left) {
                    Size.X = marginRight;
                    Location.X = relativeLocation.X + relativeSize.X / 2f + marginLeft;
                } else if ((_Anchor & Position.Right) == Position.Right) {
                    Size.X = marginLeft;
                    Location.X = relativeLocation.X + relativeSize.X / 2f - marginRight - Size.X;
                } else {
                    Size.X = marginLeft + marginRight;
                    Location.X = relativeLocation.X + relativeSize.X / 2f - marginLeft;
                }
            }

            if ((_Dock & Position.Top) == Position.Top) {
                if ((_Anchor & Position.Top) == Position.Top) {
                    Size.Y = marginBottom;
                    Location.Y = relativeLocation.Y - marginTop;
                } else if ((_Anchor & Position.Bottom) == Position.Bottom) {
                    Size.Y = marginTop;
                    Location.Y = relativeLocation.Y + marginBottom + Size.Y;
                } else {
                    Size.Y = marginTop + marginBottom;
                    Location.Y = relativeLocation.Y - marginTop;
                }
            } else if ((_Dock & Position.Bottom) == Position.Bottom) {
                if ((_Anchor & Position.Top) == Position.Top) {
                    Size.Y = marginBottom;
                    Location.Y = relativeLocation.Y - relativeSize.Y - marginTop;
                } else if ((_Anchor & Position.Bottom) == Position.Bottom) {
                    Size.Y = marginTop;
                    Location.Y = relativeLocation.Y - relativeSize.Y + marginBottom + Size.Y;
                } else {
                    Size.Y = marginTop + marginBottom;
                    Location.Y = relativeLocation.Y - relativeSize.Y + marginTop;
                }
            } else {
                if ((_Anchor & Position.Top) == Position.Top) {
                    Size.Y = marginBottom;
                    Location.Y = relativeLocation.Y - relativeSize.Y / 2f + marginTop;
                } else if ((_Anchor & Position.Bottom) == Position.Bottom) {
                    Size.Y = marginTop;
                    Location.Y = relativeLocation.Y - relativeSize.Y / 2f + marginBottom + Size.Y;
                } else {
                    Size.Y = marginTop + marginBottom;
                    Location.Y = relativeLocation.Y - relativeSize.Y / 2f + marginTop;
                }
            }

            Box = new Box(Location, Size, Rotation); // ba
            Changed?.Invoke( );
        }

        public void AdjustSize (Vector2 target) {
            Vector2 relativeSize = _Relative?.Container.Size ?? REFERENCE_SIZE;

            if (_Type == MarginType.Relative) target /= relativeSize;

            suppressChanges = true;

            if ((_Anchor & Position.Left) == Position.Left) {
                Margin.Right = target.X;
            } else if ((_Anchor & Position.Right) == Position.Right) {
                Margin.Left = target.X;
            } else {
                Margin.Left = target.X / 2f;
                Margin.Right = target.X / 2f;
            }

            if ((_Anchor & Position.Top) == Position.Top) {
                Margin.Bottom = target.Y;
            } else if ((_Anchor & Position.Bottom) == Position.Bottom) {
                Margin.Top = target.Y;
            } else {
                Margin.Top = target.Y / 2f;
                Margin.Bottom = target.Y / 2f;
            }

            suppressChanges = false;

            PropertyChanged( );
        }

        private void PropertyChanged ( ) {
            IsDirty = true;
            if (!suppressChanges)
                UpdateRequired?.Invoke( );
        }
    }
}