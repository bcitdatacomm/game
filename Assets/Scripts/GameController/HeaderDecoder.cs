/*---------------------------------------------------------------------------------------
--	SOURCE FILE:	HeaderDecoder.cs
--
--	PROGRAM:		GameController
--
--	FUNCTIONS:		public static int GetPlayerCount(byte header)
--				public static bool IsTickPacket(byte header)
--				public static bool HasBullet(byte header)
--				public static bool HasWeapon(byte header)
--
--	DATE:			Apr 2, 2018
--
--	REVISIONS:
--
--	DESIGNERS:		Benny Wang
--
--	PROGRAMMER:	Benny Wang
--
--	NOTES:
-- File for functions that determines what kind of data is in the packet.
---------------------------------------------------------------------------------------*/

class HeaderDecoder
{
  /*-------------------------------------------------------------------------------------------------
    -- FUNCTION: 		GetPlayerCount
    --
    -- DATE: 			Apr 2, 2018
    --
    -- REVISIONS:
    --
    -- DESIGNER: 		Benny Wang
    --
    -- PROGRAMMER: 	Benny Wang
    --
    -- INTERFACE: 		GetPlayerCount(byte header)
    --				header : the header byte
    --
    -- RETURNS: 		int of the number of players in the game
    --
    -- NOTES:
    -- Returns the number of players in the game.
-------------------------------------------------------------------------------------------------*/

    public static int GetPlayerCount(byte header)
    {
        return (0x1F & header);
    }

    /*-------------------------------------------------------------------------------------------------
        -- FUNCTION: 		IsTickPacket
        --
        -- DATE: 			Apr 2, 2018
        --
        -- REVISIONS:
        --
        -- DESIGNER: 		Benny Wang
        --
        -- PROGRAMMER: 	Benny Wang
        --
        -- INTERFACE: 		IsTickPacket(byte header)
        --				header : the header byte
        --
        -- RETURNS: 		whether the byte passed in is a tick packet
        --
        -- NOTES:
        -- Checks whether the byte means the packet is a tick packet or not.
    -------------------------------------------------------------------------------------------------*/

    public static bool IsTickPacket(byte header)
    {
        return (0x80 & header) > 0;
    }
    /*-------------------------------------------------------------------------------------------------
        -- FUNCTION: 		HasBullet
        --
        -- DATE: 			Apr 2, 2018
        --
        -- REVISIONS:
        --
        -- DESIGNER: 		Benny Wang
        --
        -- PROGRAMMER: 	Benny Wang
        --
        -- INTERFACE: 		HasBullet(byte header)
        --				header : the header byte
        --
        -- RETURNS: 		whether the packet has bullet info or not
        --
        -- NOTES:
        -- Checks whether the packet has bullet information or not.
    -------------------------------------------------------------------------------------------------*/

    public static bool HasBullet(byte header)
    {
        return (0x40 & header) > 0;
    }
    /*-------------------------------------------------------------------------------------------------
        -- FUNCTION: 		HasWeapon
        --
        -- DATE: 			Apr 2, 2018
        --
        -- REVISIONS:
        --
        -- DESIGNER: 		Benny Wang
        --
        -- PROGRAMMER: 	Benny Wang
        --
        -- INTERFACE: 		HasWeapon(byte header)
        --				header : the header byte
        --
        -- RETURNS: 		whether the packet has weapon info or not
        --
        -- NOTES:
        -- Checks whether the packet has weapon information or not.
    -------------------------------------------------------------------------------------------------*/

    public static bool HasWeapon(byte header)
    {
        return (0x20 & header) > 0;
    }
}
