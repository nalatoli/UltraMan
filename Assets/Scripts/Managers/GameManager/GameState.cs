using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UltraMan.Managers.GameManagerHelpers
{
    public enum GameState
    {
        [Tooltip("Initial state of game.")]
        Uninitialzed,

        [Tooltip("Client is in main menu (not connected to any game).")]
        InMainMenu,

        [Tooltip("Client is in game.")]
        InGame,
    }
}
