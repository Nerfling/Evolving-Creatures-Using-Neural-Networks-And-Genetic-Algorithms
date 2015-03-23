using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Savant
{
    class Camera
    {
        #region Fields

        protected float _zoom;
        protected Matrix _transform;
        protected Matrix _inverseTransform;
        protected Vector2 _pos;
        protected float _rotation;
        protected Viewport _viewport;
        protected MouseState _mState;
        protected KeyboardState _keyState;
        protected Int32 _scroll;

        #endregion

        #region Properties

        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }
        /// <summary>
        /// Camera View Matrix Property
        /// </summary>
        public Matrix Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }
        /// <summary>
        /// Inverse of the view matrix, can be used to get objects screen coordinates
        /// from its object coordinates
        /// </summary>
        public Matrix InverseTransform
        {
            get { return _inverseTransform; }
        }
        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }
        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        #endregion

        #region Constructor

        public Camera(Viewport viewport)
        {
            _zoom = 0.30f;
            _scroll = 1;
            _rotation = 0.0f;
            _pos = new Vector2((1600 / 2), (900 / 2));
            _viewport = viewport;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Update the camera view
        /// </summary>
        public void Update()
        {
            //Call Camera Input
            Input();
            //Clamp zoom value
            _zoom = MathHelper.Clamp(_zoom, 0.0f, 10.0f);
            //Clamp rotation value
            _rotation = ClampAngle(_rotation);
            //Create view matrix
            _transform = Matrix.CreateRotationZ(_rotation) *
                            Matrix.CreateScale(new Vector3(_zoom, _zoom, 1)) *
                            Matrix.CreateTranslation(_pos.X, _pos.Y, 0);
            //Update inverse matrix
            _inverseTransform = Matrix.Invert(_transform);
        }

        public Matrix Translate(Vector2 Position, Vector2 Origin)
        {
            return Matrix.CreateTranslation(new Vector3(-Position, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                Matrix.CreateRotationZ(_rotation) *
                Matrix.CreateScale(_zoom, _zoom, 1) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }

        /// <summary>
        /// Example Input Method, rotates using cursor keys and zooms using mouse wheel
        /// </summary>
        protected virtual void Input()
        {
            _mState = Mouse.GetState();
            _keyState = Keyboard.GetState();
            //Check zoom
            if (_mState.ScrollWheelValue > _scroll)
            {
                _zoom += 0.1f;
                _scroll = _mState.ScrollWheelValue;
            }
            else if (_mState.ScrollWheelValue < _scroll)
            {
                _zoom -= 0.1f;
                _scroll = _mState.ScrollWheelValue;
            }
            //Check rotation
            if (_keyState.IsKeyDown(Keys.A))
            {
                _rotation -= 0.01f;
            }
            if (_keyState.IsKeyDown(Keys.E))
            {
                _rotation += 0.01f;
            }
            //Check Move
            if (_keyState.IsKeyDown(Keys.Q))
            {
                _pos.X += 4f;
            }
            if (_keyState.IsKeyDown(Keys.D))
            {
                _pos.X -= 4f;
            }
            if (_keyState.IsKeyDown(Keys.Z))
            {
                _pos.Y += 4f;
            }
            if (_keyState.IsKeyDown(Keys.S))
            {
                _pos.Y -= 4f;
            }
        }

        /// <summary>
        /// Clamps a radian value between -pi and pi
        /// </summary>
        /// <param name="radians">angle to be clamped</param>
        /// <returns>clamped angle</returns>
        protected float ClampAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }

        #endregion
    }
}
