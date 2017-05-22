namespace SrpTask.Game
{
    public class Item
    {
        /// <summary>
        /// Items unique Id;
        /// </summary>
        public readonly int Id;

        /// <summary>
        /// Items name
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// How much the item heals by.
        /// </summary>
        public readonly int Heal;

        /// <summary>
        /// How much armour the player gets when it is equipped.
        /// </summary>
        public readonly int Armour;

        /// <summary>
        /// How much this item weighs in kilograms.
        /// </summary>
        public readonly int Weight;

        /// <summary>
        /// A unique item can only be picked up once.
        /// </summary>
        public readonly bool Unique;

        /// <summary>
        /// Rare items are posh and shiny
        /// </summary>
        public readonly bool Rare;

        public Item(int id, string name, int heal, int armour, int weight, bool unique, bool rare)
        {
            Rare = rare;
            Name = name;
            Heal = heal;
            Armour = armour;
            Weight = weight;
            Unique = unique;
            Id = id;
        }
    }
}