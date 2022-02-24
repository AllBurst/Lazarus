namespace Lazarus.Interfaces

type IValueRealizable<'T> =
    abstract member GetBlackJackValue: unit -> 'T
    abstract member GetChinesePokerValue: unit -> int