using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

public class Logic
{
    #region Singleton
    private static readonly Logic m_Instance = new Logic();
    public static Logic Instance { get { return m_Instance; } }
    static Logic() { }
    private Logic() { }
    #endregion

    public Player self,oppo;

    public void Init()
    {
        GameData.Instance.Init();
        self=new Player();
        self.RegisterOnEndTurn(PlayerEndTurn);
        oppo =new Player();
        oppo.RegisterOnEndTurn(PlayerEndTurn);
        oppo.IsSelf = false;

        GameData.Instance.SelfHand.RegisterOnDiscard(self.OnDiscard);
        GameData.Instance.OppoHand.RegisterOnDiscard(oppo.OnDiscard);
    }

    public void StartGame()
    {
        GameData.Instance.BaseDeck.Shuffle();
        self.DrawCards(7);
        oppo.DrawCards(7);
        self.BeginTurn();
    }

	void PlayerEndTurn(bool isSelf)
    {
        int result = Judge();
        if (result != 0)
        {
            EndGame(result == 1);
        }else
        {
			if (isSelf)
				oppo.BeginTurn();
            else
				self.BeginTurn();
        }
        
    }

    /// <summary>
    /// 判断游戏输赢
    /// </summary>
    /// <returns>0：还未结束；1：玩家赢；-1：AI赢</returns>
    int Judge()
    {
        return 0;
    }

    void EndGame(bool win)
    {
        
    }

    void CalcScore()
    {
        
    }

    void End()
    {
        
    }
}

public class GameData
{
    #region Singleton
    private static readonly GameData m_Instance = new GameData();
    public static GameData Instance { get { return m_Instance; } }
    static GameData() { }
    private GameData() { }
    #endregion

    public List<Deck> SelfDecks=new List<Deck>();
    public List<Deck> OppoDecks = new List<Deck>();

    public Deck SelfHand=new Deck();
    public Deck OppoHand=new Deck();

    /// <summary>
    /// 基础牌组
    /// </summary>
    public Deck BaseDeck=new Deck();
    
    /// <summary>
    /// 获取战线某一列的牌组
    /// </summary>
    /// <param name="index">战线第几列,从1开始</param>
    /// <param name="isSelf">是否是己方战线，默认为true为己方战线</param>
    /// <returns></returns>
    public Deck GetSide(int index, bool isSelf=true)
    {
        if (isSelf) return SelfDecks[index-1];
        return OppoDecks[index-1];
    }
    /// <summary>
    /// 获取第一列尚未达到上限的牌组
    /// </summary>
    /// <param name="isSelf">是否是己方战线，默认为false为对方战线</param>
    /// <returns></returns>
    public Deck GetRandomAvailableSide(bool isSelf = false)
    {
        if (isSelf) return SelfDecks.First (d => d.Count < d.MaxCount);
        return OppoDecks.First (d => d.Count < d.MaxCount);
    }

    public void Init()
    {
        for (int i = 9; i >= 0; i--)
        {
            SelfDecks.Add(new Deck());
            OppoDecks.Add(new Deck());
        }
        BaseDeck.MaxCount = 60;
        BaseDeck.AutoSort = false;
        SelfHand.MaxCount = 7;
        OppoHand.MaxCount = 7;
        BaseDeck.Init();
    }

    public void Reset()
    {
        for (int i = 0; i < SelfHand.Count; i++)
        {
            BaseDeck.AddCard(SelfHand[0]);
        }
        for (int i = 0; i < OppoHand.Count; i++)
        {
            BaseDeck.AddCard(OppoHand[0]);
        }
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < SelfDecks[i].Count; j++)
            {
                BaseDeck.AddCard(SelfDecks[i][0]);
            }
            for (int j = 0; j < OppoDecks[i].Count; j++)
            {
                BaseDeck.AddCard(OppoDecks[i][0]);
            }
        }
    }

    public Card DrawACard()
    {
        if (BaseDeck.Count <= 0)  return null;
        return BaseDeck[0];
    }
}

public delegate void Discard();

