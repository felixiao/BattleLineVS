using System.Collections;
using System.Security.Policy;
using System;
public delegate void EndTurn(bool isPlayer);
public class Player
{
    /// <summary>
    /// 是否是玩家，Ture为玩家，False为AI
    /// </summary>
    public bool IsSelf = true;

    private EndTurn onEndTurn;
    public void RegisterOnEndTurn(EndTurn endTurn)
    {
        onEndTurn = endTurn;
    }

    public void BeginTurn()
    {
        Console.WriteLine(string.Format("{0} Begins",IsSelf?"Player":"AI"));
        if(!IsSelf) Discard();
    }
    
    /// <summary>
    /// 玩家从发牌组中摸牌到自己手里，默认摸一张
    /// </summary>
    /// <param name="count">摸到手里的牌数</param>
    public void DrawCards(int count=1)
    {
        Console.WriteLine(string.Format("{0} DrawCards",IsSelf?"Player":"AI"));
        for(int i=count;i>0;i--)
        {
            if(IsSelf)
                GameData.Instance.SelfHand.AddCard(GameData.Instance.DrawACard());
            else
                GameData.Instance.OppoHand.AddCard(GameData.Instance.DrawACard());
        }
    }

    /// <summary>
    /// AI判断出牌
    /// </summary>
    public void Discard()
    {
		Console.WriteLine(string.Format("{0} Discard",IsSelf?"Player":"AI"));
        //随机出牌
        Deck d = GameData.Instance.GetRandomAvailableSide();
        if(d!=null) d.AddCard(GameData.Instance.OppoHand.GetRandomCard());
    }
    /// <summary>
    /// 玩家出完牌的回调
    /// </summary>
    public void OnDiscard()
    {
        Console.WriteLine(string.Format("{0} OnDiscard",IsSelf?"Player":"AI"));
		DrawCards();
		EndTurn();
    }

    public void EndTurn()
    {
        Console.WriteLine(string.Format("{0} Ends", IsSelf ? "Player" : "AI"));
		if(onEndTurn!=null) onEndTurn(IsSelf);
    }
}
