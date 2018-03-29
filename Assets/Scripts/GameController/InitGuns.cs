using System;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

namespace InitGuns
{
    class InitRandomGuns
    {
        static System.Random rand = new System.Random();

        // Valid Coordinates in town
        List<WeaponSpell> TownCoords = new List<WeaponSpell>();

        // Spaces Occupied By other Objects
        List<WeaponSpell> OccupiedSpaces = new List<WeaponSpell>();

        // The array of coordinates for each weapon
        List<WeaponSpell> SpawnedGuns = new List<WeaponSpell>();

        // An array of areas of interest to preferentially scatter guns around
        List<WeaponSpell> HotSpots = new List<WeaponSpell>();

        // An array of bytes to be initialized
        public byte[] pcktarray;
        public byte[] compressedpcktarray;


        // Empty Initialization from pulling from byte array
        public InitRandomGuns()
        {
        }

        // Constructor takes the number of players
        public InitRandomGuns(int NumPlayers)
        {

            // Add some dummy hotspots NOTE MUST BE AT LEAST clustering AWAY FROM EDGE IN EACH DIRECTION
            HotSpots.Add(new WeaponSpell(250, 250));
            HotSpots.Add(new WeaponSpell(800, 550));
            HotSpots.Add(new WeaponSpell(750, 750));
            HotSpots.Add(new WeaponSpell(900, 100));
            HotSpots.Add(new WeaponSpell(200, 900));

            // Add Set Coordinates for spawn generation in town.
            TownCoords.Add(new WeaponSpell(-161 + R.Init.TOWN_WIDTH, 57 + R.Init.TOWN_HEIGHT));
            TownCoords.Add(new WeaponSpell(-41 + R.Init.TOWN_WIDTH, 88 + R.Init.TOWN_HEIGHT));
            TownCoords.Add(new WeaponSpell(11 + R.Init.TOWN_WIDTH, 120 + R.Init.TOWN_HEIGHT));
            TownCoords.Add(new WeaponSpell(41 + R.Init.TOWN_WIDTH, 32 + R.Init.TOWN_HEIGHT));
            TownCoords.Add(new WeaponSpell(-21 + R.Init.TOWN_WIDTH, -3 + R.Init.TOWN_HEIGHT));
            TownCoords.Add(new WeaponSpell(23 + R.Init.TOWN_WIDTH, -65 + R.Init.TOWN_HEIGHT));
            TownCoords.Add(new WeaponSpell(72 + R.Init.TOWN_WIDTH, -85 + R.Init.TOWN_HEIGHT));
            TownCoords.Add(new WeaponSpell(132 + R.Init.TOWN_WIDTH, 45 + R.Init.TOWN_HEIGHT));
            TownCoords.Add(new WeaponSpell(10 + R.Init.TOWN_WIDTH, 75 + R.Init.TOWN_HEIGHT));
            TownCoords.Add(new WeaponSpell(18 + R.Init.TOWN_WIDTH, 22 + R.Init.TOWN_HEIGHT));
            TownCoords.Add(new WeaponSpell(-157 + R.Init.TOWN_WIDTH, 107 + R.Init.TOWN_HEIGHT));
            TownCoords.Add(new WeaponSpell(-12 + R.Init.TOWN_WIDTH, 109 + R.Init.TOWN_HEIGHT));
            TownCoords.Add(new WeaponSpell(9 + R.Init.TOWN_WIDTH, 64 + R.Init.TOWN_HEIGHT));
            TownCoords.Add(new WeaponSpell(-161 + R.Init.TOWN_WIDTH, 57 + R.Init.TOWN_HEIGHT));
            TownCoords.Add(new WeaponSpell(-139 + R.Init.TOWN_WIDTH, -22 + R.Init.TOWN_HEIGHT));
            TownCoords.Add(new WeaponSpell(30 + R.Init.TOWN_WIDTH, -14 + R.Init.TOWN_HEIGHT));
            TownCoords.Add(new WeaponSpell(-160 + R.Init.TOWN_WIDTH, -88 + R.Init.TOWN_HEIGHT));
            TownCoords.Add(new WeaponSpell(133 + R.Init.TOWN_WIDTH, -87 + R.Init.TOWN_HEIGHT));
            TownCoords.Add(new WeaponSpell(134 + R.Init.TOWN_WIDTH, 120 + R.Init.TOWN_HEIGHT));
            TownCoords.Add(new WeaponSpell(2 + R.Init.TOWN_WIDTH, -15 + R.Init.TOWN_HEIGHT));
            TownCoords.Add(new WeaponSpell(-144 + R.Init.TOWN_WIDTH, 70 + R.Init.TOWN_HEIGHT));

            WeaponSpell Weapon;
            int counter = 0;
            int interestTracker = 0;

            // Keep generating guns depending on number of players
            while (counter < numberOfWeapons(NumPlayers))
            {
                if (counter < numberOfWeapons(NumPlayers) / R.Init.DIVIDE_TOWNGUNS)
                {
                    int getT = rand.Next(TownCoords.Count);
                    Weapon = new WeaponSpell(TownCoords[getT].X, TownCoords[getT].Z, true);
                    TownCoords.RemoveAt(getT);
                }
                else
                {
                    if (rand.NextDouble() > R.Init.PERCENT_HOTSPOT)
                    {
                        Weapon = new WeaponSpell(rand.Next(0, R.Init.MAP_END),
                            rand.Next(0, R.Init.MAP_END), true);
                    }
                    else
                    {
                        // Choose a Hotspot at Random
                        interestTracker = rand.Next(0, HotSpots.Count);

                        int a = rand.Next(HotSpots[interestTracker].X - R.Init.CLUSTERING, HotSpots[interestTracker].X + R.Init.CLUSTERING);
                        int b = rand.Next(HotSpots[interestTracker].Z - R.Init.CLUSTERING, HotSpots[interestTracker].Z + R.Init.CLUSTERING);

                        Weapon = new WeaponSpell(a, b, true);
                    }
                }

                // Check if the generated coordinate is already occupied
                // if no add to coordinate array
                if (!OccupiedCheck(Weapon, OccupiedSpaces))
                {
                    counter++;
                    SpawnedGuns.Add(Weapon);

                    // Add a 12x12 box of around the spawned point to prevent weapons spawning inside/near
                    for (int i = Weapon.X - R.Init.OCCURANCE_SQUARE; i < Weapon.X + R.Init.OCCURANCE_SQUARE; i++)
                    {
                        for (int j = Weapon.Z - R.Init.OCCURANCE_SQUARE; j < Weapon.Z + R.Init.OCCURANCE_SQUARE; j++)
                        {
                            OccupiedSpaces.Add(new WeaponSpell(i, j));
                        }
                    }
                }
            }

            // Create a byte array with those coordinates
            getByteArray();
        }

