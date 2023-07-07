using System;
using System.Collections.Generic;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using UnturnedRocketModPluginTemplate;
using Logger = Rocket.Core.Logging.Logger;
using Random = System.Random;

namespace PointRobbery
{
    public class Main : RocketPlugin<Config>
    {
        private Dictionary<ulong, DateTime> _recentlyRobbed;

        protected override void Load()
        {
            _recentlyRobbed = new Dictionary<ulong, DateTime>();
            UnturnedPlayerEvents.OnPlayerUpdateGesture += UnturnedPlayerEventsOnOnPlayerUpdateGesture;
            
            Logger.Log($"Kamiluk || PointRobbery plugin has been loaded");
        }

        private void UnturnedPlayerEventsOnOnPlayerUpdateGesture(UnturnedPlayer player, UnturnedPlayerEvents.PlayerGesture gesture)
        {
            if (gesture != UnturnedPlayerEvents.PlayerGesture.Point)
                return;

            var locatedPlayer = DamageTool.raycast(new Ray(player.Player.look.aim.position, 
                player.Player.look.aim.forward), 
                Configuration.Instance.RobbingDistance, 
                RayMasks.ENEMY);

            if (locatedPlayer.player == null)
                return;
            
            var target = locatedPlayer.player;

            if (target.animator.gesture != EPlayerGesture.SURRENDER_START)
                return;

            if (_recentlyRobbed.ContainsKey(target.channel.owner.playerID.steamID.m_SteamID))
            {
                if ((DateTime.Now - _recentlyRobbed[target.channel.owner.playerID.steamID.m_SteamID]).TotalSeconds <
                    Configuration.Instance.DelayBetweenRobberies)
                {
                    UnturnedChat.Say(player.CSteamID, "Ten gracz został przed chwilą okradziony, musisz zaczekać.");
                    return;
                }
            }
            
            if (target.quests.isMemberOfSameGroupAs(player.Player) && !Configuration.Instance.CanRobGroupMembers)
            {
                UnturnedChat.Say(player.CSteamID, "Nie możesz okradać członka swojej grupy.");
                return;
            }
            
            Random rnd = new Random();
            
            foreach (var page in target.inventory.items)
            {
                foreach (var item in page.items)
                {
                    if (rnd.Next(1, 101) < Configuration.Instance.Chance)
                        target.inventory.sendDropItem(page.page, item.x, item.y);
                }
            }
            
            _recentlyRobbed[target.channel.owner.playerID.steamID.m_SteamID] = DateTime.Now;
        }

        protected override void Unload()
        {
            UnturnedPlayerEvents.OnPlayerUpdateGesture -= UnturnedPlayerEventsOnOnPlayerUpdateGesture;
            
            Logger.Log($"Kamiluk || PointRobbery plugin has been unloaded");
        }
    }
}