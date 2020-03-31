using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace AdventureGameBackend
{
    class Player
    {
        public int id;
        public string username;
        public Vector3 position;
        public Vector3 change;
        public Quaternion rotation;

        private float moveSpeed = 5f / Constants.TICKS_PER_SEC;
        private float[] inputs;

        public Player(int _id, string _username, Vector3 _spawnPosition)
        {
            id = _id;
            username = _username;
            position = _spawnPosition;

            inputs = new float[2];
        }

        public void Update()
        {
            change = Vector3.Zero;
            change.X = inputs[0];
            change.Y = inputs[1];

            UpdateAnimationAndMove();
        }
        public void UpdateAnimationAndMove()
        {
            //Vector3 _forward = Vector3.Transform(new Vector3(0, 0, 1), rotation);
            //Vector3 _right = Vector3.Normalize(Vector3.Cross(_forward, new Vector3(0,1,0)));

           // Vector3 _moveDirection = _right * _inputDirection.X + _forward * _inputDirection.Y;
            //position += _moveDirection * moveSpeed;
            if (change != Vector3.Zero) 
            {
                // TODO BRING ANIMATOR HERE
                MoveCharacter();
            }

            //ServerSend.PlayerPosition(this);
            //ServerSend.PlayerRotation(this);
        }

        private void MoveCharacter()
        {
            Vector3.Normalize(change);
            // MOVE PLAYER
            position += change * moveSpeed;
            ServerSend.PlayerPosition(this);
        }

        public void SetInput(float[] _inputs)
        {
            inputs = _inputs;
        }
    }
}