        // Basic Coordinates Printed
        public void printCoordinates()
        {
            foreach (var w in SpawnedGuns)
            {
                Console.Write(w.BasicString() + ",");
            }
        }

        // Coordinates Printed with type and ID
        public void printExpandedCoordinates()
        {
            foreach (var w in SpawnedGuns)
            {
                Console.Write(w.ExtendedString() + ",");
            }
        }

        // Take a byteArray and fill spawnedGuns list from bytearray
        public void fromByteArrayToList(byte[] transmittedBytes)
        {
            int size = transmittedBytes.Length / R.Init.WEAPON_PACKET;
            int count = 0;
            byte[] tempw = new byte[R.Init.WEAPON_PACKET];
            SpawnedGuns.Clear();

            for (int i = 0; i < size; i++)
            {
                Buffer.BlockCopy(transmittedBytes, count, tempw, 0, R.Init.WEAPON_PACKET);
                WeaponSpell tempwpn = GetWeaponFromBytes(tempw);
                SpawnedGuns.Add(tempwpn);
                count += R.Init.WEAPON_PACKET;
            }
        }

        // Output the list of weapons as a bytearray
        public void getByteArray()
        {
            pcktarray = new byte[R.Init.WEAPON_PACKET * SpawnedGuns.Count];
            byte[] warray;
            int offset = 0;

            foreach (var w in SpawnedGuns)
            {
                warray = PutWeaponIntoBytes(w);
                Buffer.BlockCopy(warray, 0, pcktarray, offset, R.Init.WEAPON_PACKET);
                offset += R.Init.WEAPON_PACKET;
            }
            compressedpcktarray = compressByteArray(pcktarray);
        }

        // Roger
        public static byte[] compressByteArray(byte[] data)
        {
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                zipStream.Write(data, 0, data.Length);
                zipStream.Close();
                return compressedStream.ToArray();
            }
        }

        public class WeaponSpell
        {
            public static int inc = 0;
            public int X { get; set; }
            public int Z { get; set; }
            public int ID { get; set; }
            public byte Type { get; set; }

            // Empty Constructor
            public WeaponSpell()
            {
            }

            // Plain Coordinate Constructor
            public WeaponSpell(int X, int Z)
            {
                this.X = X;
                this.Z = Z;
            }

            // Coordinate Constructor with Types
            public WeaponSpell(int X, int Z, bool flag)
            {
                this.X = X;
                this.Z = Z;
                this.Type = GenGunType();
                this.ID = inc;
                inc++;
            }

