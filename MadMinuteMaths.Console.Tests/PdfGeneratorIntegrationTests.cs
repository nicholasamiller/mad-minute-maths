using System;
using System.IO;
using MadMinuteMaths.Console;
using Xunit;

namespace MadMinuteMaths.Console.Tests;

public class PdfGeneratorIntegrationTests
{
    [Fact]
    public void CreatePdfFromLatex_CompilesPdfWithPdflatex()
    {
        var tempOutput = Path.Combine(Path.GetTempPath(), $"latex-{Guid.NewGuid():N}.pdf");
        var generator = new PdfGenerator(new DefaultProcessRunner());
        var latex = "\\documentclass{article}\\begin{document}Hello from LaTeX.\\end{document}";

        try
        {
            generator.CreatePdfFromLatex(latex, tempOutput);

            Assert.True(File.Exists(tempOutput));
            Assert.True(new FileInfo(tempOutput).Length > 0);
        }
        finally
        {
            if (File.Exists(tempOutput))
            {
                File.Delete(tempOutput);
            }
        }
    }
}
