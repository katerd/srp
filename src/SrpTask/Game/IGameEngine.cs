using System.Collections.Generic;

namespace SrpTask.Game
{
    public interface IGameEngine
    {
        void PlaySpecialEffect(string effectName);
        List<IEnemy> GetEnemiesNear(RpgPlayer player);
    }
}