using System;
using Engine.Models;
using Engine.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Services
{
    [TestClass]
    public class TestCombatService
    {
        [TestMethod]
        public void Test_FirstAttacker()
        {
            Player p = new Player("", "", 0, 18, 0, 0, 0);
            Monster m = new Monster(0, "", "", 0, 12, null, 0, 0);

            CombatService.Combatant result = CombatService.FirstAttacker(p,m);
        }
    }
}
