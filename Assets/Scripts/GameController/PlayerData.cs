using UnityEngine;

class PlayerData
    {
        public byte Id { get; set; }
        public float X { get; set; }
        public float Z { get; set; }
        public float R { get; set; }
        public byte Weapon { get; set; }
        public int Health { get; set; }

        public Vector3 Position
        {
            get
            {
                return new Vector3(this.X, 0, this.Z);
            }
        }

        public Quaternion Rotation
        {
            get
            {
                return Quaternion.Euler(0, R, 0);
            }
        }

        public PlayerData()
        {
            this.Id = 0;
            this.X = 0;
            this.Z = 0;
            this.R = 0;
            this.Weapon = 0;
            this.Health = 100;
        }

        public PlayerData(byte id)
        {
            this.Id = id;
            this.X = 0;
            this.Z = 0;
            this.R = 0;
            this.Weapon = 0;
            this.Health = 100;
        }

        public PlayerData(byte id, float x, float z, float r, byte weapon)
        {
            this.Id = id;
            this.X = x;
            this.Z = z;
            this.R = r;
            this.Weapon = weapon;
            this.Health = 100;
        }

        public PlayerData(byte id, float x, float z, float r, byte weapon, int health)
        {
            this.Id = id;
            this.X = x;
            this.Z = z;
            this.R = r;
            this.Weapon = weapon;
            this.Health = health;
        }
    }
