using System;

﻿namespace R
{
    // Contains all the constants associated with networking and the packet
    public static class Net
    {
        public const ushort PORT = 42069;
        public const Int32 MAX_INIT_BUFFER_SIZE = 8192;

        // Contains constants associated with the header type of the packet
        public static class Header
        {
            public const byte INIT_PLAYER = 0;
            public const byte TICK = 85;
            public const byte NEW_CLIENT = 69;
            public const byte ACK = 170;

			public const byte TERRAIN_DATA = 55;
			public const byte SPAWN_DATA = 56;
        }

        // Contains constants associated with the packet offset or distance into the packet
        public static class Offset
        {
            // Offsets for server to client packet
            public const int DANGER_ZONE = 1;
            public const int PLAYER_POSITIONS = DANGER_ZONE + 12;
            public const int PLAYER_ROTATIONS = PLAYER_POSITIONS + 240;
            public const int PLAYER_IDS = PLAYER_ROTATIONS + 120;
            public const int PLAYER_INVENTORIES = PLAYER_IDS + 30;
            public const int GAME_TIME = PLAYER_INVENTORIES + 150;
            public const int PLAYER_HEALTH = GAME_TIME + 4;
            public const int WEAPON_DIFF = PLAYER_HEALTH + 1;
            public const int BULLET_DIFF = WEAPON_DIFF + 150;

            // Offsets for client to server packet
            public const int PID = 1;
            public const int X = PID + 1;
            public const int Z = X + 4;
            public const int R = Z + 4;
            public const int INV = R + 4;
            public const int BULLET = INV + 5;
        }

        // Contains related to size of the data being sent
        public static class Size
        {
            // Packet sizes
            public const int SERVER_TICK = 918;
            public const int CLIENT_TICK = 24;
        }

    }

    // Contains Constants Related to the game
    public static class Game
    {
        // Terrain Constants
        public static class Terrain
        {
            // Define default constants
            public const long DEFAULT_WIDTH = 1000;
            public const long DEFAULT_LENGTH = 1000;
            public const long DEFAULT_TILE_SIZE = 20;
            public const long DEFAULT_COLLIDER_SIZE = 20;
            // For the gun objects
            public const int GUN_OBJECT_SIZE = 13;
            public const int ID_BYTE_SIZE = 4;
            public const int X_BYTE_SIZE = 4;
            public const int Z_BYTE_SIZE = 4;
            public const int ID_OFFSET = 1;
            public const int X_OFFSET = 5;
            public const int Z_OFFSET = 9;
            // Changed to a percentage - ALam
            public const float DEFAULT_CACTUS_PERC = 0.9997f;
            public const float DEFAULT_BUSH_PERC = 0.9995f;
            public const float DEFAULT_BUILDING_PERC = 0.9999f;
            // Terrain name
            public const string DEFAULT_NAME = "Terrain";
        }

        // Player Constants
        public static class Players
        {

        }
    }
}
