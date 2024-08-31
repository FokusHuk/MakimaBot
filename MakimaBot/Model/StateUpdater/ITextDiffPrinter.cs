namespace MakimaBot.Model;

public interface ITextDiffPrinter
{
    void DumpDiff(string textBefore, string textAfter);
}
