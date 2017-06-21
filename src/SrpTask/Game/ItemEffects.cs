namespace SrpTask.Game
{
    public class ItemEffects
    {
        private readonly IGameEngine _gameEngine;

        public ItemEffects(IGameEngine gameEngine)
        {
            _gameEngine = gameEngine;
        }

        public void PlayForItem(Item item)
        {
            if (item.Heal > 500)
            {
                _gameEngine.PlaySpecialEffect("green_swirly");
            }
            else if (item.Heal > 0)
            {
                return;
            }
            else if (item.Rare && item.Unique)
            {
                _gameEngine.PlaySpecialEffect("blue_swirly");
            }
            else if (item.Rare)
            {
                _gameEngine.PlaySpecialEffect("cool_swirly_particles");
            }
        }
    }
}