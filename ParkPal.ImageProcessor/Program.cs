using System.Text;
using Blurhash.ImageSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

Console.WriteLine("🎢 Starting ParkPal Image Processing Pipeline...");

// 1. Set your Mac folders up (Swap 'OldImages' for wherever you saved them!)
var inputFolder = "/Users/tom/Downloads/parks2"; 
var outputFolder = "/Users/tom/Git/DevItUp/ParkPal/ParkPal.ImageProcessor/ImagesProcessed-3";
var sqlOutputFile = "/Users/tom/Git/DevItUp/ParkPal/ParkPal.DatabaseDeployer/Scripts/UpgradeScript_1.0.27.sql";

Directory.CreateDirectory(outputFolder);
var sqlBuilder = new StringBuilder();

sqlBuilder.AppendLine("-- Update the rows with our generated data");

var files = Directory.GetFiles(inputFolder);

foreach (var file in files)
{
    // Ignore any sneaky hidden Mac files like .DS_Store!
    if (Path.GetFileName(file).StartsWith(".")) continue;

    var attractionId = Path.GetFileNameWithoutExtension(file);
    Console.WriteLine($"Processing: {attractionId}...");

    using var image = Image.Load<Rgba32>(file);

    if (image.Width > 800)
    {
        image.Mutate(x => x.Resize(800, 0)); 
    }

    var blurHash = Blurhasher.Encode(image, 4, 3);

    var webpFileName = $"{attractionId}.webp";
    var outputPath = Path.Combine(outputFolder, webpFileName);
    image.SaveAsWebp(outputPath);

    var sql = $@"UPDATE ""Static"".""Attraction"" SET ""ImageUrl"" = '{webpFileName}', ""ImageBlurHash"" = '{blurHash}' WHERE ""AttractionId"" = '{attractionId}';";
    sqlBuilder.AppendLine(sql);
}

File.WriteAllText(sqlOutputFile, sqlBuilder.ToString());

Console.WriteLine("✅ Done! Images compressed and SQL script generated.");