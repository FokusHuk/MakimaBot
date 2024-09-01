using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace MakimaBot.Model;

public class ConsoleTextDiffPrinter : ITextDiffPrinter
{
    public void DumpDiff(string textBefore, string textAfter)
    {
        var diff = InlineDiffBuilder.Diff(textBefore, textAfter);

        foreach (var line in diff.Lines)
        {
            switch (line.Type)
            {
                case ChangeType.Inserted:
                    Console.Write("+ ");
                    break;
                case ChangeType.Deleted:
                    Console.Write("- ");
                    break;
                default:
                    Console.Write("  ");
                    break;
            }

            Console.WriteLine(line.Text);
        }
    }
}
