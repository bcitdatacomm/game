using System;

namespace HighnoonTools
{
    public class HighnoonPlayer
    {
        string _name;
        bool _admin;
        int _shots_fired;
        int _kills;
        int _deaths;
        int _games_played;
        int _games_won;
        int _time_played; // TODO: is this seconds?

		public string Name { get { return _name; } set { _name = value; } }
		public bool Admin { get { return _admin; } set { _admin = value; } }
		public int Shots_Fired { get { return _shots_fired; } set { _shots_fired = value; } }
		public int Kills { get { return _kills; } set { _kills = value; } }
		public int Deaths { get { return _deaths; } set { _deaths = value; } }
		public int Games_Played { get { return _games_played; } set { _games_played = value; } }
		public int Games_Won { get { return _games_won; } set { _games_won = value; } }
		public int Time_Played { get { return _time_played; } set { _time_played = value; } }
    }
}

