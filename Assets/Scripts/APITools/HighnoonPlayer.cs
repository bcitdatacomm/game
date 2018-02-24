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

        public string Name { get { return _name; } }
        public bool Admin { get { return _admin; } }
        public int Shots_Fired { get { return _shots_fired; } }
        public int Kills { get { return _kills; } }
        public int Deaths { get { return _deaths; } }
        public int Games_Played { get { return _games_played; } }
        public int Games_Won { get { return _games_won; } }
        public int Time_Played { get { return _time_played; } }
    }
}

