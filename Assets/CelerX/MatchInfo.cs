﻿using System;
using System.Collections.Generic;

public class MatchInfo
{
    public bool shouldLaunchTutorial;
    public string matchId;
    public double sharedRandomSeed;
    public GamePlayer currentPlayer;
    public List<GamePlayer>  players;
    public int difficultyLevel;
    public string gameId;
    public bool isNeedRecordScreen;

    public MatchInfo()
    {
    }
}
