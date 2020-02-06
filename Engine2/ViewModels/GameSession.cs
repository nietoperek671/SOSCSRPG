using Engine.Factories;
using Engine.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.ViewModels
{
    public class GameSession
    {
        public World CurrentWorld { get; set; }
        public Player CurrentPlayer { get; set; }
        public Location CurrentLocation { get; set; }
        public GameSession()
        {
            #region Create player
            CurrentPlayer = new Player();
            CurrentPlayer.Name = "Scott";
            CurrentPlayer.CharacterClass = "Fighter";
            CurrentPlayer.HitPoints = 10;
            CurrentPlayer.Gold = 1_000_000;
            CurrentPlayer.ExperiencePoints = 0;
            CurrentPlayer.Level = 1;
            #endregion

            WorldFactory factory = new WorldFactory();
            CurrentWorld = factory.CreateWorld();

            CurrentLocation = CurrentWorld.LocationAt(0, -1);
        }
    }
}
