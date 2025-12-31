using System;
using System.IO;
using MadMinuteMaths.Console;
using Xunit;

namespace MadMinuteMaths.Console.Tests;

public class PdfGeneratorTests
{
    [Fact]
    public void CreatePdfFromLatex_WritesPdfToOutputPath()
    {
        var tempOutput = Path.Combine(Path.GetTempPath(), $"pdf-{Guid.NewGuid():N}.pdf");
        var runner = new FakeProcessRunner();
        var generator = new PdfGenerator(runner);

        generator.CreatePdfFromLatex("\\documentclass{article}\\begin{document}Hi\\end{document}", tempOutput);

        Assert.True(File.Exists(tempOutput));
        Assert.Equal(runner.GeneratedPdfContents, File.ReadAllBytes(tempOutput));
    }

    private sealed class FakeProcessRunner : IProcessRunner
    {
        public byte[] GeneratedPdfContents { get; } = { 37, 80, 68, 70, 45, 49, 46, 55 };

        public ProcessResult Run(string fileName, string arguments, string workingDirectory)
        {
            var texPath = Path.Combine(workingDirectory, "document.tex");
            Assert.True(File.Exists(texPath));
            Assert.Contains("\\documentclass", File.ReadAllText(texPath));

            var pdfPath = Path.Combine(workingDirectory, "document.pdf");
            File.WriteAllBytes(pdfPath, GeneratedPdfContents);

            return new ProcessResult(0, string.Empty, string.Empty);
        }
    }
}