            // Check if two sets of coordinates match
            public bool CoordinateMatch(WeaponSpell C)
            {
                if (this.X == C.X && this.Z == C.Z)
                {
                    return true;
                }
                return false;
            }

            // Basic String output
            public string BasicString()
            {
                return "(" + X + "," + Z + ")";
            }

            // Extended String output with weapon/spell type
            public string ExtendedString()
            {
                return "(ID:" + ID + " Type:" + Type + "," + X + "," + Z + ")";
            }

            // Randomly Generate a type of gun
            private byte GenGunType()
            {
                // Randomly seeded values
                double seed1 = rand.NextDouble();
                double seed2 = rand.NextDouble();

                // Gun Spawn 35% chance
                if (seed1 > 0.65)
                {
                    if (seed2 > 0.5)
                    {
                        //type 1 -- 50% chance
                        return R.Init.WEAPON_1;
                    }
                    else if (seed2 > 0.15)
                    {
                        //type = 2 -- 35% chance
                        return R.Init.WEAPON_2;
                    }
                    else
                    {
                        //type = 3 -- 15% chance
                        return R.Init.WEAPON_3;
                    }
                }

                // Spell Spawn 65% chance -- 10% chance per spell
                else
                {
                    if (seed2 > 0.9)
                    {
                        return R.Init.WEAPON_4;
                    }
                    else if (seed2 > 0.8)
                    {
                        return R.Init.WEAPON_5;
                    }
                    else if (seed2 > 0.7)
                    {
                        return R.Init.WEAPON_6;
                    }
                    else if (seed2 > 0.6)
                    {
                        return R.Init.WEAPON_7;
                    }
                    else if (seed2 > 0.5)
                    {
                        return R.Init.WEAPON_8;
                    }
                    else if (seed2 > 0.4)
                    {
                        return R.Init.WEAPON_9;
                    }
                    else if (seed2 > 0.3)
                    {
                        return R.Init.WEAPON_10;
                    }
                    else if (seed2 > 0.2)
                    {
                        return R.Init.WEAPON_11;
                    }
                    else if (seed2 > 0.1)
                    {
                        return R.Init.WEAPON_12;
                    }
                    else
                    {
                        return R.Init.WEAPON_13;
                    }
                }
            }
        }

        // Check if a certain set of spaces is occupied
        public static bool OccupiedCheck(WeaponSpell genC, List<WeaponSpell> Occupied)
        {
            return Occupied.Contains(genC);
        }

        // Takes a Weapon Object and puts it into byte array format
        public static byte[] PutWeaponIntoBytes(WeaponSpell Weapon)
        {
            byte[] wpn = new byte[R.Init.WEAPON_PACKET];

            byte[] type = new byte[sizeof(byte)];
            type[0] = Weapon.Type;
            Buffer.BlockCopy(type, 0, wpn, 0, 1);

            byte[] ID = BitConverter.GetBytes(Weapon.ID);
            Buffer.BlockCopy(ID, 0, wpn, R.Init.WEAPON_OFFSET_ID, sizeof(int));

            byte[] X = BitConverter.GetBytes(Weapon.X);
            Buffer.BlockCopy(X, 0, wpn, R.Init.WEAPON_OFFSET_X, sizeof(int));

            byte[] Z = BitConverter.GetBytes(Weapon.Z);
            Buffer.BlockCopy(Z, 0, wpn, R.Init.WEAPON_OFFSET_Z, sizeof(int));

            return wpn;
        }

        // Gets a weapon type from a bytearray format weapon
        public WeaponSpell GetWeaponFromBytes(byte[] weaponinbytes)
        {
            WeaponSpell Weapon = new WeaponSpell();

            Weapon.Type = weaponinbytes[0];

            byte[] ID = new byte[R.Init.IDBYTES];
            Weapon.ID = BitConverter.ToInt32(weaponinbytes, R.Init.WEAPON_OFFSET_ID);

            byte[] X = new byte[R.Init.COORDINATE_BYTES];
            Weapon.X = BitConverter.ToInt32(weaponinbytes, R.Init.WEAPON_OFFSET_X);

            byte[] Z = new byte[R.Init.COORDINATE_BYTES];
            Weapon.Z = BitConverter.ToInt32(weaponinbytes, R.Init.WEAPON_OFFSET_Z);

            return Weapon;
        }

        // Generates a number of guns depending on the number of players
        public static int numberOfWeapons(int players)
        {
            return players * R.Init.PLAYER_WEAPON_MULTIPLIER;
        }
    }
}