/// <summary>
/// 牌组，保存一组牌
/// </summary>
public class Deck : IList<Card>, IComparable<Deck>
{
    #region Override methods
    private readonly IList<Card> _list=new List<Card>();
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var card in this)
        {
            sb.Append("[").Append(card.ID).Append("-").Append(card).Append("] ");
        }
        return sb.ToString();
    }

    #region Implementation of IList<T>
    public int IndexOf(Card item)
    {
        return _list.IndexOf(item);
    }

    public void Insert(int index, Card item)
    {
        _list.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _list.RemoveAt(index);
    }

    public Card this[int index]
    {
        get { return _list[index]; }
        set { _list[index] = value; }
    }
    #endregion

    #region Implementation of ICollection<T>
    public void Add(Card item)
    {
        _list.Add(item);
    }

    public void Clear()
    {
        _list.Clear();
    }

    public bool Contains(Card item)
    {
        return _list.Contains(item);
    }

    public void CopyTo(Card[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    public int Count
    {
        get { return _list.Count; }
    }

    public bool IsReadOnly
    {
        get { return _list.IsReadOnly; }
    }

    public bool Remove(Card item)
    {
        return _list.Remove(item);
    }
    #endregion

    #region Implementation of IEnumerable
    public IEnumerator<Card> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion

    /// <summary>
    /// 比较两个牌组的大小
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(Deck other)
    {
        //TODO: 比较两个牌组的大小
        return 0;
    }
    #endregion
    
    #region Custom Methods
    /// <summary>
    /// 牌组上限
    /// </summary>
    public int MaxCount=3;

    /// <summary>
    /// 在新增卡牌时是否自动排序
    /// </summary>
    public bool AutoSort = true;
    
    /// <summary>
    /// 初始化牌组，只用于基础发牌堆
    /// 添加所有的卡牌，并注册移出牌组委托
    /// </summary>
    public void Init()
    {
        _list.Clear();
        for (int i = 1; i <= 10; i++)
        {
            _list.Add(new Card(TYPE.Red, i, i));
            _list.Add(new Card(TYPE.Green, i, 10 + i));
            _list.Add(new Card(TYPE.Blue, i, 20 + i));
            _list.Add(new Card(TYPE.Cyan, i, 30 + i));
            _list.Add(new Card(TYPE.Yellow, i, 40 + i));
            _list.Add(new Card(TYPE.Magenta, i, 50 + i));
        }
        foreach (var card in _list)
        {
            card.RegisterRemoveFromDeckHandler(RemoveCard);
        }
    }

    /// <summary>
    /// Card中的OnRemove委托，用于在卡牌加到新牌组中时，将卡牌从旧牌组中移除
    /// </summary>
    /// <param name="card">被移除的卡牌</param>
    public void RemoveCard(Card card)
    {
        if (!_list.Contains(card)) return;
        _list.Remove(card);

        if (this._onDiscard != null) this._onDiscard();
    }

    /// <summary>
    /// 添加卡牌到牌组中
    /// 调用Card.AddToDeck 方法以 通过 Card的AddToDeckView委托 调用 CardView的OnAdd方法将CardView添加到本牌组对应的DeckView下
    /// 注册card的RemoveFromDeck委托为RemoveCard，用以在添加card时，将card从上一个牌组移除
    /// 实际添加card到_list中
    /// 根据是否需要排序来对牌组排序
    /// </summary>
    /// <param name="card"></param>
    public void AddCard(Card card)
    {
        if (card==null || this.Contains(card) || this.Count >= MaxCount) return;
        
        card.RegisterRemoveFromDeckHandler(RemoveCard);
        _list.Add(card);
        if (AutoSort) SortCard();
    }

    /// <summary>
    /// 洗牌
    /// </summary>
    public void Shuffle()
    {
        int n = _list.Count;
        while (n > 1)
        {
            n--;
            Random r = new Random();
            int k = r.Next(n + 1);
            Card value = _list[k];
            _list[k] = _list[n];
            _list[n] = value;
        }
    }

    /// <summary>
    /// 根据卡牌ID获取牌组中的卡牌
    /// </summary>
    /// <param name="id">卡牌ID</param>
    /// <returns>若牌组中有该id的卡牌则返回该卡牌，否则返回null</returns>
    public Card GetCardByID(int id)
    {
        return _list.Single(c => c.ID == id);
    }

    /// <summary>
    /// 随机返回一张卡牌，用于从牌组中抓牌
    /// </summary>
    /// <returns>返回牌组中随机的一张卡牌</returns>
    public Card GetRandomCard()
    {
        Random r = new Random();
        return _list[r.Next(this.Count)];
    }

    /// <summary>
    /// 对牌组进行排序，按照卡牌的ID递增排序
    /// </summary>
    public void SortCard()
    {
        ((List<Card>)_list).Sort();
    }

    /// <summary>
    /// 注册用于玩家出牌的委托，当玩家牌组数量减少时调用
    /// 可能会在GameData.Reset时出错
    /// </summary>
    /// <param name="discard"></param>
    public void RegisterOnDiscard(Discard discard)
    {
        _onDiscard = discard;
    }
    private Discard _onDiscard;
    #endregion
}

public enum TYPE
{
    Red,
    Green,
    Blue,
    Yellow,
    Cyan,
    Magenta
}

//Card中的OnRemove委托，用于在卡牌加到新牌组中时，将卡牌从旧牌组中移除，将注册为Deck.RemoveCard
public delegate void RemoveFromDeck(Card card);

//Card中的OnAdd委托，用于在CardView中将卡牌移动到新的DeckView下
public class Card:IComparable<Card>,IEquatable<Card>
{
    private int _id;
    private TYPE _type;
    private int _num;

    public TYPE Type { get { return _type; } set { this._type = value; } }
    public int Num { get { return _num; } set { this._num = value; } }
    public int ID { get { return _id; } private set { this._id = value; } }

    private RemoveFromDeck onRemove;
    public void RegisterRemoveFromDeckHandler(RemoveFromDeck onRemoveHandler)
    {
        onRemove = onRemoveHandler;
    }
    
    public Card(TYPE t, int num, int id)
    {
        this._type = t;
        this._num = num;
        this._id = id;
    }


    #region Override methods
    public override string ToString()
    {
        return String.Format("{0}_{1}",_type,_num);
    }

    public int CompareTo(Card other)
    {
        if (other == null) return 1;
        return this.ID.CompareTo(other.ID);
    }

    public bool Equals(Card other)
    {
        if (other == null) return false;
        return this.ID.Equals(other.ID);
    }

    public override int GetHashCode()
    {
        return ID;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        Card other = obj as Card;
        if (other == null) return false;
        return Equals(other);
    }
    #endregion
}