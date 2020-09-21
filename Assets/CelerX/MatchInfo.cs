using System;
using System.Collections.Generic;

public class MatchInfo
{
    public string shouldLaunchTutorial;
    public string matchId;
    public double sharedRandomSeed;
    public GamePlayer currentPlayer;
    public List<GamePlayer>  players;

    public MatchInfo()
    {
    }
}
