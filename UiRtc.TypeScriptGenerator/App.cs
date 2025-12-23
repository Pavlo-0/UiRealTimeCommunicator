using Cocona;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using UiRtc.TypeScriptGenerator;
using Tapper;

public class App : CoconaConsoleAppBase
{
    private readonly ILogger<App> _logger;

    public App(ILogger<App> logger)
    {
        _logger = logger;
    }

    [Command(Description = "Transpile a C# project to the specified output directory.")]
    public async Task Generator(
        [Option('p', Description = "Path to the project file (Xxx.csproj)")] string project,
        [Option('o', Description = "Output directory and file")] string output)
    {
        _logger.Log(LogLevel.Information, "Generating...");
        try
        {
            if (!File.Exists(project))
            {
                _logger.Log(LogLevel.Error, "The project file {path} does not exist.", Path.GetFullPath(project));
                return;
            }
            var dataCollectionService = new DataCollectorService(_logger);

            //Generate type script per model for consumers and senders
            var tsModels = await dataCollectionService.TsModelGenerator(project, this.Context.CancellationToken);

            var (senderCollection, handlerCollection) = await dataCollectionService.ExtractHubsContracts(project, this.Context.CancellationToken);

            var contractGenerator = new TsGeneratorService(_logger);

            // Determine output paths
            var outputFullPath = Path.GetFullPath(output);
            string outputDirectory;
            if (Path.HasExtension(outputFullPath))
            {
                outputDirectory = Path.GetDirectoryName(outputFullPath) ?? outputFullPath;
            }
            else
            {
                outputDirectory = outputFullPath;
            }

            if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // Generate main contract using models collection; TsGeneratorService will save models files to outputDirectory
            var tsContract = contractGenerator.GenerateService(senderCollection, handlerCollection, tsModels, outputDirectory);

            // Write main contract file (if output was a directory, create default file name)
            var contractFilePath = Path.HasExtension(outputFullPath) ? outputFullPath : Path.Combine(outputDirectory, "contract.ts");

            WriteToFile(contractFilePath, tsContract);

            _logger.Log(LogLevel.Information, "Generating done");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, ex, $"An error occurred during generating typeScript file. Message: {ex.Message}");
        }
    }

    private void WriteToFile(string fullPath, string content)
    {
        try
        {
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            else
            {
                // Delete the file if it exists
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            File.WriteAllText(fullPath, content);
        }
        catch (IOException ex)
        {
            _logger.Log(LogLevel.Error, $"Error writing file {fullPath}: {ex.Message}");
            throw;
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.Log(LogLevel.Error, $"Access denied to {fullPath}: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, $"Internal error during writing file to {fullPath}: {ex.Message}");
            throw;
        }
    }
}
