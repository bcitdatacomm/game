using UnityEngine;
/*---------------------------------------------------------------------------------------
--	SOURCE FILE:	PlayerData.cs
--
--	PROGRAM:		GameController
--
--	FUNCTIONS:		public PlayerData()
--				public PlayerData(byte id)
--				public PlayerData(byte id, float x, float z, float r, byte weapon)
public PlayerData(byte id, float x, float z, float r, byte weapon, int	 health)
--
--	DATE:			Apr 2, 2018
--
--	REVISIONS:		Apr 10, 2018 - added arrow pointing to danger zone
--
--	DESIGNERS:		Benny Wang
--
--	PROGRAMMER:	Benny Wang
--
--	NOTES:
-- Initializes and defines important player data.
---------------------------------------------------------------------------------------*/

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
        /*-------------------------------------------------------------------------------------------------
            -- FUNCTION: 		PlayerData()
            --
            -- DATE: 			Apr 2, 2018
            --
            -- REVISIONS:
            --
            -- DESIGNER: 		Benny Wang
            --
            -- PROGRAMMER: 	Benny Wang
            --
            -- INTERFACE: 		PlayerData()
            --
            -- RETURNS: 		void
            --
            -- NOTES:
            -- Initizalizes empty player data, with full health.
        -------------------------------------------------------------------------------------------------*/

        public PlayerData()
        {
            this.Id = 0;
            this.X = 0;
            this.Z = 0;
            this.R = 0;
            this.Weapon = 0;
            this.Health = 100;
        }
        /*-------------------------------------------------------------------------------------------------
            -- FUNCTION: 		PlayerData(byte id)
            --
            -- DATE: 			Apr 2, 2018
            --
            -- REVISIONS:
            --
            -- DESIGNER: 		Benny Wang
            --
            -- PROGRAMMER: 	Benny Wang
            --
            -- INTERFACE: 		PlayerData(byte id)
            --				id : the ID of the player
            --
            -- RETURNS: 		void
            --
            -- NOTES:
            -- Initizalizes empty player data with a given ID and full health.
        -------------------------------------------------------------------------------------------------*/

        public PlayerData(byte id)
        {
            this.Id = id;
            this.X = 0;
            this.Z = 0;
            this.R = 0;
            this.Weapon = 0;
            this.Health = 100;
        }
        /*-------------------------------------------------------------------------------------------------
            -- FUNCTION: 		PlayerData(byte id, float x, float z, float r, byte weapon)
            --
            -- DATE: 			Apr 2, 2018
            --
            -- REVISIONS:
            --
            -- DESIGNER: 		Benny Wang
            --
            -- PROGRAMMER: 	Benny Wang
            --
            -- INTERFACE: 		PlayerData(byte id, float x, float z, float r, byte weapon)
            --				id : the ID of the player
            --				x, z : the position of the player
            --				r : the way the player faces
            --				weapon : the weapon to give the player
            --
            -- RETURNS: 		void
            --
            -- NOTES:
            -- Initizalizes a player with a given ID, full health at a certain position
            -- with a weapon.
        -------------------------------------------------------------------------------------------------*/

        public PlayerData(byte id, float x, float z, float r, byte weapon)
        {
            this.Id = id;
            this.X = x;
            this.Z = z;
            this.R = r;
            this.Weapon = weapon;
            this.Health = 100;
        }
        /*-------------------------------------------------------------------------------------------------
            -- FUNCTION: 		PlayerData(byte id, float x, float z, float r, byte weapon int health)
            --
            -- DATE: 			Apr 2, 2018
            --
            -- REVISIONS:
            --
            -- DESIGNER: 		Benny Wang
            --
            -- PROGRAMMER: 	Benny Wang
            --
            -- INTERFACE: 		PlayerData(byte id, float x, float z, float r, byte weapon int health)
            --				id : the ID of the player
            --				x, z : the position of the player
            --				r : the way the player faces
            --				weapon : the weapon to give the player
            --				health : the health of the player
            --
            -- RETURNS: 		void
            --
            -- NOTES:
            -- Initizalizes a player with a given ID, given health at a certain position
            -- with a weapon.
        -------------------------------------------------------------------------------------------------*/

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
