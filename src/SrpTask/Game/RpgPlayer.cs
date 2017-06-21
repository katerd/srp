using System.Collections.Generic;
using System.Linq;

namespace SrpTask.Game
{
    public class RpgPlayer
    {
        public const int MaximumCarryingCapacity = 1000;

        private readonly IGameEngine _gameEngine;

        public int Health { get; set; }

        public int MaxHealth { get; set; }

        public int Armour { get; private set; }

        public List<Item> Inventory;
        private readonly ItemEffects _itemEffects;

        /// <summary>
        /// How much the player can carry in kilograms
        /// </summary>
        public int CarryingCapacity { get; private set; }

        public RpgPlayer(IGameEngine gameEngine)
        {
            _gameEngine = gameEngine;
            _itemEffects = new ItemEffects(gameEngine);
            Inventory = new List<Item>();
            CarryingCapacity = MaximumCarryingCapacity;
        }

        public void UseItem(Item item)
        {
            if (item.Name == "Stink Bomb")
            {
                var enemies = _gameEngine.GetEnemiesNear(this);

                foreach (var enemy in enemies)
                {
                    enemy.TakeDamage(100);
                }
            }
        }

        public bool PickUpItem(Item item)
        {
            var weight = CalculateInventoryWeight();
            if (weight + item.Weight > CarryingCapacity)
                return false;

            if (item.Unique && CheckIfItemExistsInInventory(item))
                return false;

            // Don't pick up items that give health, just consume them.
            if (UseOnPickup(item))
            {
                Health += item.Heal;

                if (Health > MaxHealth)
                    Health = MaxHealth;
            }
            else
            {
                Inventory.Add(item);
                CalculateStats();
            }

            _itemEffects.PlayForItem(item);

            return true;
        }

        private static bool UseOnPickup(Item item)
        {
            return item.Heal > 0;
        }

        private void CalculateStats()
        {
            Armour = Inventory.Sum(x => x.Armour);
        }

        private bool CheckIfItemExistsInInventory(Item item)
        {
            return Inventory.Any(x => x.Id == item.Id);
        }

        private int CalculateInventoryWeight()
        {
            return Inventory.Sum(x => x.Weight);
        }

        public void TakeDamage(int damage)
        {
            if (damage < Armour)
            {
                _gameEngine.PlaySpecialEffect("parry");
                return;
            }

            var damageToDeal = damage - Armour;
            Health -= damageToDeal;
            
            _gameEngine.PlaySpecialEffect("lots_of_gore");
        }
    }
}