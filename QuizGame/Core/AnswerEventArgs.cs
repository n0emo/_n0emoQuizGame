using System;

namespace QuizGame.Core;

public class AnswerEventArgs : EventArgs
{
    public int AnsweredTeam { get; }
    
    public int Score { get; }
    
    public AnswerResult Result { get; }
    
    public enum AnswerResult
    {
        Successed, Canceled
    }

    public AnswerEventArgs(int answeredTeam, int score, AnswerResult result)
    {
        AnsweredTeam = answeredTeam;
        Score = score;
        Result = result;
    }
}