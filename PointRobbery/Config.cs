using Rocket.API;

namespace UnturnedRocketModPluginTemplate
{
    public class Config : IRocketPluginConfiguration
    {
        public float RobbingDistance;
        public uint DelayBetweenRobberies;
        public uint Chance;
        public bool CanRobGroupMembers;

        public void LoadDefaults()
        {
            RobbingDistance = 5;
            DelayBetweenRobberies = 30;
            Chance = 50;
            CanRobGroupMembers = true;
        }
    }
}