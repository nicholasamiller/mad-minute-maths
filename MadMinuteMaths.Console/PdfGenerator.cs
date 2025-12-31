using System;
using System.Diagnostics;
using System.IO;

namespace MadMinuteMaths.Console;

public sealed class PdfGenerator
{
    private readonly IProcessRunner _processRunner;

    public PdfGenerator(IProcessRunner processRunner)
    {
        _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
    }

    public void CreatePdfFromLatex(string latex, string outputPdfPath)
    {
        if (string.IsNullOrWhiteSpace(latex))
        {
            throw new ArgumentException("LaTeX content must be provided.", nameof(latex));
        }

        if (string.IsNullOrWhiteSpace(outputPdfPath))
        {
            throw new ArgumentException("Output path must be provided.", nameof(outputPdfPath));
        }

        var outputDirectory = Path.GetDirectoryName(outputPdfPath);
        if (!string.IsNullOrWhiteSpace(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        var workingDirectory = Path.Combine(Path.GetTempPath(), "mad-minute-maths", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(workingDirectory);

        try
        {
            var texPath = Path.Combine(workingDirectory, "document.tex");
            File.WriteAllText(texPath, latex);

            var arguments = $"-interaction=nonstopmode -halt-on-error -output-directory \"{workingDirectory}\" \"{texPath}\"";
            var result = _processRunner.Run("pdflatex", arguments, workingDirectory);

            if (result.ExitCode != 0)
            {
                throw new InvalidOperationException($"pdflatex failed with exit code {result.ExitCode}.\n{result.StandardOutput}\n{result.StandardError}");
            }

            var generatedPdfPath = Path.Combine(workingDirectory, "document.pdf");
            if (!File.Exists(generatedPdfPath))
            {
                throw new FileNotFoundException("PDF output was not created by pdflatex.", generatedPdfPath);
            }

            File.Copy(generatedPdfPath, outputPdfPath, true);
        }
        finally
        {
            if (Directory.Exists(workingDirectory))
            {
                Directory.Delete(workingDirectory, true);
            }
        }
    }
}

public interface IProcessRunner
{
    ProcessResult Run(string fileName, string arguments, string workingDirectory);
}

public sealed class ProcessResult
{
    public ProcessResult(int exitCode, string standardOutput, string standardError)
    {
        ExitCode = exitCode;
        StandardOutput = standardOutput;
        StandardError = standardError;
    }

    public int ExitCode { get; }
    public string StandardOutput { get; }
    public string StandardError { get; }
}

public sealed class DefaultProcessRunner : IProcessRunner
{
    public ProcessResult Run(string fileName, string arguments, string workingDirectory)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = Process.Start(startInfo);
        if (process is null)
        {
            throw new InvalidOperationException("Failed to start pdflatex process.");
        }

        var standardOutput = process.StandardOutput.ReadToEnd();
        var standardError = process.StandardError.ReadToEnd();
        process.WaitForExit();

        return new ProcessResult(process.ExitCode, standardOutput, standardError);
    }
}
