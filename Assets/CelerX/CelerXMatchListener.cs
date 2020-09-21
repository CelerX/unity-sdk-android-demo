using UnityEngine;
using System.Collections;

public interface CelerXMatchListener
{
    /*
     * This Function will be callback to C sharp after CelerX platform has match player finishing.
       Developers can get any match information from param `MatchInfo`, such as players information and sharedRandomSeed. Developers must give ui rendering in this callback.
     */
    void onMatchJoined(MatchInfo mathInfo);

    /*
     * Calling after player clicking ready button and CelerX platform confirmed everything is ready, game can start now.
     */
    void onMatchReadyToStart(MatchInfo mathInfo);
}