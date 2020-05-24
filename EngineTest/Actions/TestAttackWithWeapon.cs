using System;
using Engine.Actions;
using Engine.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Actions
{
    [TestClass]
    public class TestAttackWithWeapon
    {
        [TestMethod]
        public void Test_Constructor_GoodParameters()
        {
            var pointyStick = ItemFactory.CreateGameItem(1001);

            var attackWithWeapon = new AttackWithWeapon(pointyStick, 1, 5);

            Assert.IsNotNull(attackWithWeapon);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Constructor_ItemIsNotAWeapon()
        {
            var granolaBar = ItemFactory.CreateGameItem(2001);

            var attackWithWeapon = new AttackWithWeapon(granolaBar, 1, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Constructor_MinimumDamageLessThanZero()
        {
            var pointyStick = ItemFactory.CreateGameItem(1001);

            var attackWithWeapon = new AttackWithWeapon(pointyStick, -1, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Constructor_MaximumDamageLessThanMinimumDamage()
        {
            var pointyStick = ItemFactory.CreateGameItem(1001);

            var attackWithWeapon = new AttackWithWeapon(pointyStick, 6, 5);
        }
    }
}